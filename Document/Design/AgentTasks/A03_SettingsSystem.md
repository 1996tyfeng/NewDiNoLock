# A03 设置系统任务

## 任务目标

实现用户设置模型、本地路径管理、设置加载保存、默认值和损坏回退。

## 推荐模型

GPT-5.5 中。

## 必读文档

- `../DesktopPet_FunctionalDesign.md` 中 6.7、9.1
- `../Project_CodeStructure_Guidelines.md` 中 System、配置与数据规范
- `../WorkBreakdown_Priority_ModelGuide.md` 中 T0.4

## 负责范围

主要目录：

```text
Assets/NewDiNoLock/Scripts/System/
Assets/NewDiNoLock/Tests/EditMode/
```

建议文件：

```text
AppSettings.cs
WindowSettings.cs
PetSettings.cs
SystemSettings.cs
FeatureSettings.cs
SettingsService.cs
LocalStorageService.cs
AppPaths.cs
```

## 功能要求

- 首次运行生成默认设置。
- 修改设置后可保存到本地。
- 设置文件损坏时回退默认设置，并记录 Warning。
- 设置变化时发布 `SettingsChanged` 事件。
- 本地路径由 `AppPaths` 统一提供。

## 默认设置至少包含

```text
alwaysOnTop = true
showAboveFullscreen = false
autoHideInFullscreen = true
autoWalkEnabled = true
keepAwakeEnabled = false
currentSkinId = dino_default
restorePositionOnStart = true
volume = 0.5
interactionBubbleEnabled = true
```

## 不要修改

- 不直接实现 UI。
- 不调用 Windows API。
- 不把用户运行时数据写入 `Assets`。

## 验收标准

- 有 EditMode 测试覆盖默认值、保存加载、损坏回退。
- 其他模块可以按段读取设置。
- 没有散落的硬编码本地数据路径。

## 交付说明

说明设置文件路径、数据结构、默认值和测试结果。

