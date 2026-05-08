# C01 桌面窗口基础任务

## 任务目标

实现 Windows 平台透明无边框窗口、置顶、全屏上方显示开关的基础能力。

## 推荐模型

GPT-5.5 高。

## 必读文档

- `../DesktopPet_FunctionalDesign.md` 中 6.1
- `../Project_CodeStructure_Guidelines.md` 中 Window、平台适配规范
- `../WorkBreakdown_Priority_ModelGuide.md` 中 T2.1、T2.2

## 负责范围

主要目录：

```text
Assets/NewDiNoLock/Scripts/Window/
Assets/NewDiNoLock/Scripts/System/
```

建议文件：

```text
IDesktopWindowService.cs
DesktopWindowService.cs
AlwaysOnTopService.cs
PlatformWindowHandle.cs
```

## 功能要求

- 获取 Unity 窗口句柄。
- 设置无边框。
- 设置透明窗口。
- 支持普通置顶开关。
- 支持 `showAboveFullscreen` 配置，但全屏置顶行为必须可关闭。
- 非 Windows 平台提供安全空实现。

## 平台要求

- Windows API 调用集中在 Window 模块。
- 使用 `#if UNITY_STANDALONE_WIN` 隔离平台代码。
- API 调用失败时记录日志，不阻断应用启动。

## 不要修改

- 不实现宠物状态机。
- 不实现拖动行为。
- 不实现托盘菜单，除非只是为接口预留。

## 验收标准

- 构建后窗口无边框。
- 背景透明，仅显示宠物内容。
- `alwaysOnTop` 开关生效。
- 配置关闭时不会强制覆盖全屏应用。

## 交付说明

说明使用的 Windows API、兼容性风险、Editor 与构建表现差异。

