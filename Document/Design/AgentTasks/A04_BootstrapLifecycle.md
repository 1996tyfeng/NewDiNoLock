# A04 Bootstrap 与生命周期任务

## 任务目标

建立应用启动入口、服务注册和退出清理流程，让后续模块可以稳定接入。

## 推荐模型

GPT-5.5 中或 gpt-5.3-codex。

## 必读文档

- `../Project_CodeStructure_Guidelines.md` 中 Bootstrap
- `../WorkBreakdown_Priority_ModelGuide.md` 中 T0.2

## 负责范围

主要目录：

```text
Assets/NewDiNoLock/Scripts/Bootstrap/
Assets/NewDiNoLock/Prefabs/System/
Assets/NewDiNoLock/Scenes/
```

建议文件：

```text
AppBootstrapper.cs
AppLifecycle.cs
ServiceRegistry.cs
```

## 功能要求

- 主场景中存在统一应用入口。
- 初始化 Infrastructure、Settings、Window、Rendering、Core 等服务的接入位置。
- 应用退出时提供统一清理入口。
- 为防锁屏、事件订阅、托盘等后续资源释放预留流程。

## 不要修改

- 不写具体窗口平台 API。
- 不写宠物状态机细节。
- 不写 Feature 业务逻辑。

## 验收标准

- Main 场景启动能执行 Bootstrap。
- 服务注册关系清晰。
- 退出时能调用生命周期清理。

## 交付说明

说明启动顺序、服务注册方式和后续模块接入点。

