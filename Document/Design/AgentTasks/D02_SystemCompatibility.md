# D02 系统兼容能力任务

## 任务目标

实现防锁屏、全屏应用检测、自动隐藏和全局快捷键等系统能力。

## 推荐模型

GPT-5.5 高。

## 必读文档

- `../DesktopPet_FunctionalDesign.md` 中 6.5、6.1.2
- `../Project_CodeStructure_Guidelines.md` 中 System、Window、平台适配规范
- `../WorkBreakdown_Priority_ModelGuide.md` 中 T4.1、T4.2、T4.3

## 负责范围

主要目录：

```text
Assets/NewDiNoLock/Scripts/System/
Assets/NewDiNoLock/Scripts/Window/
Assets/NewDiNoLock/Scripts/UI/
Assets/NewDiNoLock/Scripts/Bootstrap/
```

建议文件：

```text
KeepAwakeService.cs
FullscreenDetectService.cs
HotkeyService.cs
```

## 功能要求

防锁屏：

- 默认关闭。
- 开启后阻止系统自动睡眠或锁屏。
- 关闭或应用退出后释放状态。

全屏检测：

- 检测当前是否有全屏应用。
- 根据 `autoHideInFullscreen` 自动隐藏或恢复。
- 不频繁闪烁或反复切层级。

快捷键：

- 支持显示/隐藏宠物。
- 快捷键冲突时有日志或提示。

## 不要修改

- 不实现宠物业务动画。
- 不把平台 API 写到 UI 或 Core。
- 不绕过设置服务直接保存配置。

## 验收标准

- 防锁屏开关生效并可释放。
- 全屏应用进入/退出时行为符合设置。
- 非 Windows 平台安全回退。

## 交付说明

说明平台 API、测试方式、兼容性风险和退出清理结果。

