# E01 提醒、番茄钟与健康提醒任务

## 任务目标

建立提醒公共模型，并实现番茄钟、喝水提醒、站立提醒。

## 推荐模型

提醒公共模型使用 GPT-5.5 中；番茄钟和健康提醒可用 GPT-5.4 高。

## 必读文档

- `../DesktopPet_FunctionalDesign.md` 中 7.1、7.2
- `../Project_CodeStructure_Guidelines.md` 中 Features、UI
- `../WorkBreakdown_Priority_ModelGuide.md` 中 T5.1、T5.2、T5.3

## 负责范围

主要目录：

```text
Assets/NewDiNoLock/Scripts/Features/Common/
Assets/NewDiNoLock/Scripts/Features/Pomodoro/
Assets/NewDiNoLock/Scripts/Features/HealthReminder/
Assets/NewDiNoLock/Scripts/UI/
Assets/NewDiNoLock/Scripts/System/
```

建议文件：

```text
IReminderFeature.cs
ReminderEvent.cs
ReminderPriority.cs
PomodoroFeature.cs
PomodoroSettings.cs
PomodoroState.cs
HealthReminderFeature.cs
HealthReminderSettings.cs
HealthReminderType.cs
ReminderPopupView.cs
PomodoroPanelView.cs
```

## 功能要求

公共提醒：

- Feature 只发布 `ReminderEvent`。
- Core 决定宠物是否进入 Notify。
- UI 决定气泡和弹窗表现。

番茄钟：

- 支持开始、暂停、继续、结束。
- 支持专注时长、休息时长。
- 专注结束触发提醒。

健康提醒：

- 支持喝水提醒和站立提醒。
- 支持独立开关。
- 支持延后。

## 不要修改

- 不直接调用宠物动画播放器。
- 不修改窗口平台代码。
- 不把待办数据混进 settings。

## 验收标准

- 任意提醒可触发统一气泡。
- 番茄钟计时状态正确。
- 喝水/站立提醒可独立开关和延后。

## 交付说明

说明提醒事件结构、计时逻辑、设置项、测试方式。

