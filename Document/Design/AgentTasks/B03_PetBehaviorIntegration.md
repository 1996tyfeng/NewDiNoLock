# B03 宠物行为调度集成任务

## 任务目标

把状态机、动画接口和基础行为控制连接起来，让宠物启动后能进入 Idle，并能响应状态变化播放动作。

## 推荐模型

GPT-5.5 高。

## 必读文档

- `../DesktopPet_FunctionalDesign.md` 中 6.2、6.3
- `../Project_CodeStructure_Guidelines.md` 中 Core、Rendering
- `../WorkBreakdown_Priority_ModelGuide.md` 中 T1.3

## 负责范围

主要目录：

```text
Assets/NewDiNoLock/Scripts/Core/
Assets/NewDiNoLock/Prefabs/Pet/
```

建议文件：

```text
PetBehaviorController.cs
PetMovementController.cs 或预留接口
PetInteractionController.cs 或预留接口
```

## 功能要求

- 启动后宠物进入 Idle。
- 状态切换驱动动画播放。
- Walk 播放 Walk 动作，Dragged 播放 Lift，Interact 播放 Click，Notify 播放 Notify 或回退动作。
- 行为调度只依赖 `IPetAnimationPlayer`。
- 为后续自动行走、拖动、提醒留出清晰入口。

## 不要修改

- 不实现 Windows 透明窗口。
- 不实现完整皮肤系统。
- 不让 UI 直接抢状态。

## 验收标准

- 在场景中可看到宠物基础对象。
- Idle 状态和动画可自动运行。
- 手动触发状态切换时动画响应正确。

## 交付说明

说明集成关系、状态到动画的映射、需要后续任务补充的入口。

