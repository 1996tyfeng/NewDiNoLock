# B01 宠物状态机任务

## 任务目标

实现宠物主状态机和状态优先级，作为所有行为、动画和提醒的核心调度基础。

## 推荐模型

GPT-5.5 高。

## 必读文档

- `../DesktopPet_FunctionalDesign.md` 中 6.2
- `../Project_CodeStructure_Guidelines.md` 中 Core
- `../WorkBreakdown_Priority_ModelGuide.md` 中 T1.1

## 负责范围

主要目录：

```text
Assets/NewDiNoLock/Scripts/Core/
Assets/NewDiNoLock/Tests/EditMode/
```

建议文件：

```text
PetState.cs
PetStateMachine.cs
PetActionPriority.cs
PetStateChangedEvent.cs
```

## 状态要求

必须支持：

```text
Idle
Walk
Dragged
Interact
Notify
Hidden
Sleep
```

优先级：

```text
Hidden > Dragged > 高优先级 Notify > Interact > Walk > Idle > Sleep
```

## 功能要求

- 任一时刻只有一个主状态。
- 拖动状态不可被普通互动或自动行走打断。
- Hidden 状态不可被普通提醒打断。
- 状态变化发布事件。
- 提供清晰的请求状态切换 API。

## 不要修改

- 不直接播放 Spine 或 Animator。
- 不直接调用窗口服务。
- 不直接读写设置文件。

## 验收标准

- 有 EditMode 测试覆盖主要优先级和不可打断规则。
- 状态切换逻辑集中，不散落到 UI 或 Feature。
- API 便于后续行为调度接入。

## 交付说明

说明状态切换 API、优先级规则、测试覆盖情况。

