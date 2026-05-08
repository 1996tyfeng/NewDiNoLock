#!/usr/bin/env node
import crypto from "node:crypto";
import http from "node:http";
import { Buffer } from "node:buffer";

const PORT = Number(process.env.UNITY_MCP_WS_PORT || 8080);
const HOST = process.env.UNITY_MCP_WS_HOST || "127.0.0.1";

let unitySocket = null;
let latestEditorState = null;
let latestLogs = [];
let commandWaiters = [];

function sendMcp(message) {
  process.stdout.write(`${JSON.stringify(message)}\n`);
}

function ok(id, result = {}) {
  sendMcp({ jsonrpc: "2.0", id, result });
}

function fail(id, code, message) {
  sendMcp({ jsonrpc: "2.0", id, error: { code, message } });
}

function toolResult(id, text) {
  ok(id, { content: [{ type: "text", text }] });
}

function connected() {
  return Boolean(unitySocket && !unitySocket.destroyed);
}

function sendWsJson(socket, payload) {
  const body = Buffer.from(JSON.stringify(payload), "utf8");
  let header;

  if (body.length < 126) {
    header = Buffer.from([0x81, body.length]);
  } else if (body.length < 65536) {
    header = Buffer.alloc(4);
    header[0] = 0x81;
    header[1] = 126;
    header.writeUInt16BE(body.length, 2);
  } else {
    header = Buffer.alloc(10);
    header[0] = 0x81;
    header[1] = 127;
    header.writeBigUInt64BE(BigInt(body.length), 2);
  }

  socket.write(Buffer.concat([header, body]));
}

function decodeWsFrames(socket, chunk) {
  socket._mcpBuffer = socket._mcpBuffer
    ? Buffer.concat([socket._mcpBuffer, chunk])
    : chunk;

  const messages = [];

  while (socket._mcpBuffer.length >= 2) {
    const buffer = socket._mcpBuffer;
    const opcode = buffer[0] & 0x0f;
    const masked = (buffer[1] & 0x80) !== 0;
    let length = buffer[1] & 0x7f;
    let offset = 2;

    if (length === 126) {
      if (buffer.length < offset + 2) break;
      length = buffer.readUInt16BE(offset);
      offset += 2;
    } else if (length === 127) {
      if (buffer.length < offset + 8) break;
      length = Number(buffer.readBigUInt64BE(offset));
      offset += 8;
    }

    const maskLength = masked ? 4 : 0;
    if (buffer.length < offset + maskLength + length) break;

    let payload = buffer.subarray(offset + maskLength, offset + maskLength + length);
    if (masked) {
      const mask = buffer.subarray(offset, offset + 4);
      payload = Buffer.from(payload.map((byte, index) => byte ^ mask[index % 4]));
    }

    socket._mcpBuffer = buffer.subarray(offset + maskLength + length);

    if (opcode === 0x8) {
      socket.end();
      break;
    }

    if (opcode === 0x1) {
      messages.push(payload.toString("utf8"));
    }
  }

  return messages;
}

function handleUnityMessage(raw) {
  let message;
  try {
    message = JSON.parse(raw);
  } catch {
    return;
  }

  if (message.type === "editorState") {
    latestEditorState = message.data;
    return;
  }

  if (message.type === "log") {
    latestLogs.push(message.data);
    latestLogs = latestLogs.slice(-200);
    return;
  }

  if (message.type === "commandResult") {
    const waiters = commandWaiters;
    commandWaiters = [];
    for (const waiter of waiters) waiter.resolve(message.data);
  }
}

const wsServer = http.createServer();

wsServer.on("upgrade", (request, socket) => {
  const key = request.headers["sec-websocket-key"];
  if (!key) {
    socket.destroy();
    return;
  }

  const accept = crypto
    .createHash("sha1")
    .update(`${key}258EAFA5-E914-47DA-95CA-C5AB0DC85B11`)
    .digest("base64");

  socket.write(
    "HTTP/1.1 101 Switching Protocols\r\n" +
      "Upgrade: websocket\r\n" +
      "Connection: Upgrade\r\n" +
      `Sec-WebSocket-Accept: ${accept}\r\n\r\n`
  );

  if (unitySocket && !unitySocket.destroyed) unitySocket.destroy();
  unitySocket = socket;
  socket.on("data", (chunk) => {
    for (const text of decodeWsFrames(socket, chunk)) {
      handleUnityMessage(text);
    }
  });
  socket.on("close", () => {
    if (unitySocket === socket) unitySocket = null;
  });
  socket.on("error", () => {
    if (unitySocket === socket) unitySocket = null;
  });
});

wsServer.listen(PORT, HOST);

function waitForCommandResult(timeoutMs = 10000) {
  return new Promise((resolve, reject) => {
    const timer = setTimeout(() => {
      commandWaiters = commandWaiters.filter((waiter) => waiter.resolve !== resolve);
      reject(new Error("Timed out waiting for Unity command result."));
    }, timeoutMs);

    commandWaiters.push({
      resolve: (value) => {
        clearTimeout(timer);
        resolve(value);
      },
    });
  });
}

async function executeEditorCode(code) {
  if (!connected()) {
    throw new Error(`Unity is not connected. Open NewDiNoLock in Unity and wait for ws://${HOST}:${PORT}.`);
  }

  const resultPromise = waitForCommandResult();
  sendWsJson(unitySocket, {
    type: "executeEditorCommand",
    data: JSON.stringify({ code }),
  });
  return await resultPromise;
}

function statusText() {
  return JSON.stringify(
    {
      websocket: `ws://${HOST}:${PORT}`,
      unityConnected: connected(),
      hasEditorState: Boolean(latestEditorState),
      playModeState: latestEditorState?.playModeState || "Unknown",
      selectedObjects: latestEditorState?.selectedObjects || [],
      activeGameObjectCount: latestEditorState?.activeGameObjects?.length || 0,
      recentLogs: latestLogs.slice(-10),
    },
    null,
    2
  );
}

const tools = [
  {
    name: "unity_status",
    description: "Show whether the NewDiNoLock Unity Editor is connected and return recent editor state.",
    inputSchema: { type: "object", properties: {} },
  },
  {
    name: "unity_execute_editor_code",
    description: "Execute C# editor code in the connected NewDiNoLock Unity Editor.",
    inputSchema: {
      type: "object",
      properties: {
        code: { type: "string", description: "C# code to execute in the Unity Editor context." },
      },
      required: ["code"],
    },
  },
  {
    name: "unity_select_game_object",
    description: "Select a GameObject by name/path in the connected Unity scene.",
    inputSchema: {
      type: "object",
      properties: {
        path: { type: "string", description: "GameObject name or path." },
      },
      required: ["path"],
    },
  },
  {
    name: "unity_toggle_play_mode",
    description: "Toggle Unity Editor play mode.",
    inputSchema: { type: "object", properties: {} },
  },
];

async function handleMcp(request) {
  const { id, method, params } = request;

  if (method === "initialize") {
    ok(id, {
      protocolVersion: params?.protocolVersion || "2024-11-05",
      capabilities: { tools: {} },
      serverInfo: { name: "newdinolock-unity-mcp", version: "0.1.0" },
    });
    return;
  }

  if (method === "notifications/initialized") return;

  if (method === "tools/list") {
    ok(id, { tools });
    return;
  }

  if (method !== "tools/call") {
    fail(id, -32601, `Unsupported method: ${method}`);
    return;
  }

  const name = params?.name;
  const args = params?.arguments || {};

  try {
    if (name === "unity_status") {
      toolResult(id, statusText());
    } else if (name === "unity_execute_editor_code") {
      const result = await executeEditorCode(args.code);
      toolResult(id, JSON.stringify(result, null, 2));
    } else if (name === "unity_select_game_object") {
      if (!connected()) throw new Error("Unity is not connected.");
      sendWsJson(unitySocket, { type: "selectGameObject", data: args.path });
      toolResult(id, `Selection request sent: ${args.path}`);
    } else if (name === "unity_toggle_play_mode") {
      if (!connected()) throw new Error("Unity is not connected.");
      sendWsJson(unitySocket, { type: "togglePlayMode" });
      toolResult(id, "Toggle play mode request sent.");
    } else {
      fail(id, -32602, `Unknown tool: ${name}`);
    }
  } catch (error) {
    ok(id, {
      isError: true,
      content: [{ type: "text", text: error.message }],
    });
  }
}

let stdinBuffer = "";
process.stdin.setEncoding("utf8");
process.stdin.on("data", (chunk) => {
  stdinBuffer += chunk;
  let newlineIndex;
  while ((newlineIndex = stdinBuffer.indexOf("\n")) >= 0) {
    const line = stdinBuffer.slice(0, newlineIndex).trim();
    stdinBuffer = stdinBuffer.slice(newlineIndex + 1);
    if (!line) continue;

    let request;
    try {
      request = JSON.parse(line);
    } catch {
      continue;
    }

    handleMcp(request);
  }
});

process.on("SIGINT", () => process.exit(0));
process.on("SIGTERM", () => process.exit(0));
