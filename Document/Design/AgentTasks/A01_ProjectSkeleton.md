# A01 工程目录结构任务

## 任务目标

建立 NewDiNoLock 的标准 Unity 工程目录，为后续所有功能开发提供稳定落点。

## 推荐模型

GPT-5.4-Mini 或 GPT-5.4 中。

## 必读文档

- `../Project_CodeStructure_Guidelines.md`
- `../WorkBreakdown_Priority_ModelGuide.md` 中 T0.1

## 负责范围

主要目录：

```text
/Users/littlefive/UnityProject/NewDiNoLock/NewDiNoLock/Assets/NewDiNoLock/
```

建议创建：

```text
Art/Skins
Art/Icons
Art/UI
Art/Audio
Prefabs/Pet
Prefabs/UI
Prefabs/System
Scenes
ScriptableObjects/Defaults
ScriptableObjects/Skins
ScriptableObjects/Reminders
Scripts/Bootstrap
Scripts/Core
Scripts/Rendering
Scripts/Window
Scripts/System
Scripts/Features/Common
Scripts/Features/Pomodoro
Scripts/Features/HealthReminder
Scripts/Features/Todo
Scripts/Features/MessageNotifier
Scripts/UI
Scripts/Infrastructure
Scripts/Utilities
Settings/RenderPipeline
Tests/EditMode
Tests/PlayMode
```

## 不要修改

- 不移动现有 `Assets/Settings` 和 `Assets/Scenes`。
- 不删除任何 `.meta` 文件。
- 不新增业务脚本，除非只是占位说明文件。

## 验收标准

- 目录结构符合规范。
- Unity 工程仍可正常打开。
- 后续任务能按目录落位。

## 交付说明

列出创建的目录，并说明是否有移动或重命名现有文件。

