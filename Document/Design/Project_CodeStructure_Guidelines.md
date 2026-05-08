# NewDiNoLock 项目代码结构与开发规范

## 1. 文档信息

| 项目 | 内容 |
| --- | --- |
| 文档类型 | 项目代码结构与开发规范 |
| 当前版本 | v0.1 |
| 创建日期 | 2026-05-08 |
| 适用项目 | NewDiNoLock |
| Unity 版本 | 2022.3.62f3 |
| 参考文档 | DesktopPet_FunctionalDesign.md |

## 2. 文档目标

本文档用于统一 NewDiNoLock 桌面宠物项目的代码组织、模块边界、命名规范、资源规范和协作方式。后续不同子 agent 或开发者在开发单个功能时，应优先查询本文档，确认文件应放置的位置、依赖方向、接口边界和测试要求。

核心目标：

- 保持功能模块之间低耦合。
- 避免多个功能同时修改同一批核心文件。
- 让桌面窗口、宠物行为、皮肤、提醒、系统能力等模块各自清晰。
- 方便后续功能从 MVP 平滑扩展为办公助手。

## 3. 当前工程位置

仓库外层目录：

```text
/Users/littlefive/UnityProject/NewDiNoLock
```

Unity 工程目录：

```text
/Users/littlefive/UnityProject/NewDiNoLock/NewDiNoLock
```

文档目录：

```text
/Users/littlefive/UnityProject/NewDiNoLock/Document/Design
```

所有 Unity 运行时代码、场景、预制体、资源默认放在：

```text
/Users/littlefive/UnityProject/NewDiNoLock/NewDiNoLock/Assets
```

## 4. 推荐 Assets 目录结构

后续开发应逐步整理为以下结构：

```text
Assets/
  NewDiNoLock/
    Art/
      Skins/
      Icons/
      UI/
      Audio/
    Prefabs/
      Pet/
      UI/
      System/
    Scenes/
      Main.unity
    ScriptableObjects/
      Defaults/
      Skins/
      Reminders/
    Scripts/
      Bootstrap/
      Core/
      Rendering/
      Window/
      System/
      Features/
      UI/
      Infrastructure/
      Utilities/
    Settings/
      RenderPipeline/
    Tests/
      EditMode/
      PlayMode/
  ThirdParty/
    Spine/
```

目录职责：

| 目录 | 职责 |
| --- | --- |
| Art/Skins | 宠物皮肤资源，包括 Spine、帧动画、预览图、配置 |
| Art/Icons | 托盘图标、菜单图标、功能图标 |
| Art/UI | UI 贴图、气泡背景、设置面板素材 |
| Art/Audio | 点击、提醒、状态反馈音效 |
| Prefabs/Pet | 宠物根预制体、皮肤挂载预制体 |
| Prefabs/UI | 气泡、设置面板、提醒面板等 |
| Prefabs/System | AppRoot、服务管理器等系统预制体 |
| Scenes | 项目场景，MVP 建议主场景命名为 Main |
| ScriptableObjects | 默认配置、皮肤配置、提醒配置模板 |
| Scripts/Bootstrap | 启动入口、依赖组装、生命周期管理 |
| Scripts/Core | 宠物状态机、行为调度、核心领域模型 |
| Scripts/Rendering | 皮肤加载、Spine/帧动画适配 |
| Scripts/Window | 透明窗口、置顶、全屏检测、托盘 |
| Scripts/System | 设置、防锁屏、热键、本地存储 |
| Scripts/Features | 番茄钟、喝水提醒、待办、消息提示等独立功能 |
| Scripts/UI | 设置面板、气泡、上下文菜单、提示视图 |
| Scripts/Infrastructure | 事件总线、时间服务、日志、平台适配基础设施 |
| Scripts/Utilities | 纯工具类、扩展方法、小型通用 helper |
| ThirdParty | 第三方运行库或插件，不直接写业务逻辑 |

## 5. 脚本模块结构

### 5.1 Bootstrap

负责应用启动与模块组装，不承载业务逻辑。

推荐文件：

```text
Scripts/Bootstrap/
  AppBootstrapper.cs
  AppLifecycle.cs
  ServiceRegistry.cs
```

职责：

- 初始化设置服务。
- 初始化窗口服务。
- 初始化宠物状态机和皮肤系统。
- 注册提醒功能模块。
- 处理应用退出时的清理工作，例如释放防锁屏状态。

约束：

- 不直接写宠物行为细节。
- 不直接写平台 API 调用细节。
- 不直接持有 UI 具体交互逻辑。

### 5.2 Core

负责桌面宠物的核心行为和领域状态。

推荐文件：

```text
Scripts/Core/
  PetState.cs
  PetStateMachine.cs
  PetBehaviorController.cs
  PetInteractionController.cs
  PetMovementController.cs
  PetBoundsService.cs
  PetActionPriority.cs
```

职责：

- 管理 Idle、Walk、Dragged、Interact、Notify、Hidden、Sleep 等状态。
- 处理状态优先级。
- 处理自动行走目标选择与边界限制。
- 将点击、拖拽、提醒等输入转换为宠物状态变化。

约束：

- Core 可以依赖接口，不直接依赖 Windows API、Spine API 或具体 UI 组件。
- Core 不负责读取磁盘配置，只接收已加载的设置数据。
- 状态切换必须集中在 PetStateMachine 或明确的状态调度类中，避免各模块自行抢动画。

### 5.3 Rendering

负责宠物外观、皮肤、动画播放。

推荐文件：

```text
Scripts/Rendering/
  IPetAnimationPlayer.cs
  SkinManager.cs
  SkinDefinition.cs
  SkinType.cs
  SpineAnimationPlayer.cs
  FrameAnimationPlayer.cs
  AnimationName.cs
```

职责：

- 统一 Spine 与帧动画播放接口。
- 根据皮肤配置加载对应资源。
- 处理动作名映射和缺失动作回退。
- 控制角色朝向、缩放、碰撞区域配置。

约束：

- Core 只能通过 IPetAnimationPlayer 触发动作，不直接访问 Spine 或 Animator 组件。
- 皮肤资源缺失时必须回退到默认皮肤或 Idle 动作。
- 动作名使用统一常量，不允许散落字符串。

### 5.4 Window

负责桌面窗口行为和操作系统窗口能力。

推荐文件：

```text
Scripts/Window/
  IDesktopWindowService.cs
  DesktopWindowService.cs
  AlwaysOnTopService.cs
  FullscreenDetectService.cs
  TrayService.cs
  ClickThroughService.cs
  PlatformWindowHandle.cs
```

职责：

- 透明无边框窗口。
- 置顶/取消置顶。
- 全屏应用检测。
- 鼠标穿透控制。
- 托盘菜单。
- 显示/隐藏主窗口。

约束：

- 平台 API 只能在 Window 或 System 的平台适配类中出现。
- 任何全屏置顶逻辑必须保留设置开关。
- 鼠标穿透逻辑不能破坏宠物有效交互区域。

### 5.5 System

负责本地系统能力和持久化。

推荐文件：

```text
Scripts/System/
  SettingsService.cs
  AppSettings.cs
  LocalStorageService.cs
  KeepAwakeService.cs
  HotkeyService.cs
  AppPaths.cs
```

职责：

- 读写用户设置。
- 提供本地数据保存路径。
- 防锁屏。
- 全局快捷键。
- 应用退出清理。

约束：

- 用户设置必须有默认值和损坏回退逻辑。
- KeepAwakeService 必须在应用退出时释放系统状态。
- 任何本地路径应通过 AppPaths 统一生成，避免硬编码散落。

### 5.6 Features

负责可独立开关的办公功能模块。

推荐结构：

```text
Scripts/Features/
  Common/
    IFeatureModule.cs
    IReminderFeature.cs
    ReminderEvent.cs
    ReminderPriority.cs
  Pomodoro/
    PomodoroFeature.cs
    PomodoroSettings.cs
    PomodoroState.cs
  HealthReminder/
    HealthReminderFeature.cs
    HealthReminderSettings.cs
    HealthReminderType.cs
  Todo/
    TodoFeature.cs
    TodoItem.cs
    TodoRepository.cs
  MessageNotifier/
    MessageNotifierFeature.cs
    MessageNotification.cs
```

职责：

- 番茄钟、喝水提醒、站立提醒、待办提醒、消息提示。
- 将提醒统一转换为 ReminderEvent。
- 通过事件通知 Core/UI，不直接控制宠物动画。

约束：

- 单个 Feature 不直接引用另一个 Feature 的具体实现。
- Feature 可以依赖 Common 接口、SettingsService、EventBus。
- Feature 触发提醒后，由 Core 决定宠物状态，由 UI 决定展示样式。

### 5.7 UI

负责 Unity 内部 UI 表现和用户操作入口。

推荐文件：

```text
Scripts/UI/
  BubbleView.cs
  ContextMenuView.cs
  SettingsPanelView.cs
  SkinSelectView.cs
  ReminderPopupView.cs
  PomodoroPanelView.cs
```

职责：

- 显示互动气泡。
- 右键菜单。
- 设置面板。
- 皮肤选择。
- 提醒弹窗。

约束：

- UI 只负责展示和收集用户输入。
- UI 不直接写磁盘，不直接调用平台 API。
- UI 修改设置时通过 SettingsService 或对应 ViewModel/Controller。

### 5.8 Infrastructure

负责跨模块基础设施。

推荐文件：

```text
Scripts/Infrastructure/
  EventBus.cs
  TimeProvider.cs
  Logger.cs
  DisposableGroup.cs
  Platform.cs
```

职责：

- 跨模块事件。
- 时间抽象，便于番茄钟和提醒测试。
- 日志封装。
- 生命周期释放工具。
- 平台判断。

约束：

- Infrastructure 不依赖业务模块。
- 不把业务规则写入通用工具类。

## 6. Assembly Definition 建议

为减少编译时间和控制依赖方向，建议逐步添加 asmdef。

推荐结构：

```text
NewDiNoLock.Infrastructure
NewDiNoLock.System
NewDiNoLock.Window
NewDiNoLock.Rendering
NewDiNoLock.Core
NewDiNoLock.Features
NewDiNoLock.UI
NewDiNoLock.Tests.EditMode
NewDiNoLock.Tests.PlayMode
```

依赖方向建议：

```text
Infrastructure
  ↑
System      Window      Rendering
  ↑           ↑            ↑
  └──────── Core ──────────┘
              ↑
          Features
              ↑
             UI
```

实际依赖规则：

- Infrastructure 不依赖任何业务 asmdef。
- System 可依赖 Infrastructure。
- Window 可依赖 Infrastructure、System 的设置模型或接口。
- Rendering 可依赖 Infrastructure。
- Core 可依赖 Infrastructure、Rendering 接口、System 设置模型。
- Features 可依赖 Infrastructure、System、Core 的事件或接口。
- UI 可依赖 Core、Features、System，但不反向被这些模块依赖。

如果某个依赖导致循环，优先抽接口到 Infrastructure 或模块内的 Abstractions 文件夹。

## 7. 命名空间规范

统一使用 `NewDiNoLock` 作为根命名空间。

示例：

```csharp
namespace NewDiNoLock.Core
{
    public sealed class PetStateMachine
    {
    }
}
```

推荐命名空间：

| 目录 | 命名空间 |
| --- | --- |
| Scripts/Bootstrap | NewDiNoLock.Bootstrap |
| Scripts/Core | NewDiNoLock.Core |
| Scripts/Rendering | NewDiNoLock.Rendering |
| Scripts/Window | NewDiNoLock.Window |
| Scripts/System | NewDiNoLock.System |
| Scripts/Features/Common | NewDiNoLock.Features |
| Scripts/Features/Pomodoro | NewDiNoLock.Features.Pomodoro |
| Scripts/Features/HealthReminder | NewDiNoLock.Features.HealthReminder |
| Scripts/Features/Todo | NewDiNoLock.Features.Todo |
| Scripts/Features/MessageNotifier | NewDiNoLock.Features.MessageNotifier |
| Scripts/UI | NewDiNoLock.UI |
| Scripts/Infrastructure | NewDiNoLock.Infrastructure |
| Scripts/Utilities | NewDiNoLock.Utilities |

## 8. C# 编码规范

### 8.1 基础风格

- 类型名使用 PascalCase。
- public 成员使用 PascalCase。
- private 字段使用 `_camelCase`。
- private serialized 字段使用 `[SerializeField] private`，命名仍使用 `_camelCase`。
- 局部变量和参数使用 camelCase。
- 常量使用 PascalCase，不使用全大写下划线。
- 文件名必须与主类型名一致。

示例：

```csharp
public sealed class PetMovementController : MonoBehaviour
{
    [SerializeField] private float _walkSpeed = 120f;

    private IPetAnimationPlayer _animationPlayer;

    public bool IsWalking { get; private set; }

    public void MoveTo(Vector2 targetPosition)
    {
    }
}
```

### 8.2 MonoBehaviour 使用规则

- 只有需要挂在 GameObject 上、使用 Unity 生命周期或 Inspector 配置的类才继承 MonoBehaviour。
- 纯逻辑类不要继承 MonoBehaviour，例如状态机、计时器、设置模型、仓储类。
- MonoBehaviour 中避免直接 new 大量业务对象，优先由 Bootstrap 或 ServiceRegistry 组装。
- Update 中只保留必要逻辑，复杂逻辑拆到独立方法或纯 C# 类。

### 8.3 事件规则

- 跨模块通信优先使用事件或接口，不直接查找场景对象。
- 不在业务代码中使用 `FindObjectOfType`、`GameObject.Find` 作为常规依赖获取方式。
- 订阅事件必须在 OnDisable、Dispose 或生命周期结束时取消订阅。
- 事件命名使用过去式或明确动作，例如 `PetDragStarted`、`ReminderTriggered`。

### 8.4 字符串与常量

- 动画名、设置 key、事件 id 不允许散落硬编码。
- 动画名统一放在 `AnimationName`。
- 功能模块 id 统一放在对应 Feature 常量或配置中。
- 路径统一由 `AppPaths` 管理。

### 8.5 错误处理

- 皮肤加载、设置读取、平台 API 调用必须处理异常。
- 可恢复错误应记录日志并回退默认值。
- 不允许因为单个扩展功能失败导致宠物核心功能不可用。

## 9. 配置与数据规范

### 9.1 用户设置

设置模型统一由 `AppSettings` 表达，由 `SettingsService` 读写。

建议结构：

```text
AppSettings
  WindowSettings
  PetSettings
  SystemSettings
  FeatureSettings
```

规则：

- 每个设置项必须有默认值。
- 修改设置后由 SettingsService 统一保存。
- 设置变更通过 `SettingsChanged` 事件广播。
- 不同模块只读取自己关心的设置段。

### 9.2 本地数据路径

Windows 运行时数据建议放在：

```text
%AppData%/NewDiNoLock/
  settings.json
  reminders.json
  logs/
```

Unity 编辑器调试时可使用：

```text
Application.persistentDataPath
```

规则：

- 代码中不要直接拼接 `%AppData%`。
- 不要把用户运行时数据写入 Assets。
- 测试数据放入 Tests 或 TestData，不混入正式资源目录。

## 10. 资源规范

### 10.1 皮肤目录

每个皮肤一个独立目录：

```text
Assets/NewDiNoLock/Art/Skins/
  DinoDefault/
    skin.json
    icon.png
    preview.png
    Spine/
    Frames/
```

规则：

- 皮肤 id 使用小写蛇形命名，例如 `dino_default`。
- 目录名使用 PascalCase，例如 `DinoDefault`。
- `skin.json` 中必须声明 id、displayName、type、scale、pivot、hitArea、animations。
- 缺失动画必须有回退策略。

### 10.2 动作命名

统一动作名：

```text
Idle
Walk
Lift
Drop
Click
Happy
Sleep
Notify
```

规则：

- 业务层只能使用统一动作名。
- 资源内真实动画名通过 skin.json 映射。
- MVP 必须提供 Idle、Walk、Lift、Click。

### 10.3 Prefab 规范

Prefab 命名：

| 类型 | 命名示例 |
| --- | --- |
| 宠物根节点 | PetRoot.prefab |
| 默认皮肤 | PetSkin_DinoDefault.prefab |
| 气泡 | BubbleView.prefab |
| 设置面板 | SettingsPanel.prefab |
| 系统入口 | AppRoot.prefab |

规则：

- Prefab 上的脚本引用必须尽量通过 Inspector 明确绑定。
- 不在 Prefab 中引用临时测试资源。
- 跨模块 Prefab 不直接引用 Feature 内部实现，使用事件或公开接口连接。

## 11. 功能开发落位指南

### 11.1 开发透明窗口/置顶/全屏显示

主要目录：

```text
Scripts/Window/
Scripts/System/
Scripts/Bootstrap/
```

应修改：

- `IDesktopWindowService`
- `DesktopWindowService`
- `AlwaysOnTopService`
- `FullscreenDetectService`
- `AppSettings.Window`

不要修改：

- PetStateMachine 的具体状态逻辑，除非需要响应全屏事件。
- SkinManager。
- Feature 模块。

### 11.2 开发随机行走

主要目录：

```text
Scripts/Core/
Scripts/System/
```

应修改：

- `PetMovementController`
- `PetBehaviorController`
- `PetBoundsService`
- `PetStateMachine`
- `AppSettings.Pet.autoWalkEnabled`

不要修改：

- Window 平台 API。
- Skin 资源加载逻辑。

### 11.3 开发拖动与点击互动

主要目录：

```text
Scripts/Core/
Scripts/UI/
Scripts/Rendering/
```

应修改：

- `PetInteractionController`
- `PetStateMachine`
- `IPetAnimationPlayer` 的调用逻辑。
- `BubbleView`。

注意：

- 拖动状态优先级高于提醒和行走。
- 鼠标穿透切换如需平台支持，应通过 Window 模块接口完成。

### 11.4 开发防锁屏

主要目录：

```text
Scripts/System/
Scripts/UI/
Scripts/Bootstrap/
```

应修改：

- `KeepAwakeService`
- `AppSettings.System.keepAwakeEnabled`
- 设置面板或托盘菜单入口。
- AppLifecycle 退出清理。

注意：

- 默认关闭。
- 应用退出必须释放。
- 平台 API 调用失败不能影响宠物显示。

### 11.5 开发皮肤切换

主要目录：

```text
Scripts/Rendering/
Scripts/UI/
Art/Skins/
ScriptableObjects/Skins/
```

应修改：

- `SkinManager`
- `SkinDefinition`
- `SpineAnimationPlayer`
- `FrameAnimationPlayer`
- `SkinSelectView`

注意：

- 统一动作名不能随皮肤变化。
- Spine 和帧动画必须走同一个 `IPetAnimationPlayer`。

### 11.6 开发番茄钟

主要目录：

```text
Scripts/Features/Pomodoro/
Scripts/Features/Common/
Scripts/UI/
Scripts/System/
```

应修改：

- `PomodoroFeature`
- `PomodoroSettings`
- `PomodoroState`
- `ReminderEvent`
- `PomodoroPanelView`

不要修改：

- PetStateMachine 的基础状态定义，除非新增提醒优先级。
- Window 平台能力。

### 11.7 开发喝水/站立提醒

主要目录：

```text
Scripts/Features/HealthReminder/
Scripts/Features/Common/
Scripts/UI/
Scripts/System/
```

应修改：

- `HealthReminderFeature`
- `HealthReminderSettings`
- `HealthReminderType`
- `ReminderPopupView`

注意：

- 提醒功能只发出 ReminderEvent。
- 是否播放 Notify 动画由 Core 决定。

### 11.8 开发待办事项

主要目录：

```text
Scripts/Features/Todo/
Scripts/Features/Common/
Scripts/UI/
Scripts/System/
```

应修改：

- `TodoFeature`
- `TodoItem`
- `TodoRepository`
- 待办 UI。

注意：

- 待办数据不要写入设置文件。
- 使用独立 `reminders.json` 或 todo 数据文件。

## 12. 子 agent 协作规范

### 12.1 开发前必须确认

每个子 agent 开发前应确认：

- 目标功能属于哪个模块。
- 主要修改目录和文件。
- 是否需要新增公共接口。
- 是否会影响 PetStateMachine、SettingsService、AppBootstrapper 这类共享文件。
- 是否需要迁移或新增资源目录。

### 12.2 修改范围

原则：

- 单个功能尽量只修改自己的模块目录。
- 修改共享接口时，必须同步更新调用方和文档注释。
- 不做与当前功能无关的重构。
- 不移动 Unity 资源文件，除非任务明确要求整理目录。
- 不删除他人未确认的资源、脚本、meta 文件。

### 12.3 共享文件风险等级

高风险共享文件：

```text
AppBootstrapper.cs
ServiceRegistry.cs
PetStateMachine.cs
AppSettings.cs
SettingsService.cs
EventBus.cs
IPetAnimationPlayer.cs
IDesktopWindowService.cs
```

修改这些文件时需要特别说明：

- 为什么必须修改。
- 新增或改变了什么接口。
- 哪些模块会受到影响。
- 是否需要其他功能同步适配。

### 12.4 交付说明要求

每个功能开发完成后，交付说明至少包含：

- 修改了哪些目录和主要文件。
- 新增了哪些设置项或资源规范。
- 如何在 Unity 中验证。
- 运行过哪些测试。
- 尚未覆盖的风险。

## 13. 测试规范

### 13.1 EditMode 测试

适合测试：

- PetStateMachine 状态切换。
- 行为优先级。
- 设置默认值和反序列化。
- 番茄钟计时逻辑。
- 提醒调度逻辑。
- 皮肤动作名回退。

目录：

```text
Assets/NewDiNoLock/Tests/EditMode/
```

### 13.2 PlayMode 测试

适合测试：

- 宠物拖动交互。
- UI 面板显示。
- 皮肤切换后的 GameObject 状态。
- 提醒气泡生命周期。

目录：

```text
Assets/NewDiNoLock/Tests/PlayMode/
```

### 13.3 手动验证清单

桌面宠物 MVP 每次关键修改后至少手动验证：

- 应用启动后宠物显示正常。
- 透明背景未出现异常色块。
- 置顶开关生效。
- 自动行走不越出屏幕。
- 拖动时切换 Lift 动作，松开后恢复。
- 隐藏/显示可恢复。
- 设置保存后重启仍生效。
- 退出应用后防锁屏释放。

## 14. 日志规范

建议统一通过 `Logger` 输出日志。

日志级别：

| 级别 | 使用场景 |
| --- | --- |
| Debug | 开发期状态变化、调试信息 |
| Info | 功能启动、设置变更、皮肤切换 |
| Warning | 可恢复异常、资源缺失、兼容性降级 |
| Error | 功能失败、平台 API 调用失败、数据损坏 |

规则：

- 不在高频 Update 中持续刷日志。
- 用户隐私数据不写日志。
- 通讯工具消息正文默认不写日志。

## 15. 平台适配规范

### 15.1 Windows 优先

MVP 优先支持 Windows，相关平台能力集中在：

```text
Scripts/Window/
Scripts/System/
```

涉及能力：

- 无边框透明窗口。
- 置顶。
- 全屏检测。
- 托盘。
- 鼠标穿透。
- 防锁屏。
- 全局快捷键。

### 15.2 条件编译

平台相关代码必须使用条件编译隔离：

```csharp
#if UNITY_STANDALONE_WIN
// Windows implementation
#endif
```

规则：

- 非 Windows 平台至少提供空实现或安全回退。
- 业务模块不直接包含大量平台条件编译。
- 平台差异优先隐藏在服务实现内部。

## 16. UI 开发规范

### 16.1 UI 分层

推荐分层：

```text
View
  只负责显示和 Unity 事件绑定
Controller/ViewModel
  负责把用户操作转为服务调用
Service/Feature
  负责业务逻辑
```

### 16.2 UI 行为

规则：

- 提醒气泡不应永久遮挡用户工作区域。
- 设置修改应即时生效，并持久化。
- 右键菜单与托盘菜单的同类开关应保持状态一致。
- 隐私相关提示应默认保守。

## 17. 代码注释规范

- 公共接口需要 XML 注释说明用途。
- 复杂状态切换需要简短注释说明原因。
- 平台 API 调用需要注释说明关键参数含义。
- 不写重复代码含义的空洞注释。

示例：

```csharp
/// <summary>
/// Plays pet animations through a skin-agnostic interface.
/// Core systems should depend on this interface instead of Spine or frame animation implementations.
/// </summary>
public interface IPetAnimationPlayer
{
}
```

## 18. 推荐开发顺序

建议按以下顺序建立基础代码：

1. 建立 `Assets/NewDiNoLock` 目录结构。
2. 建立 Bootstrap、Infrastructure、System 基础服务。
3. 建立 Core 状态机和宠物根 Prefab。
4. 建立 Rendering 接口和默认帧动画/占位动画。
5. 建立 Window 透明置顶能力。
6. 建立拖动、点击、自动行走。
7. 建立设置保存和设置 UI。
8. 建立皮肤切换。
9. 建立防锁屏。
10. 建立番茄钟、健康提醒、待办提醒等 Feature。

## 19. 变更文档要求

当代码结构发生以下变化时，需要更新本文档：

- 新增顶层模块目录。
- 新增或移除 asmdef。
- 修改核心依赖方向。
- 修改皮肤资源结构。
- 修改用户设置数据结构。
- 新增平台适配策略。
- 新增 Feature 模块类型。

## 20. 快速索引

| 想做的事 | 先看哪里 |
| --- | --- |
| 宠物状态/动画切换 | Scripts/Core, Scripts/Rendering |
| 透明窗口/置顶/全屏 | Scripts/Window |
| 防锁屏/快捷键/设置保存 | Scripts/System |
| 换皮肤 | Scripts/Rendering, Art/Skins |
| 气泡/菜单/设置面板 | Scripts/UI |
| 番茄钟 | Scripts/Features/Pomodoro |
| 喝水/站立提醒 | Scripts/Features/HealthReminder |
| 待办提醒 | Scripts/Features/Todo |
| 新消息提示 | Scripts/Features/MessageNotifier |
| 跨模块事件 | Scripts/Infrastructure/EventBus.cs |
| 本地数据路径 | Scripts/System/AppPaths.cs |
