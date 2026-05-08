# NewDiNoLock 工作拆分、优先级与模型使用建议

## 1. 文档信息

| 项目 | 内容 |
| --- | --- |
| 文档类型 | 工作拆分与子 agent 派单指南 |
| 当前版本 | v0.1 |
| 创建日期 | 2026-05-08 |
| 适用项目 | NewDiNoLock |
| 参考文档 | DesktopPet_FunctionalDesign.md, Project_CodeStructure_Guidelines.md |

## 2. 使用方式

本文档用于把 NewDiNoLock 桌面宠物项目拆成可并行、可验收的任务包。给子 agent 下发任务前，建议同时附带以下文档：

- `DesktopPet_FunctionalDesign.md`
- `Project_CodeStructure_Guidelines.md`
- 本文档中的对应任务卡片

子 agent 开发前必须确认：

- 自己负责的目录和文件范围。
- 是否会修改高风险共享文件。
- 是否需要新增公共接口或设置项。
- 开发完成后如何验证。

## 3. 模型级别建议

| 模型级别 | 推荐用途 | 适合任务 |
| --- | --- | --- |
| GPT-5.5 高 | 高复杂度架构、跨模块核心能力、平台兼容风险高的任务 | 透明窗口、置顶、全屏检测、防锁屏、核心状态机、依赖重构 |
| GPT-5.5 中 | 中高复杂度功能实现，需要较强工程判断 | 皮肤系统、拖动交互、设置系统、提醒调度、番茄钟 |
| GPT-5.5 低 | 逻辑清晰、范围较小，但仍希望质量稳定 | UI 面板、菜单、气泡、配置类、简单 Feature |
| GPT-5.4 高 | 可替代 GPT-5.5 中，用于常规工程实现和测试补全 | 设置 UI、健康提醒、待办 MVP、日志与测试 |
| GPT-5.4 中 | 常规代码补齐、资源目录整理、低风险模块 | 数据模型、View 绑定、Prefab 规范落地 |
| GPT-5.4-Mini | 简单机械任务、文档整理、小修小补 | 命名整理、注释、简单测试用例、Markdown 更新 |
| gpt-5.3-codex | 代码库内连续开发、修 bug、跑测试、集成改动 | 多文件实现、测试修复、Unity C# 代码维护 |

推荐原则：

- 涉及 Windows 原生 API、Unity 窗口句柄、全屏兼容、系统状态恢复：使用 GPT-5.5 高。
- 涉及核心状态机、跨模块事件、设置迁移：使用 GPT-5.5 高或 GPT-5.5 中。
- 单一 Feature 且接口已稳定：使用 GPT-5.5 中、GPT-5.5 低或 GPT-5.4 高。
- 单纯 UI、文档、配置、测试补充：使用 GPT-5.4 中或 GPT-5.4-Mini。

## 4. 总体优先级

| 阶段 | 目标 | 优先级 | 是否可并行 |
| --- | --- | --- | --- |
| Phase 0 | 工程骨架与规范落地 | P0 | 部分可并行 |
| Phase 1 | 桌面宠物基础可运行 | P0 | 谨慎并行 |
| Phase 2 | 交互、设置、隐藏、托盘完善 | P0 | 可并行 |
| Phase 3 | 皮肤系统与动画适配 | P0/P1 | 可并行 |
| Phase 4 | 防锁屏与全屏兼容 | P1 | 可并行 |
| Phase 5 | 办公提醒 MVP | P1 | 可并行 |
| Phase 6 | 待办与通讯扩展 | P2/P3 | 可并行 |
| Phase 7 | 测试、打包、质量收口 | P0/P1 | 贯穿全程 |

## 5. 依赖关系总览

```text
工程目录与 Bootstrap
  ├─ Infrastructure/EventBus/Logger/TimeProvider
  ├─ System/SettingsService/AppSettings/AppPaths
  ├─ Rendering/IPetAnimationPlayer/占位动画
  ├─ Core/PetStateMachine/PetBehavior
  │   ├─ 拖动与点击互动
  │   ├─ 自动行走
  │   └─ 提醒状态接入
  ├─ Window/透明窗口/置顶/鼠标穿透
  │   ├─ 隐藏显示
  │   ├─ 托盘菜单
  │   └─ 全屏检测
  ├─ UI/气泡/设置面板/皮肤面板
  ├─ Features/番茄钟/健康提醒/待办
  └─ 测试与打包
```

关键路径：

1. 先建立工程骨架、基础服务、设置模型。
2. 再建立宠物根对象、状态机、动画接口。
3. 然后实现透明窗口、拖动、自动行走。
4. 再补 UI、托盘、皮肤、防锁屏。
5. 最后接入番茄钟、健康提醒、待办等办公功能。

## 6. Phase 0：工程骨架与基础设施

### T0.1 建立 Assets/NewDiNoLock 工程目录

| 项目 | 内容 |
| --- | --- |
| 优先级 | P0 |
| 推荐模型 | GPT-5.4 中 或 GPT-5.4-Mini |
| 负责目录 | `Assets/NewDiNoLock/` |
| 依赖 | 无 |
| 可并行 | 否，建议最先完成 |

任务内容：

- 按 `Project_CodeStructure_Guidelines.md` 建立推荐目录。
- 创建 Main 场景目录、Scripts 子目录、Art/Prefabs/Tests 等目录。
- 保留 Unity 原有 Settings，不做无关移动。

验收标准：

- Unity 工程能正常打开。
- 目录结构符合规范。
- 未删除 Unity 自动生成的 `.meta` 文件。

### T0.2 Bootstrap 与生命周期入口

| 项目 | 内容 |
| --- | --- |
| 优先级 | P0 |
| 推荐模型 | GPT-5.5 中 或 gpt-5.3-codex |
| 负责目录 | `Scripts/Bootstrap/`, `Prefabs/System/` |
| 依赖 | T0.1 |
| 可并行 | 可与 T0.3 并行 |

任务内容：

- 创建 `AppBootstrapper`。
- 创建 `AppLifecycle`。
- 创建基础 `ServiceRegistry`。
- 准备 `AppRoot` 预制体或场景根对象。

验收标准：

- Main 场景启动后能初始化 AppBootstrapper。
- 应用退出时有统一清理入口。
- Bootstrap 不包含具体业务逻辑。

### T0.3 Infrastructure 基础设施

| 项目 | 内容 |
| --- | --- |
| 优先级 | P0 |
| 推荐模型 | GPT-5.5 中 |
| 负责目录 | `Scripts/Infrastructure/` |
| 依赖 | T0.1 |
| 可并行 | 可与 T0.2 并行 |

任务内容：

- 实现轻量 `EventBus`。
- 实现 `Logger` 封装。
- 实现 `TimeProvider`。
- 提供 `DisposableGroup` 或统一释放工具。

验收标准：

- 可发布和订阅事件。
- 订阅可释放，避免重复回调。
- 纯 C# 逻辑可写 EditMode 测试。

### T0.4 设置与本地路径基础

| 项目 | 内容 |
| --- | --- |
| 优先级 | P0 |
| 推荐模型 | GPT-5.5 中 |
| 负责目录 | `Scripts/System/` |
| 依赖 | T0.3 |
| 可并行 | 可与 Rendering/Core 初稿并行 |

任务内容：

- 定义 `AppSettings`、`WindowSettings`、`PetSettings`、`SystemSettings`、`FeatureSettings`。
- 实现 `AppPaths`。
- 实现 `SettingsService` 的加载、保存、默认值、损坏回退。
- 发布 `SettingsChanged` 事件。

验收标准：

- 首次运行生成默认设置。
- 修改设置可持久化。
- 设置文件损坏时可回退默认值。

## 7. Phase 1：桌面宠物基础可运行

### T1.1 宠物状态机

| 项目 | 内容 |
| --- | --- |
| 优先级 | P0 |
| 推荐模型 | GPT-5.5 高 |
| 负责目录 | `Scripts/Core/` |
| 依赖 | T0.3 |
| 可并行 | 可与 T1.2 并行，但需约定接口 |

任务内容：

- 定义 `PetState`。
- 实现 `PetStateMachine`。
- 实现状态优先级：Hidden、Dragged、Notify、Interact、Walk、Idle、Sleep。
- 提供状态变更事件。

验收标准：

- 状态切换符合功能设计文档。
- 拖动状态不可被普通互动打断。
- 有 EditMode 测试覆盖优先级。

### T1.2 动画播放接口与占位动画

| 项目 | 内容 |
| --- | --- |
| 优先级 | P0 |
| 推荐模型 | GPT-5.5 中 或 gpt-5.3-codex |
| 负责目录 | `Scripts/Rendering/`, `Prefabs/Pet/` |
| 依赖 | T0.1 |
| 可并行 | 可与 T1.1 并行 |

任务内容：

- 定义 `IPetAnimationPlayer`。
- 定义 `AnimationName` 常量。
- 实现一个基础占位动画播放器。
- 准备可显示的宠物根对象。

验收标准：

- Core 可以通过接口播放 Idle/Walk/Lift/Click。
- 缺失动作回退到 Idle。
- 不依赖具体 Spine 包即可运行基础场景。

### T1.3 宠物行为调度

| 项目 | 内容 |
| --- | --- |
| 优先级 | P0 |
| 推荐模型 | GPT-5.5 高 |
| 负责目录 | `Scripts/Core/` |
| 依赖 | T1.1, T1.2 |
| 可并行 | 否，属于核心集成任务 |

任务内容：

- 实现 `PetBehaviorController`。
- 根据状态切换触发对应动画。
- 接入基础 Idle。
- 为 Walk、Dragged、Interact、Notify 预留入口。

验收标准：

- 启动后宠物进入 Idle。
- 状态变化会驱动动画变化。
- 行为调度不直接依赖具体皮肤实现。

## 8. Phase 2：窗口、交互与显示控制

### T2.1 Windows 透明无边框窗口

| 项目 | 内容 |
| --- | --- |
| 优先级 | P0 |
| 推荐模型 | GPT-5.5 高 |
| 负责目录 | `Scripts/Window/` |
| 依赖 | T0.2, T0.4 |
| 可并行 | 可与 T2.3 并行 |

任务内容：

- 获取 Unity 窗口句柄。
- 实现无边框、透明背景、基础窗口样式设置。
- 提供 `IDesktopWindowService`。

验收标准：

- 构建后窗口无系统边框。
- 背景透明，仅显示宠物内容。
- 非 Windows 平台有安全空实现。

### T2.2 置顶与全屏上方显示开关

| 项目 | 内容 |
| --- | --- |
| 优先级 | P0 |
| 推荐模型 | GPT-5.5 高 |
| 负责目录 | `Scripts/Window/`, `Scripts/System/` |
| 依赖 | T2.1, T0.4 |
| 可并行 | 可与 T2.4 并行 |

任务内容：

- 实现普通置顶。
- 实现 `showAboveFullscreen` 设置接入。
- 将全屏置顶作为兼容性选项。

验收标准：

- `alwaysOnTop` 开关生效。
- `showAboveFullscreen` 配置可保存。
- 关闭全屏上方显示时不会强行覆盖全屏应用。

### T2.3 拖动与点击互动

| 项目 | 内容 |
| --- | --- |
| 优先级 | P0 |
| 推荐模型 | GPT-5.5 中 |
| 负责目录 | `Scripts/Core/`, `Scripts/UI/` |
| 依赖 | T1.3 |
| 可并行 | 可与 T2.1 并行 |

任务内容：

- 实现 `PetInteractionController`。
- 支持左键点击、拖动、松开。
- 拖动时进入 Dragged 状态并播放 Lift。
- 点击时进入 Interact 状态并播放 Click。

验收标准：

- 鼠标拖动可改变宠物位置。
- 拖动中不会触发自动行走。
- 松开后回到 Idle 或后续调度状态。

### T2.4 自动行走

| 项目 | 内容 |
| --- | --- |
| 优先级 | P0 |
| 推荐模型 | GPT-5.5 中 |
| 负责目录 | `Scripts/Core/`, `Scripts/System/` |
| 依赖 | T1.3, T0.4 |
| 可并行 | 可与 T2.3 并行，但需避免同时改 PetStateMachine |

任务内容：

- 实现 `PetMovementController`。
- 实现屏幕边界服务 `PetBoundsService`。
- 根据随机间隔选择目标点。
- 接入 `autoWalkEnabled` 设置。

验收标准：

- 开启自动行走后宠物会在屏幕范围内移动。
- 关闭自动行走后宠物不主动移动。
- 目标点不会让宠物主体离开屏幕。

### T2.5 隐藏/显示与鼠标穿透

| 项目 | 内容 |
| --- | --- |
| 优先级 | P0 |
| 推荐模型 | GPT-5.5 高 |
| 负责目录 | `Scripts/Window/`, `Scripts/Core/`, `Scripts/UI/` |
| 依赖 | T2.1, T2.3 |
| 可并行 | 可与 T2.6 并行 |

任务内容：

- 实现窗口隐藏/显示。
- 实现宠物区域外鼠标穿透策略。
- Hidden 状态接入。

验收标准：

- 隐藏后宠物不可见。
- 恢复后位置和状态合理。
- 宠物区域可交互，透明区域尽量不阻挡桌面。

### T2.6 右键菜单与托盘菜单

| 项目 | 内容 |
| --- | --- |
| 优先级 | P0 |
| 推荐模型 | GPT-5.5 中 或 GPT-5.4 高 |
| 负责目录 | `Scripts/UI/`, `Scripts/Window/`, `Art/Icons/` |
| 依赖 | T0.4, T2.1 |
| 可并行 | 可与 T2.5 并行 |

任务内容：

- 实现右键菜单：隐藏、自动走动、置顶、全屏上方显示、设置、退出。
- 实现托盘菜单：显示/隐藏、设置、退出。
- 菜单状态与设置同步。

验收标准：

- 右键菜单可用。
- 托盘可恢复隐藏宠物。
- 菜单开关修改后设置保存。

## 9. Phase 3：皮肤系统

### T3.1 皮肤数据模型与加载

| 项目 | 内容 |
| --- | --- |
| 优先级 | P0 |
| 推荐模型 | GPT-5.5 中 |
| 负责目录 | `Scripts/Rendering/`, `Art/Skins/` |
| 依赖 | T1.2, T0.4 |
| 可并行 | 可与 T3.2 并行 |

任务内容：

- 定义 `SkinDefinition`、`SkinType`。
- 解析 `skin.json` 或 ScriptableObject 配置。
- 实现 `SkinManager`。
- 支持默认皮肤回退。

验收标准：

- 能列出可用皮肤。
- 能加载默认皮肤。
- 配置错误时有日志和回退。

### T3.2 帧动画皮肤适配

| 项目 | 内容 |
| --- | --- |
| 优先级 | P0 |
| 推荐模型 | GPT-5.5 中 或 GPT-5.4 高 |
| 负责目录 | `Scripts/Rendering/`, `Art/Skins/` |
| 依赖 | T1.2 |
| 可并行 | 可与 T3.1 并行 |

任务内容：

- 实现 `FrameAnimationPlayer`。
- 支持 Idle/Walk/Lift/Click。
- 支持动作循环与一次性播放。

验收标准：

- 默认帧动画皮肤可播放。
- 动作缺失时回退。
- 不影响 Spine 后续接入。

### T3.3 Spine 皮肤适配

| 项目 | 内容 |
| --- | --- |
| 优先级 | P1 |
| 推荐模型 | GPT-5.5 高 |
| 负责目录 | `Scripts/Rendering/`, `ThirdParty/Spine/`, `Art/Skins/` |
| 依赖 | T3.1 |
| 可并行 | 可与 T3.4 并行 |

任务内容：

- 接入 Spine Unity Runtime。
- 实现 `SpineAnimationPlayer`。
- 支持统一动作映射。

验收标准：

- Spine 皮肤能播放统一动作。
- 业务层无需关心 Spine 细节。
- Spine 包缺失时项目有清晰错误提示。

### T3.4 皮肤选择 UI

| 项目 | 内容 |
| --- | --- |
| 优先级 | P1 |
| 推荐模型 | GPT-5.4 高 或 GPT-5.5 低 |
| 负责目录 | `Scripts/UI/`, `Scripts/Rendering/` |
| 依赖 | T3.1 |
| 可并行 | 可与 T3.3 并行 |

任务内容：

- 实现 `SkinSelectView`。
- 显示皮肤名称、图标、预览。
- 切换皮肤并保存 `currentSkinId`。

验收标准：

- UI 可切换皮肤。
- 重启后恢复上次皮肤。
- 皮肤加载失败时显示合理提示。

## 10. Phase 4：系统能力与兼容

### T4.1 防锁屏

| 项目 | 内容 |
| --- | --- |
| 优先级 | P1 |
| 推荐模型 | GPT-5.5 高 |
| 负责目录 | `Scripts/System/`, `Scripts/UI/`, `Scripts/Bootstrap/` |
| 依赖 | T0.4 |
| 可并行 | 可与 T4.2 并行 |

任务内容：

- 实现 `KeepAwakeService`。
- 接入 `keepAwakeEnabled` 设置。
- 退出应用时释放防锁屏状态。
- 在设置面板/菜单中提供开关。

验收标准：

- 开启后系统不会自动睡眠或锁屏。
- 关闭或退出后释放状态。
- Windows API 调用失败时有日志，不影响应用主流程。

### T4.2 全屏检测与自动隐藏

| 项目 | 内容 |
| --- | --- |
| 优先级 | P1 |
| 推荐模型 | GPT-5.5 高 |
| 负责目录 | `Scripts/Window/`, `Scripts/System/`, `Scripts/Core/` |
| 依赖 | T2.1, T0.4 |
| 可并行 | 可与 T4.1 并行 |

任务内容：

- 实现 `FullscreenDetectService`。
- 接入 `autoHideInFullscreen`。
- 检测到全屏应用后根据设置隐藏或降低置顶策略。

验收标准：

- 全屏应用出现时行为符合设置。
- 退出全屏后宠物可恢复。
- 不频繁闪烁或反复切换窗口层级。

### T4.3 全局快捷键

| 项目 | 内容 |
| --- | --- |
| 优先级 | P2 |
| 推荐模型 | GPT-5.4 高 |
| 负责目录 | `Scripts/System/`, `Scripts/UI/` |
| 依赖 | T2.5 |
| 可并行 | 可与办公功能并行 |

任务内容：

- 实现 `HotkeyService`。
- 支持显示/隐藏快捷键。
- 后续预留打开设置、开始番茄钟等快捷键。

验收标准：

- 快捷键可触发隐藏/显示。
- 快捷键冲突时有日志或提示。

## 11. Phase 5：办公提醒 MVP

### T5.1 提醒公共模型与调度基础

| 项目 | 内容 |
| --- | --- |
| 优先级 | P1 |
| 推荐模型 | GPT-5.5 中 |
| 负责目录 | `Scripts/Features/Common/`, `Scripts/Core/`, `Scripts/UI/` |
| 依赖 | T0.3, T1.3 |
| 可并行 | 否，先于具体提醒功能 |

任务内容：

- 定义 `IReminderFeature`。
- 定义 `ReminderEvent`、`ReminderPriority`。
- Core 接入 Notify 状态。
- UI 接入通用提醒气泡。

验收标准：

- 任意 Feature 可发布 ReminderEvent。
- 宠物可进入 Notify 状态。
- 提醒可以确认、忽略或超时关闭。

### T5.2 番茄钟

| 项目 | 内容 |
| --- | --- |
| 优先级 | P1 |
| 推荐模型 | GPT-5.5 中 或 GPT-5.4 高 |
| 负责目录 | `Scripts/Features/Pomodoro/`, `Scripts/UI/` |
| 依赖 | T5.1, T0.4 |
| 可并行 | 可与 T5.3 并行 |

任务内容：

- 实现专注、暂停、继续、休息、结束。
- 支持自定义专注和休息时长。
- 专注结束时触发 ReminderEvent。

验收标准：

- 计时准确。
- 暂停/继续状态正确。
- 提醒能驱动宠物气泡和 Notify 动画。

### T5.3 喝水/站立提醒

| 项目 | 内容 |
| --- | --- |
| 优先级 | P1 |
| 推荐模型 | GPT-5.4 高 或 GPT-5.5 低 |
| 负责目录 | `Scripts/Features/HealthReminder/`, `Scripts/UI/` |
| 依赖 | T5.1, T0.4 |
| 可并行 | 可与 T5.2 并行 |

任务内容：

- 实现喝水提醒。
- 实现站立提醒。
- 支持提醒间隔和延后。

验收标准：

- 两类提醒可独立开关。
- 延后后按新时间提醒。
- 勿扰/隐藏时行为符合设置。

## 12. Phase 6：待办与通讯扩展

### T6.1 待办事项 MVP

| 项目 | 内容 |
| --- | --- |
| 优先级 | P2 |
| 推荐模型 | GPT-5.5 中 或 GPT-5.4 高 |
| 负责目录 | `Scripts/Features/Todo/`, `Scripts/UI/`, `Scripts/System/` |
| 依赖 | T5.1 |
| 可并行 | 可与 T6.2 需求调研并行 |

任务内容：

- 创建待办数据模型。
- 创建、完成、删除待办。
- 设置提醒时间。
- 到期触发 ReminderEvent。

验收标准：

- 待办数据独立保存，不写入 settings。
- 到期提醒可触发宠物提示。
- 支持完成和延后。

### T6.2 通讯工具消息提示调研与接口预留

| 项目 | 内容 |
| --- | --- |
| 优先级 | P3 |
| 推荐模型 | GPT-5.5 高 |
| 负责目录 | `Scripts/Features/MessageNotifier/`, `Document/Design/` |
| 依赖 | T5.1 |
| 可并行 | 可独立调研 |

任务内容：

- 设计 `MessageNotifierFeature` 接口。
- 明确消息来源、隐私模式、提示格式。
- 先做本地模拟消息，不直接接真实通讯工具。

验收标准：

- 有统一 `MessageNotification` 数据模型。
- 可模拟触发消息提醒。
- 隐私模式默认不展示敏感正文。

## 13. Phase 7：测试、构建与质量收口

### T7.1 核心 EditMode 测试

| 项目 | 内容 |
| --- | --- |
| 优先级 | P0 |
| 推荐模型 | GPT-5.4 高 或 gpt-5.3-codex |
| 负责目录 | `Tests/EditMode/` |
| 依赖 | 对应功能模块 |
| 可并行 | 可贯穿全程 |

任务内容：

- PetStateMachine 测试。
- SettingsService 测试。
- Reminder 调度测试。
- Skin 动作回退测试。

验收标准：

- 核心逻辑有自动化测试。
- 测试可在 Unity Test Runner 中运行。

### T7.2 PlayMode 和手动验证清单

| 项目 | 内容 |
| --- | --- |
| 优先级 | P1 |
| 推荐模型 | GPT-5.4 高 |
| 负责目录 | `Tests/PlayMode/`, `Document/Design/` |
| 依赖 | MVP 功能 |
| 可并行 | 可在 Phase 2 后开始 |

任务内容：

- 宠物显示、拖动、隐藏、气泡、皮肤切换的 PlayMode 测试。
- 维护手动验证清单。

验收标准：

- 关键用户行为有测试或清单覆盖。
- 每次发版前可按清单验证。

### T7.3 Windows 构建与发布配置

| 项目 | 内容 |
| --- | --- |
| 优先级 | P1 |
| 推荐模型 | GPT-5.5 中 或 gpt-5.3-codex |
| 负责目录 | `ProjectSettings/`, `Document/Design/` |
| 依赖 | Phase 2 基本完成 |
| 可并行 | 后期进行 |

任务内容：

- 配置 Windows 构建参数。
- 确认透明窗口构建后表现。
- 整理发布包结构。
- 准备版本号和日志位置。

验收标准：

- Windows 构建可运行。
- 退出流程正常。
- 不依赖 Unity Editor 才能使用核心功能。

## 14. 推荐并行派单方案

### 第一批：基础骨架

建议同时派出 3 个子 agent：

| Agent | 任务 | 推荐模型 |
| --- | --- | --- |
| A | T0.1 工程目录 | GPT-5.4-Mini |
| B | T0.3 Infrastructure | GPT-5.5 中 |
| C | T0.4 设置系统 | GPT-5.5 中 |

注意：

- T0.4 依赖 T0.3 的事件能力，如果并行开发，先约定 EventBus 最小接口。
- T0.1 完成前其他 agent 不应移动资源。

### 第二批：宠物核心

建议同时派出 3 个子 agent：

| Agent | 任务 | 推荐模型 |
| --- | --- | --- |
| A | T1.1 宠物状态机 | GPT-5.5 高 |
| B | T1.2 动画接口与占位动画 | GPT-5.5 中 |
| C | T2.1 透明窗口预研/实现 | GPT-5.5 高 |

注意：

- T1.1 和 T1.2 需要先约定 `IPetAnimationPlayer` 和 `AnimationName`。
- T2.1 涉及平台 API，建议独立分支或独立提交。

### 第三批：MVP 交互闭环

建议同时派出 4 个子 agent：

| Agent | 任务 | 推荐模型 |
| --- | --- | --- |
| A | T1.3 行为调度集成 | GPT-5.5 高 |
| B | T2.3 拖动与点击互动 | GPT-5.5 中 |
| C | T2.4 自动行走 | GPT-5.5 中 |
| D | T2.6 右键菜单与托盘菜单 | GPT-5.4 高 |

注意：

- A 是集成核心，B/C 不应同时大改 PetStateMachine。
- D 可先用设置服务和窗口服务接口做 UI 接入。

### 第四批：表现和系统能力

建议同时派出 4 个子 agent：

| Agent | 任务 | 推荐模型 |
| --- | --- | --- |
| A | T3.1 皮肤数据模型 | GPT-5.5 中 |
| B | T3.2 帧动画皮肤 | GPT-5.4 高 |
| C | T4.1 防锁屏 | GPT-5.5 高 |
| D | T4.2 全屏检测 | GPT-5.5 高 |

注意：

- 防锁屏和全屏检测都涉及平台 API，避免共享同一个平台工具文件时产生冲突。
- Spine 接入可放到帧动画稳定之后。

### 第五批：办公助手能力

建议同时派出 3 个子 agent：

| Agent | 任务 | 推荐模型 |
| --- | --- | --- |
| A | T5.1 提醒公共模型 | GPT-5.5 中 |
| B | T5.2 番茄钟 | GPT-5.4 高 |
| C | T5.3 健康提醒 | GPT-5.4 高 |

注意：

- T5.1 是前置任务，B/C 可先开发纯逻辑，等公共 ReminderEvent 稳定后接入。

## 15. 子 agent 派单模板

```text
你负责 NewDiNoLock 的【任务编号 + 任务名称】。

请先阅读：
- /Users/littlefive/UnityProject/NewDiNoLock/Document/Design/DesktopPet_FunctionalDesign.md
- /Users/littlefive/UnityProject/NewDiNoLock/Document/Design/Project_CodeStructure_Guidelines.md
- /Users/littlefive/UnityProject/NewDiNoLock/Document/Design/WorkBreakdown_Priority_ModelGuide.md 中对应任务卡片

负责范围：
- 主要目录：
- 可修改文件：
- 尽量不要修改：

交付要求：
- 按项目规范实现功能。
- 如需修改共享接口，先说明原因和影响范围。
- 增加必要测试或手动验证说明。
- 最终回复列出修改文件、验证结果、残余风险。
```

## 16. 近期最推荐执行顺序

短期建议先不要上来就做番茄钟或通讯接入，先把宠物“活起来”。推荐顺序：

1. T0.1 工程目录。
2. T0.3 Infrastructure。
3. T0.4 设置系统。
4. T1.2 动画接口与占位动画。
5. T1.1 宠物状态机。
6. T1.3 行为调度。
7. T2.1 透明窗口。
8. T2.3 拖动与点击互动。
9. T2.4 自动行走。
10. T2.6 右键菜单与托盘菜单。
11. T2.5 隐藏/显示与鼠标穿透。
12. T3.1/T3.2 皮肤基础。
13. T4.1 防锁屏。
14. T5.1/T5.2/T5.3 办公提醒。

完成前 10 项后，就能得到一个基本可体验的 MVP 雏形。
