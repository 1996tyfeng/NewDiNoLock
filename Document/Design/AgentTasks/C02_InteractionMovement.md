# C02 拖动、点击互动与自动行走任务

## 任务目标

实现宠物基本交互闭环：点击反馈、鼠标拖动、拖动动作、随机行走和屏幕边界限制。

## 推荐模型

GPT-5.5 中。

## 必读文档

- `../DesktopPet_FunctionalDesign.md` 中 6.2、6.3
- `../Project_CodeStructure_Guidelines.md` 中 11.2、11.3
- `../WorkBreakdown_Priority_ModelGuide.md` 中 T2.3、T2.4

## 负责范围

主要目录：

```text
Assets/NewDiNoLock/Scripts/Core/
Assets/NewDiNoLock/Scripts/UI/
Assets/NewDiNoLock/Scripts/System/
```

建议文件：

```text
PetInteractionController.cs
PetMovementController.cs
PetBoundsService.cs
```

## 功能要求

点击互动：

- 左键单击进入 Interact 状态。
- 播放 Click 动作或回退动作。
- 可触发简单气泡事件。

拖动：

- 鼠标按住并移动进入 Dragged。
- 拖动中播放 Lift。
- 松开后播放 Drop 或回到 Idle。
- 松开位置超出屏幕时吸附到可见边界。

自动行走：

- 根据随机间隔选择目标点。
- 目标点必须在屏幕可见范围内。
- `autoWalkEnabled` 为 false 时不主动移动。
- 拖动、互动、提醒时暂停自动行走。

## 不要修改

- 不直接调用 Windows API。
- 不改变状态机优先级规则，除非先说明原因。
- 不实现完整皮肤系统。

## 验收标准

- 可拖动宠物改变位置。
- 拖动中不会触发自动行走。
- 自动行走不越出屏幕。
- 设置关闭后自动行走停止。

## 交付说明

说明交互入口、边界算法、状态机调用方式和手动验证结果。

