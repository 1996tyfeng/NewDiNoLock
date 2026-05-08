# D01 皮肤系统任务

## 任务目标

实现皮肤数据模型、皮肤加载、帧动画适配、Spine 适配和皮肤选择 UI。

## 推荐模型

GPT-5.5 中。Spine 运行库接入建议使用 GPT-5.5 高。

## 必读文档

- `../DesktopPet_FunctionalDesign.md` 中 6.6
- `../Project_CodeStructure_Guidelines.md` 中 Rendering、资源规范
- `../WorkBreakdown_Priority_ModelGuide.md` 中 T3.1、T3.2、T3.3、T3.4

## 负责范围

主要目录：

```text
Assets/NewDiNoLock/Scripts/Rendering/
Assets/NewDiNoLock/Scripts/UI/
Assets/NewDiNoLock/Art/Skins/
Assets/NewDiNoLock/ScriptableObjects/Skins/
Assets/ThirdParty/Spine/
```

建议文件：

```text
SkinDefinition.cs
SkinType.cs
SkinManager.cs
FrameAnimationPlayer.cs
SpineAnimationPlayer.cs
SkinSelectView.cs
```

## 功能要求

皮肤数据：

- 每个皮肤有 id、displayName、type、scale、pivot、hitArea、animations。
- 支持默认皮肤回退。

帧动画：

- 实现统一动作播放。
- 支持循环和一次性动作。

Spine：

- 通过同一个 `IPetAnimationPlayer` 适配。
- 业务层不直接引用 Spine API。

皮肤选择 UI：

- 显示皮肤名称、图标、预览。
- 切换后保存 `currentSkinId`。

## 不要修改

- 不改变 Core 状态机规则。
- 不把皮肤资源结构写死在业务逻辑里。
- 不要求 Spine 包缺失时项目完全不可运行。

## 验收标准

- 能列出可用皮肤。
- 能切换至少一个默认或占位皮肤。
- 皮肤缺失动作能回退。
- 重启后恢复上次皮肤。

## 交付说明

说明皮肤目录格式、配置字段、动画映射、Spine 依赖情况。

