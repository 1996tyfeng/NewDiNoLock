# A02-A04 Implementation Interface Guide

本文档总结 A02 Infrastructure、A03 SettingsSystem、A04 BootstrapLifecycle 已落地的公共接口、方法和协作边界，供后续 agent 开发时快速接入。

## 1. Infrastructure

命名空间：

```csharp
NewDiNoLock.Infrastructure
```

程序集：

```text
NewDiNoLock.Infrastructure
```

### 1.1 IEventBus

```csharp
public interface IEventBus
{
    IDisposable Subscribe<TEvent>(Action<TEvent> handler);
    void Publish<TEvent>(TEvent eventData);
}
```

用途：

- 跨模块事件发布和订阅。
- `Subscribe` 返回的 `IDisposable` 必须在模块销毁或生命周期结束时释放。
- 高频事件使用泛型订阅列表，避免 struct 事件发布时产生装箱。

示例：

```csharp
var subscription = eventBus.Subscribe<SettingsChanged>(eventData =>
{
    var settings = eventData.Settings;
});

lifecycle.RegisterDisposable(subscription);
```

### 1.2 ILogger

```csharp
public interface ILogger
{
    void Debug(string message);
    void Info(string message);
    void Warning(string message);
    void Error(string message);
    void Error(Exception exception, string message = null);
}
```

用途：

- 统一日志输出。
- 可恢复问题使用 `Warning`。
- 功能失败、平台 API 失败、数据损坏使用 `Error`。

注意：

- 与 `UnityEngine.ILogger` 同名，业务代码中如出现歧义，显式使用 `NewDiNoLock.Infrastructure.ILogger`。

### 1.3 ITimeProvider

```csharp
public interface ITimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    float DeltaTime { get; }
    float UnscaledDeltaTime { get; }
}
```

实现：

- `TimeProvider`：运行时使用 Unity 时间。
- `ManualTimeProvider`：测试中手动推进时间。

`ManualTimeProvider` 额外方法：

```csharp
void Advance(TimeSpan timeSpan);
void SetDeltaTime(float deltaTime);
```

### 1.4 DisposableGroup

```csharp
public sealed class DisposableGroup : IDisposable
{
    int Count { get; }
    bool IsDisposed { get; }

    T Add<T>(T disposable) where T : IDisposable;
    bool Remove(IDisposable disposable);
    void Clear();
    void Dispose();
}
```

用途：

- 统一释放多个订阅或资源。
- `Dispose` 后再 `Add` 的资源会被立即释放。

### 1.5 Platform

```csharp
public enum RuntimePlatformKind
{
    Unknown,
    Windows,
    MacOS,
    Linux,
    Editor
}

public static class Platform
{
    RuntimePlatformKind Current { get; }
    bool IsEditor { get; }
    bool IsWindows { get; }
    bool IsMacOS { get; }
    bool IsLinux { get; }
}
```

用途：

- 基础平台判断。
- 具体窗口 API 仍应放在 `Scripts/Window/`，不要写进 Infrastructure。

## 2. System

命名空间：

```csharp
NewDiNoLock.System
```

程序集：

```text
NewDiNoLock.System
```

依赖：

```text
NewDiNoLock.Infrastructure
```

### 2.1 ISettingsService

```csharp
public interface ISettingsService
{
    AppSettings Current { get; }
    AppSettings Load();
    void Save(AppSettings settings);
    void Update(Action<AppSettings> updateSettings);
}
```

用途：

- 统一加载、保存、修改用户设置。
- 修改设置时优先使用 `Update`。
- `Save` 和 `Update` 会发布 `SettingsChanged` 事件。

示例：

```csharp
settingsService.Update(settings =>
{
    settings.window.alwaysOnTop = false;
});
```

### 2.2 SettingsChanged

```csharp
public readonly struct SettingsChanged
{
    public AppSettings Settings { get; }
}
```

用途：

- 设置保存后通过 `IEventBus` 广播。
- 其他模块订阅后只读取自己关心的设置段。

### 2.3 AppPaths

```csharp
public sealed class AppPaths
{
    public const string SettingsFileName = "settings.json";

    public string DataRoot { get; }
    public string SettingsFilePath { get; }
}
```

默认路径：

```text
Application.persistentDataPath/settings.json
```

测试可注入临时目录：

```csharp
var paths = new AppPaths(tempRoot);
```

规则：

- 不要把用户运行时数据写入 `Assets`。
- 不要在业务模块散落硬编码本地路径。
- 本地路径统一通过 `AppPaths` 扩展。

### 2.4 ILocalStorageService

```csharp
public interface ILocalStorageService
{
    bool FileExists(string path);
    string ReadAllText(string path);
    void WriteAllText(string path, string contents);
    void EnsureDirectory(string path);
}
```

用途：

- 文件读写抽象。
- 当前运行时实现为 `LocalStorageService`。

### 2.5 AppSettings

```csharp
public sealed class AppSettings
{
    public WindowSettings window;
    public PetSettings pet;
    public SystemSettings system;
    public FeatureSettings features;

    public static AppSettings CreateDefault();
    public AppSettings Clone();
    public void Normalize();
}
```

设置分段：

```text
AppSettings
  WindowSettings window
  PetSettings pet
  SystemSettings system
  FeatureSettings features
```

默认值：

```csharp
settings.window.alwaysOnTop = true;
settings.window.showAboveFullscreen = false;
settings.window.autoHideInFullscreen = true;

settings.pet.autoWalkEnabled = true;
settings.pet.currentSkinId = "dino_default";
settings.pet.restorePositionOnStart = true;
settings.pet.position = null;

settings.system.keepAwakeEnabled = false;
settings.system.volume = 0.5f;
settings.system.interactionBubbleEnabled = true;

settings.features.pomodoro.enabled = false;
settings.features.healthReminder.enabled = false;
settings.features.todoReminder.enabled = false;
```

### 2.6 WindowSettings

```csharp
public sealed class WindowSettings
{
    public bool alwaysOnTop;
    public bool showAboveFullscreen;
    public bool autoHideInFullscreen;

    public static WindowSettings CreateDefault();
}
```

### 2.7 PetSettings

```csharp
public sealed class PetSettings
{
    public const string DefaultSkinId = "dino_default";

    public bool autoWalkEnabled;
    public string currentSkinId;
    public bool restorePositionOnStart;
    public PetPosition position;

    public static PetSettings CreateDefault();
    public void Normalize();
}

public sealed class PetPosition
{
    public int displayIndex;
    public float x;
    public float y;
}
```

### 2.8 SystemSettings

```csharp
public sealed class SystemSettings
{
    public bool keepAwakeEnabled;
    public float volume;
    public bool interactionBubbleEnabled;

    public static SystemSettings CreateDefault();
    public void Normalize();
}
```

`Normalize` 会将 `volume` 限制在 `0..1`。

### 2.9 FeatureSettings

```csharp
public sealed class FeatureSettings
{
    public FeatureToggleSettings pomodoro;
    public FeatureToggleSettings healthReminder;
    public FeatureToggleSettings todoReminder;

    public static FeatureSettings CreateDefault();
    public void Normalize();
}

public sealed class FeatureToggleSettings
{
    public bool enabled;

    public static FeatureToggleSettings CreateDisabled();
}
```

## 3. Bootstrap

命名空间：

```csharp
NewDiNoLock.Bootstrap
```

程序集：

```text
NewDiNoLock.Bootstrap
```

依赖：

```text
NewDiNoLock.Infrastructure
NewDiNoLock.System
```

### 3.1 ServiceRegistry

```csharp
public sealed class ServiceRegistry : IDisposable
{
    void Register<TService>(TService service, bool disposeWithRegistry = true);
    bool TryResolve<TService>(out TService service);
    TService Resolve<TService>();
    void Dispose();
}
```

用途：

- 服务注册与解析。
- 默认会在 `ServiceRegistry.Dispose` 时释放实现了 `IDisposable` 的服务。
- 如果服务不应由 Registry 拥有，注册时传 `disposeWithRegistry: false`。

示例：

```csharp
services.Register<IMyService>(myService);
var myService = services.Resolve<IMyService>();
```

### 3.2 AppLifecycle

```csharp
public sealed class AppLifecycle : IDisposable
{
    bool IsRunning { get; }
    bool IsShutdown { get; }

    void MarkStarted();
    void RegisterShutdownAction(Action action);
    T RegisterDisposable<T>(T disposable) where T : IDisposable;
    void Shutdown();
    void Dispose();
}
```

用途：

- 统一退出清理。
- 托盘、防锁屏、事件订阅、窗口服务等资源都应注册到这里。
- 清理顺序是注册逆序。

示例：

```csharp
var subscription = eventBus.Subscribe<MyEvent>(OnMyEvent);
lifecycle.RegisterDisposable(subscription);

lifecycle.RegisterShutdownAction(() =>
{
    trayService.DisposeTray();
});
```

### 3.3 AppBootstrapper

```csharp
public sealed class AppBootstrapper : MonoBehaviour
{
    ServiceRegistry Services { get; }
    AppLifecycle Lifecycle { get; }

    void Bootstrap();
    void Shutdown();

    static ServiceRegistry CreateDefaultServices(AppLifecycle lifecycle);
}
```

启动顺序：

```text
AppBootstrapper.Awake
  -> Create AppLifecycle
  -> CreateDefaultServices
  -> SettingsService.Load()
  -> Lifecycle.MarkStarted()
```

当前默认注册的服务：

```csharp
AppLifecycle
IEventBus -> EventBus
ILogger -> Logger
ITimeProvider -> TimeProvider
AppPaths
ILocalStorageService -> LocalStorageService
ISettingsService -> SettingsService
```

后续模块接入点：

```csharp
var lifecycle = services.Resolve<AppLifecycle>();
var eventBus = services.Resolve<IEventBus>();
var settings = services.Resolve<ISettingsService>();

lifecycle.RegisterDisposable(subscription);
lifecycle.RegisterShutdownAction(() => SomeCleanup());
```

注意：

- `AppBootstrapper` 只做依赖组装和启动，不写具体业务逻辑。
- Window、Rendering、Core、Features 后续可以在 `CreateDefaultServices` 中注册服务。
- 具体窗口平台 API 不写在 Bootstrap。
- 宠物状态机细节不写在 Bootstrap。
- Feature 业务逻辑不写在 Bootstrap。

## 4. Scenes And Prefabs

主场景：

```text
Assets/NewDiNoLock/Scenes/Main.unity
```

系统入口预制体：

```text
Assets/NewDiNoLock/Prefabs/System/AppRoot.prefab
```

场景入口：

```text
Main.unity
  AppRoot
    AppBootstrapper
```

## 5. Collaboration Rules For Later Agents

后续 agent 必须遵守：

- 不要重复创建全局 `EventBus`、`SettingsService`、`Logger`。
- 不要把用户运行时数据写入 `Assets`。
- 不要让 `Infrastructure` 依赖业务模块。
- 订阅事件必须释放，推荐注册到 `AppLifecycle` 或 `DisposableGroup`。
- 修改设置必须通过 `ISettingsService`。
- 本地路径必须通过 `AppPaths`。
- Bootstrap 只做组装，不承载窗口 API、宠物状态机、Feature 业务逻辑。
- 后续 asmdef 依赖方向应保持：

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

## 6. Current Verification Notes

当前已通过 Unity 自带 Roslyn/Mono 编译器验证以下程序集可编译：

```text
NewDiNoLock.Infrastructure
NewDiNoLock.System
NewDiNoLock.Bootstrap
NewDiNoLock.Tests.EditMode
```

Unity batchmode Test Runner 暂未能执行到测试阶段，原因是本机 Unity Licensing Client IPC 超时，未生成测试结果 XML。
