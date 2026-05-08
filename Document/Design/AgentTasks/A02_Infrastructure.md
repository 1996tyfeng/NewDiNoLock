# A02 Infrastructure 基础设施任务

## 任务目标

实现跨模块基础设施，包括事件总线、日志、时间抽象和释放工具。

## 推荐模型

GPT-5.5 中。

## 必读文档

- `../Project_CodeStructure_Guidelines.md`
- `../WorkBreakdown_Priority_ModelGuide.md` 中 T0.3

## 负责范围

主要目录：

```text
Assets/NewDiNoLock/Scripts/Infrastructure/
Assets/NewDiNoLock/Tests/EditMode/
```

建议文件：

```text
EventBus.cs
Logger.cs
TimeProvider.cs
DisposableGroup.cs
Platform.cs
```

## 功能要求

- `EventBus` 支持订阅、取消订阅、发布事件。
- 事件订阅必须可释放，避免重复回调和内存泄漏。
- `TimeProvider` 提供当前时间、delta time 或可测试时间来源。
- `Logger` 封装 Debug/Info/Warning/Error。
- `DisposableGroup` 用于统一释放多个订阅或资源。

## 接口建议

```csharp
public interface IEventBus
{
    IDisposable Subscribe<TEvent>(Action<TEvent> handler);
    void Publish<TEvent>(TEvent eventData);
}
```

## 不要修改

- 不写宠物状态机。
- 不写设置系统的具体数据模型。
- 不依赖 Core、UI、Features 等业务模块。

## 验收标准

- EditMode 测试覆盖事件发布、订阅、取消订阅。
- Infrastructure 不依赖业务命名空间。
- 高频事件不会产生明显 GC 压力的设计风险。

## 交付说明

说明公开接口、测试结果和后续模块如何使用。

