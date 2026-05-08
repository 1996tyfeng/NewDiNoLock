# 桌面宠物功能设计文档

## 1. 文档信息

| 项目 | 内容 |
| --- | --- |
| 项目名称 | NewDiNoLock 桌面宠物 |
| 文档类型 | 功能设计文档 |
| 当前版本 | v0.1 |
| 创建日期 | 2026-05-08 |
| 目标平台 | 优先 Windows 桌面，后续可扩展 macOS |
| 核心定位 | 陪伴型桌面宠物 + 办公提醒助手 |

## 2. 产品目标

NewDiNoLock 是一个基于 Unity 的桌面宠物应用。它以“轻陪伴、低打扰、可扩展”为核心原则，在用户工作时提供一个常驻桌面的可互动角色，并逐步集成番茄钟、喝水提醒、站立提醒、待办提醒、通讯工具消息提示等办公辅助能力。

产品需要做到：

- 桌面宠物能够稳定显示在桌面或其他窗口上方。
- 宠物拥有基础生命感：待机、行走、拖拽、互动、隐藏、换肤等。
- 办公提醒功能以温和、可配置的方式出现，避免频繁打断用户。
- 后续能力可以通过模块扩展，不影响宠物核心表现逻辑。

## 3. 用户场景

### 3.1 日常陪伴

用户打开应用后，宠物常驻桌面边缘或空闲区域。用户工作时，宠物会偶尔走动、待机、做小动作，增加陪伴感。

### 3.2 鼠标互动

用户可以点击、拖动、右键打开菜单。拖动宠物时，宠物切换到“被提起”动作；松开后落到新位置并恢复待机或行走。

### 3.3 办公提醒

用户开启番茄钟、喝水提醒、站立提醒、待办提醒后，宠物会用气泡、动作、音效或轻量弹窗进行提示。

### 3.4 临时隐藏

用户在开会、演示、全屏游戏或需要专注时，可以快速隐藏宠物，并在托盘菜单或快捷键中恢复。

## 4. 设计原则

- 默认低打扰：宠物默认不遮挡用户主要工作区域。
- 行为可关闭：自动走动、全屏置顶、防锁屏、提醒音效等都应可配置。
- 状态清晰：宠物行为由明确状态机驱动，避免动作互相抢占。
- 资源可替换：皮肤、动画、音效、交互配置应数据化。
- 扩展友好：番茄钟、提醒、通讯工具接入等作为插件式功能模块接入。

## 5. 功能范围

### 5.1 MVP 功能

| 模块 | 功能 | 优先级 | 说明 |
| --- | --- | --- | --- |
| 窗口显示 | 透明窗口 | P0 | Unity 窗口背景透明，仅显示宠物内容 |
| 窗口显示 | 置顶显示 | P0 | 支持桌面置顶 |
| 窗口显示 | 全屏上方显示开关 | P0 | 用户可选择是否覆盖全屏软件 |
| 宠物行为 | 待机动作 | P0 | 默认循环播放 |
| 宠物行为 | 屏幕范围内随机行走 | P0 | 可配置开启/关闭 |
| 宠物行为 | 鼠标拖动 | P0 | 支持拖拽改变位置 |
| 宠物行为 | 拖动特殊动作 | P0 | 拖动中播放“被提起”动作 |
| 互动 | 点击互动 | P0 | 点击后播放反馈动作或气泡 |
| 互动 | 右键菜单 | P0 | 显示隐藏、设置、换肤、退出等 |
| 系统能力 | 运行时防锁屏 | P1 | 可配置开启/关闭 |
| 显示控制 | 隐藏/显示 | P0 | 支持菜单或快捷键隐藏 |
| 皮肤系统 | 更换皮肤 | P0 | 支持 Spine 与帧动画两类皮肤 |
| 设置系统 | 本地设置保存 | P0 | 保存置顶、走动、皮肤、防锁屏等配置 |

### 5.2 后续扩展功能

| 模块 | 功能 | 优先级 | 说明 |
| --- | --- | --- | --- |
| 番茄钟 | 专注/休息周期 | P1 | 支持自定义时长、开始、暂停、结束 |
| 健康提醒 | 喝水提醒 | P1 | 定时提醒，可延后 |
| 健康提醒 | 站立提醒 | P1 | 定时提醒，可延后 |
| 待办事项 | 待办创建与提醒 | P2 | 支持简单待办、定时提醒 |
| 通讯工具 | 新消息提示 | P3 | 接入企业微信、飞书、钉钉、Slack 等 |
| 角色成长 | 好感度/心情值 | P3 | 根据互动与办公习惯变化 |
| 扩展系统 | 功能插件管理 | P3 | 允许模块开关与独立配置 |

## 6. 核心功能设计

### 6.1 桌面窗口显示

#### 6.1.1 透明窗口

应用窗口需要呈现为无边框、透明背景，只保留宠物渲染内容。窗口不应显示标题栏、系统边框或默认背景。

预期表现：

- 宠物以透明背景叠加在桌面上。
- 非宠物区域尽量允许鼠标穿透，避免阻挡桌面操作。
- 宠物可交互区域保留点击、拖动、右键菜单能力。

#### 6.1.2 置顶策略

置顶模式分为三档：

| 模式 | 说明 | 适用场景 |
| --- | --- | --- |
| 普通桌面层 | 不主动置顶 | 用户只希望宠物在桌面可见 |
| 普通置顶 | 位于常规窗口上方 | 办公场景默认推荐 |
| 全屏置顶 | 尝试位于全屏软件上方 | 看视频、演示、游戏等场景可选 |

注意事项：

- 全屏置顶行为与操作系统、显卡、全屏模式有关，需在设置中明确为“实验性/兼容性选项”。
- 当检测到全屏应用运行时，如果用户关闭“全屏上方显示”，宠物应自动降低层级或隐藏。

### 6.2 宠物行为系统

宠物行为由状态机统一管理。任一时刻只允许一个主状态生效，避免行走、互动、拖动等动作冲突。

#### 6.2.1 状态定义

| 状态 | 说明 | 可被打断 | 进入条件 | 退出条件 |
| --- | --- | --- | --- | --- |
| Idle | 待机 | 是 | 无任务、无输入 | 计时结束、收到输入 |
| Walk | 行走 | 是 | 自动行走触发 | 到达目标、被点击/拖动 |
| Dragged | 被拖动 | 否 | 鼠标按住并拖动宠物 | 鼠标松开 |
| Interact | 互动反馈 | 是 | 点击、抚摸、菜单动作 | 动画结束 |
| Notify | 提醒表现 | 视优先级 | 番茄钟/喝水/待办触发 | 用户确认、超时 |
| Hidden | 隐藏 | 否 | 用户隐藏 | 用户恢复 |
| Sleep | 休眠/低活跃 | 是 | 长时间无交互或专注模式 | 用户交互、提醒触发 |

#### 6.2.2 行为优先级

当多个行为同时触发时，按以下优先级处理：

1. Hidden
2. Dragged
3. 高优先级 Notify
4. Interact
5. Walk
6. Idle
7. Sleep

#### 6.2.3 随机行走

随机行走用于提升陪伴感，但必须避免打扰。

规则：

- 用户可在设置中关闭自动行走。
- 行走目标点必须位于当前屏幕可见区域内。
- 宠物不应走到屏幕外，至少保留主要身体可见。
- 行走频率使用随机间隔，例如 15-60 秒。
- 鼠标悬停、拖动、互动、提醒时暂停自动行走。
- 支持多显示器时，默认限制在宠物当前所在显示器，后续可支持跨屏移动。

#### 6.2.4 拖动

用户在宠物有效点击区域按下鼠标并移动时进入 Dragged 状态。

交互细节：

- 按下后记录鼠标与宠物根节点偏移。
- 拖动中宠物跟随鼠标移动。
- 拖动中播放 Lift/Hold 动画。
- 松开时播放 Drop/Land 动画，随后进入 Idle。
- 如果松开位置超出屏幕，自动吸附到最近的可见边界。

### 6.3 基础互动

MVP 阶段提供轻量互动：

| 操作 | 反馈 |
| --- | --- |
| 左键单击 | 播放随机互动动作或显示短气泡 |
| 左键双击 | 触发特殊动作，后续可打开快捷面板 |
| 鼠标悬停 | 可选地看向鼠标或播放注视动作 |
| 右键 | 打开桌面宠物菜单 |
| 拖动 | 切换被提起动作 |

互动反馈应避免过长，建议单次反馈 1-3 秒。

### 6.4 隐藏与恢复

隐藏方式：

- 右键菜单点击“隐藏”。
- 托盘菜单点击“隐藏/显示”。
- 可选快捷键，例如 Ctrl + Alt + D。
- 全屏应用自动隐藏，取决于用户设置。

隐藏后行为：

- 主窗口不可见。
- 提醒模块可以继续运行。
- 到达重要提醒时，根据设置决定是否临时显示气泡或仅托盘通知。

### 6.5 防锁屏

防锁屏用于避免用户工作时电脑自动锁屏或睡眠。

设计要求：

- 默认关闭，由用户主动开启。
- 开启后在应用运行期间定期向系统报告“正在使用”状态。
- 提供状态提示，例如设置页显示“防锁屏已开启”。
- 应用退出、崩溃恢复或用户关闭开关时，必须释放防锁屏状态。

Windows 预期实现方向：

- 使用系统 API 设置执行状态，防止显示器关闭或系统睡眠。
- 区分“防睡眠”和“防锁屏/屏幕关闭”配置，MVP 可先合并为一个开关。

### 6.6 皮肤与动画系统

皮肤系统需要同时支持 Spine 和帧动画。业务层不直接关心具体动画实现，而是通过统一接口播放动作。

#### 6.6.1 皮肤类型

| 类型 | 说明 | 适用 |
| --- | --- | --- |
| SpineSkin | Spine 骨骼动画皮肤 | 高表现力角色 |
| FrameSkin | 帧动画皮肤 | 像素风、轻量角色 |

#### 6.6.2 统一动作名

所有皮肤需要尽量提供统一动作集合：

| 动作名 | 必需 | 说明 |
| --- | --- | --- |
| Idle | 是 | 待机 |
| Walk | 是 | 行走 |
| Lift | 是 | 被提起/拖动 |
| Drop | 否 | 放下 |
| Click | 是 | 点击反馈 |
| Happy | 否 | 开心 |
| Sleep | 否 | 休眠 |
| Notify | 否 | 提醒 |

如果某个皮肤缺少动作，应回退到 Idle 或 Click。

#### 6.6.3 皮肤配置

每个皮肤建议配置为独立目录：

```text
Skins/
  DinoDefault/
    skin.json
    spine/
    frames/
    icon.png
    preview.png
```

`skin.json` 建议字段：

```json
{
  "id": "dino_default",
  "displayName": "Default Dino",
  "type": "Spine",
  "scale": 1.0,
  "pivot": { "x": 0.5, "y": 0.0 },
  "hitArea": { "x": 0, "y": 0, "width": 160, "height": 180 },
  "animations": {
    "Idle": "idle",
    "Walk": "walk",
    "Lift": "lift",
    "Drop": "drop",
    "Click": "click",
    "Notify": "notify"
  }
}
```

### 6.7 设置系统

设置项需要本地持久化。MVP 可以使用 JSON 文件，后续如设置复杂再考虑 ScriptableObject 默认配置 + 用户 JSON 覆盖。

建议设置项：

| 设置项 | 类型 | 默认值 | 说明 |
| --- | --- | --- | --- |
| alwaysOnTop | bool | true | 是否置顶 |
| showAboveFullscreen | bool | false | 是否尝试显示在全屏软件上方 |
| autoWalkEnabled | bool | true | 是否自动行走 |
| keepAwakeEnabled | bool | false | 是否防锁屏 |
| currentSkinId | string | dino_default | 当前皮肤 |
| volume | float | 0.5 | 音量 |
| interactionBubbleEnabled | bool | true | 是否显示互动气泡 |
| autoHideInFullscreen | bool | true | 全屏时是否自动隐藏 |
| restorePositionOnStart | bool | true | 启动时恢复上次位置 |
| petPosition | object | null | 上次宠物位置 |

### 6.8 菜单设计

右键菜单 MVP 项：

- 显示/隐藏
- 锁定位置
- 开启/关闭自动走动
- 开启/关闭置顶
- 开启/关闭全屏上方显示
- 开启/关闭防锁屏
- 更换皮肤
- 设置
- 退出

托盘菜单 MVP 项：

- 显示/隐藏宠物
- 快速开启/关闭防锁屏
- 快速开启/关闭自动走动
- 设置
- 退出

## 7. 办公扩展功能设计

### 7.1 番茄钟

核心能力：

- 开始专注。
- 暂停/继续。
- 结束当前轮。
- 自动进入休息。
- 支持自定义专注时长、短休息、长休息、循环次数。

提醒表现：

- 专注开始：宠物进入安静状态，减少随机走动。
- 专注结束：宠物播放提醒动作并显示气泡。
- 休息结束：宠物提示回到工作。

### 7.2 喝水与站立提醒

提醒规则：

- 支持固定间隔提醒。
- 支持延后 5/10/15 分钟。
- 支持工作时间段限制。
- 支持勿扰模式。

提醒表现：

- 宠物气泡。
- 托盘通知。
- 可选音效。
- 可选强提示动作。

### 7.3 待办事项提醒

MVP 后的最小形态：

- 创建待办事项。
- 设置提醒时间。
- 提醒到达时由宠物展示。
- 支持完成、延后、忽略。

后续增强：

- 每日清单。
- 周期任务。
- 与系统日历或第三方待办工具同步。

### 7.4 通讯工具消息提示

通讯接入作为后期扩展，建议不要和核心宠物逻辑耦合。

接入方式：

- 本地客户端通知监听。
- 官方 API 或 Webhook。
- 浏览器/桌面通知桥接。

提示原则：

- 默认只显示来源和摘要。
- 隐私模式下只显示“有新消息”。
- 可按应用、联系人、群组设置提醒强度。

## 8. 系统架构设计

### 8.1 模块划分

```text
DesktopPetApp
  Core
    PetStateMachine
    PetBehaviorController
    PetInteractionController
  Rendering
    SkinManager
    IAnimationPlayer
    SpineAnimationPlayer
    FrameAnimationPlayer
  Window
    DesktopWindowService
    AlwaysOnTopService
    FullscreenDetectService
    TrayService
  System
    KeepAwakeService
    HotkeyService
    SettingsService
  Features
    Pomodoro
    HealthReminder
    TodoReminder
    MessageNotifier
  UI
    ContextMenu
    SettingsPanel
    BubbleView
```

### 8.2 关键接口

#### 8.2.1 动画播放接口

```csharp
public interface IPetAnimationPlayer
{
    string CurrentAnimation { get; }
    bool HasAnimation(string animationName);
    void Play(string animationName, bool loop = false);
    void SetFlipX(bool flipX);
    void SetSkin(string skinId);
}
```

#### 8.2.2 系统窗口接口

```csharp
public interface IDesktopWindowService
{
    void Initialize();
    void SetTransparent(bool enabled);
    void SetClickThrough(bool enabled);
    void SetAlwaysOnTop(bool enabled);
    void SetShowAboveFullscreen(bool enabled);
    void SetVisible(bool visible);
}
```

#### 8.2.3 提醒接口

```csharp
public interface IReminderFeature
{
    string Id { get; }
    bool Enabled { get; }
    void Start();
    void Stop();
}
```

### 8.3 事件流

建议使用轻量事件总线或 C# event 解耦模块。

常见事件：

| 事件 | 发送方 | 接收方 |
| --- | --- | --- |
| PetClicked | InteractionController | StateMachine, BubbleView |
| PetDragStarted | InteractionController | StateMachine, WindowService |
| PetDragEnded | InteractionController | StateMachine, SettingsService |
| SkinChanged | SettingsPanel | SkinManager |
| ReminderTriggered | Feature Module | StateMachine, BubbleView, TrayService |
| FullscreenAppDetected | FullscreenDetectService | WindowService, StateMachine |
| SettingsChanged | SettingsService | All Modules |

## 9. 数据设计

### 9.1 用户设置

用户设置保存到本地，例如：

```text
%AppData%/NewDiNoLock/settings.json
```

建议按模块拆分：

```json
{
  "window": {
    "alwaysOnTop": true,
    "showAboveFullscreen": false,
    "autoHideInFullscreen": true
  },
  "pet": {
    "autoWalkEnabled": true,
    "currentSkinId": "dino_default",
    "restorePositionOnStart": true,
    "position": { "displayIndex": 0, "x": 1200, "y": 760 }
  },
  "system": {
    "keepAwakeEnabled": false,
    "volume": 0.5
  },
  "features": {
    "pomodoro": { "enabled": false },
    "healthReminder": { "enabled": false },
    "todoReminder": { "enabled": false }
  }
}
```

### 9.2 提醒数据

后续可将提醒数据保存为：

```text
%AppData%/NewDiNoLock/reminders.json
```

待办提醒数据示例：

```json
{
  "items": [
    {
      "id": "todo_001",
      "title": "Review weekly notes",
      "remindAt": "2026-05-08T15:30:00+08:00",
      "status": "Pending"
    }
  ]
}
```

## 10. 非功能需求

### 10.1 性能

- 空闲时 CPU 占用应尽量低。
- 未播放复杂动作时降低不必要的 Update 逻辑。
- 帧动画资源需要控制纹理尺寸和内存占用。
- 后台隐藏时可降低渲染频率或暂停部分动画。

### 10.2 稳定性

- 应用异常退出时不应残留防锁屏状态。
- 设置文件损坏时应自动回退默认配置。
- 皮肤加载失败时回退默认皮肤。
- 系统 API 调用失败时应记录日志，不阻塞主流程。

### 10.3 兼容性

- Windows 作为首发平台。
- 多显示器、缩放比例、任务栏位置需要纳入测试。
- 全屏置顶属于高风险兼容功能，应提供可关闭开关。
- macOS 后续需要单独适配透明窗口、置顶、托盘、防睡眠等能力。

### 10.4 隐私

- 本地提醒和待办默认保存在本机。
- 通讯工具接入前必须设计授权与隐私模式。
- 消息提示默认不展示敏感正文，允许用户关闭摘要。

## 11. 里程碑建议

### 11.1 阶段一：桌面宠物基础可用

目标：宠物能透明置顶显示，可拖动、可互动、可隐藏。

交付内容：

- 透明无边框窗口。
- 置顶开关。
- 宠物状态机。
- Idle/Walk/Lift/Click 动作。
- 鼠标拖动。
- 右键菜单。
- 本地设置保存。

### 11.2 阶段二：皮肤与系统能力完善

目标：宠物表现可替换，系统功能可配置。

交付内容：

- Spine 皮肤加载。
- 帧动画皮肤加载。
- 皮肤切换 UI。
- 防锁屏开关。
- 全屏检测与自动隐藏。
- 托盘菜单。

### 11.3 阶段三：办公提醒 MVP

目标：桌面宠物从陪伴角色变为轻办公助手。

交付内容：

- 番茄钟。
- 喝水提醒。
- 站立提醒。
- 提醒气泡。
- 提醒延后/完成/忽略。

### 11.4 阶段四：办公助手扩展

目标：提供更多与工作流相关的能力。

交付内容：

- 待办事项提醒。
- 通讯工具消息提示。
- 隐私模式。
- 功能模块管理。

## 12. 风险与待确认问题

| 风险/问题 | 影响 | 建议 |
| --- | --- | --- |
| 全屏置顶兼容性 | 部分全屏软件可能无法覆盖 | 设置为可选能力，优先支持无边框全屏 |
| 鼠标穿透与交互冲突 | 可能导致宠物无法点击或阻挡桌面 | 按宠物碰撞区域动态切换穿透 |
| Spine 授权与包管理 | 影响构建和发布 | 尽早确认 Spine Unity Runtime 使用方式 |
| 多显示器坐标 | 拖动和随机行走可能越界 | 抽象 ScreenBoundsService |
| 防锁屏行为边界 | 用户可能不希望长期阻止睡眠 | 默认关闭，并在 UI 中明确状态 |
| 通讯工具接入 | 涉及隐私、授权和平台限制 | 后期作为独立插件模块设计 |

## 13. 验收标准

MVP 验收标准：

- 启动应用后，宠物可以透明显示在桌面上。
- 开启置顶后，宠物位于普通窗口上方。
- 用户可以拖动宠物，拖动时播放 Lift 动作，松开后停在新位置。
- 开启自动行走后，宠物会在屏幕范围内随机移动。
- 关闭自动行走后，宠物保持待机，不再主动移动。
- 用户可以右键打开菜单并执行隐藏、显示、退出。
- 更换皮肤后，宠物外观和动画正常切换。
- 开启防锁屏后，应用运行期间系统不会自动进入睡眠/锁屏流程。
- 关闭应用后，防锁屏状态被释放。

## 14. 后续设计文档建议

建议继续补充以下文档：

- 技术方案设计：Unity 透明窗口、置顶、防锁屏、托盘、多显示器适配。
- 宠物状态机详细设计：状态图、状态切换条件、动画回退规则。
- 皮肤资源规范：Spine 导出要求、帧动画命名、目录结构、碰撞区域配置。
- 提醒系统设计：番茄钟、健康提醒、待办提醒的数据模型与调度方式。
- UI 设计稿：右键菜单、托盘菜单、设置面板、提醒气泡。
