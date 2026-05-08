# E02 待办与消息提示扩展任务

## 任务目标

实现待办事项 MVP，并为通讯工具新消息提示预留独立扩展接口。

## 推荐模型

GPT-5.5 中或 GPT-5.4 高。通讯接入调研可用 GPT-5.5 高。

## 必读文档

- `../DesktopPet_FunctionalDesign.md` 中 7.3、7.4
- `../Project_CodeStructure_Guidelines.md` 中 Features
- `../WorkBreakdown_Priority_ModelGuide.md` 中 T6.1、T6.2

## 负责范围

主要目录：

```text
Assets/NewDiNoLock/Scripts/Features/Todo/
Assets/NewDiNoLock/Scripts/Features/MessageNotifier/
Assets/NewDiNoLock/Scripts/Features/Common/
Assets/NewDiNoLock/Scripts/UI/
Assets/NewDiNoLock/Scripts/System/
```

建议文件：

```text
TodoFeature.cs
TodoItem.cs
TodoRepository.cs
MessageNotifierFeature.cs
MessageNotification.cs
```

## 功能要求

待办 MVP：

- 创建待办。
- 完成待办。
- 删除待办。
- 设置提醒时间。
- 到期触发 `ReminderEvent`。
- 待办数据独立保存，不写入 settings。

消息提示接口：

- 先支持模拟消息。
- 定义来源、标题、摘要、隐私模式字段。
- 隐私模式默认不展示敏感正文。

## 不要修改

- 不直接接入真实通讯工具账号。
- 不写通讯工具敏感内容日志。
- 不修改核心状态机，除非提醒公共模型缺口明确。

## 验收标准

- 待办到期可触发宠物提醒。
- 待办数据重启后保留。
- 模拟消息可走统一提醒链路。
- 隐私模式字段存在并默认保守。

## 交付说明

说明数据文件路径、待办模型、消息模型和隐私策略。

