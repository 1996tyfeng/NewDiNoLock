# C03 隐藏显示、菜单与托盘任务

## 任务目标

实现隐藏/显示、鼠标穿透策略、右键菜单和托盘菜单，让用户能控制宠物显示与基础设置。

## 推荐模型

GPT-5.5 中或 GPT-5.4 高。

## 必读文档

- `../DesktopPet_FunctionalDesign.md` 中 6.4、6.8
- `../Project_CodeStructure_Guidelines.md` 中 UI、Window
- `../WorkBreakdown_Priority_ModelGuide.md` 中 T2.5、T2.6

## 负责范围

主要目录：

```text
Assets/NewDiNoLock/Scripts/Window/
Assets/NewDiNoLock/Scripts/UI/
Assets/NewDiNoLock/Scripts/Core/
Assets/NewDiNoLock/Art/Icons/
```

建议文件：

```text
ClickThroughService.cs
TrayService.cs
ContextMenuView.cs
SettingsMenuController.cs
```

## 功能要求

隐藏/显示：

- 右键菜单可隐藏宠物。
- 托盘菜单可恢复宠物。
- Hidden 状态接入状态机。

鼠标穿透：

- 非宠物区域尽量允许鼠标穿透。
- 宠物有效交互区域保留点击和拖动。

右键菜单：

- 显示/隐藏。
- 开启/关闭自动走动。
- 开启/关闭置顶。
- 开启/关闭全屏上方显示。
- 设置。
- 退出。

托盘菜单：

- 显示/隐藏宠物。
- 设置。
- 退出。

## 不要修改

- 不实现番茄钟等办公功能。
- 不把设置保存逻辑写进 View。
- 不直接抢动画状态。

## 验收标准

- 隐藏后宠物不可见。
- 托盘能恢复宠物。
- 菜单开关状态与设置一致。
- 退出流程正常触发生命周期清理。

## 交付说明

说明菜单项、设置同步方式、鼠标穿透限制和平台风险。

