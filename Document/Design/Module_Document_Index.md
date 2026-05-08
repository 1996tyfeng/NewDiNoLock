# NewDiNoLock 模块文档查询索引

## 1. 使用目的

本文档是后续 agent 写代码前的第一入口。它不承载完整设计细节，只负责把问题快速路由到最少、最相关的文档内容，降低无效阅读和 token 消耗。

推荐阅读顺序：

1. 先读本文档，定位任务所属模块。
2. 再读对应的 Agent 任务文档。
3. 只阅读公共设计文档中的相关章节。
4. 需要接入公共服务时，再读 `Implementation_CommonInterfaces.md` 的对应接口。

## 2. 必读等级

| 等级 | 说明 |
| --- | --- |
| 必读 | 不读容易写错边界、重复造服务或破坏依赖方向 |
| 按需读 | 任务碰到相关问题时再读 |
| 背景读 | 用于理解产品目标，不要求完整阅读 |

## 3. 文档地图

```text
Module_Document_Index.md
  ├─ 我要理解产品功能
  │   └─ DesktopPet_FunctionalDesign.md
  ├─ 我要知道代码放哪里
  │   └─ Project_CodeStructure_Guidelines.md
  ├─ 我要接公共服务
  │   └─ Implementation_CommonInterfaces.md
  ├─ 我要派发或领取任务
  │   ├─ WorkBreakdown_Priority_ModelGuide.md
  │   └─ AgentTasks/*.md
  └─ 我要验收和测试
      ├─ Project_CodeStructure_Guidelines.md
      ├─ WorkBreakdown_Priority_ModelGuide.md
      └─ AgentTasks/Q01_TestBuildQA.md
```

## 4. 公共文档索引

| 文档 | 何时阅读 | 重点内容 |
| --- | --- | --- |
| `DesktopPet_FunctionalDesign.md` | 需要理解用户体验、功能行为、验收目标 | 产品目标、宠物状态、窗口显示、互动、皮肤、提醒 |
| `Project_CodeStructure_Guidelines.md` | 任何要新增代码、资源、测试的任务 | 目录结构、模块边界、命名空间、编码规范、测试规范 |
| `Implementation_CommonInterfaces.md` | 任何要写 Unity C# 代码并接入公共服务的任务 | EventBus、Logger、TimeProvider、SettingsService、Lifecycle、ServiceRegistry |
| `WorkBreakdown_Priority_ModelGuide.md` | 派单、排期、判断模型档位时 | Phase 拆分、任务依赖、模型推荐、验收标准 |
| `AgentTasks/README_AgentTask_Index.md` | 给子 agent 分配具体任务时 | 任务文档列表、推荐派单顺序、通用交付要求 |

## 5. 按任务类型查询

### 5.1 我只是在创建目录或整理资源

必读：

- `AgentTasks/A01_ProjectSkeleton.md`
- `Project_CodeStructure_Guidelines.md` 第 4 节“推荐 Assets 目录结构”

按需读：

- `WorkBreakdown_Priority_ModelGuide.md` 中 T0.1

通常不需要读：

- `Implementation_CommonInterfaces.md`

### 5.2 我要写基础设施：事件、日志、时间、释放

必读：

- `AgentTasks/A02_Infrastructure.md`
- `Project_CodeStructure_Guidelines.md` 第 5.8 节“Infrastructure”

按需读：

- `Implementation_CommonInterfaces.md` 第 1 节“Infrastructure”

注意：

- 不要让 Infrastructure 依赖 Core、System、Window、Rendering、Features、UI。

### 5.3 我要写设置系统、本地存储、设置项

必读：

- `AgentTasks/A03_SettingsSystem.md`
- `Project_CodeStructure_Guidelines.md` 第 5.5 节“System”
- `Project_CodeStructure_Guidelines.md` 第 9 节“配置与数据规范”
- `Implementation_CommonInterfaces.md` 第 2 节“System”

背景读：

- `DesktopPet_FunctionalDesign.md` 第 6.7 节“设置系统”
- `DesktopPet_FunctionalDesign.md` 第 9.1 节“用户设置”

注意：

- 修改设置必须通过 `ISettingsService`。
- 用户运行时数据不要写入 `Assets`。

### 5.4 我要写 Bootstrap、生命周期、服务注册

必读：

- `AgentTasks/A04_BootstrapLifecycle.md`
- `Project_CodeStructure_Guidelines.md` 第 5.1 节“Bootstrap”
- `Implementation_CommonInterfaces.md` 第 3 节“Bootstrap”

注意：

- Bootstrap 只做依赖组装和生命周期，不写窗口 API、宠物状态机细节或 Feature 业务逻辑。

### 5.5 我要写宠物状态机

必读：

- `AgentTasks/B01_PetStateMachine.md`
- `DesktopPet_FunctionalDesign.md` 第 6.2 节“宠物行为系统”
- `Project_CodeStructure_Guidelines.md` 第 5.2 节“Core”

按需读：

- `Implementation_CommonInterfaces.md` 第 1.1 节 `IEventBus`
- `Implementation_CommonInterfaces.md` 第 1.2 节 `ILogger`

注意：

- 状态切换集中在状态机，不要散落到 UI 或 Feature。
- 不直接依赖 Spine、Windows API 或具体 UI。

### 5.6 我要写动画接口、占位动画、皮肤播放

必读：

- `AgentTasks/B02_RenderingAnimationBase.md`
- `DesktopPet_FunctionalDesign.md` 第 6.6 节“皮肤与动画系统”
- `Project_CodeStructure_Guidelines.md` 第 5.3 节“Rendering”
- `Project_CodeStructure_Guidelines.md` 第 10 节“资源规范”

按需读：

- `Implementation_CommonInterfaces.md` 第 1.2 节 `ILogger`
- `Implementation_CommonInterfaces.md` 第 2.1 节 `ISettingsService`

注意：

- Core 只能依赖 `IPetAnimationPlayer`，不要直接调用 Spine 或 Animator 细节。

### 5.7 我要集成宠物行为调度

必读：

- `AgentTasks/B03_PetBehaviorIntegration.md`
- `DesktopPet_FunctionalDesign.md` 第 6.2、6.3 节
- `Project_CodeStructure_Guidelines.md` 第 5.2、5.3 节

按需读：

- `Implementation_CommonInterfaces.md` 第 1.1 节 `IEventBus`
- `Implementation_CommonInterfaces.md` 第 3.2 节 `AppLifecycle`

注意：

- 行为调度负责状态到动画的连接，不直接处理平台窗口能力。

### 5.8 我要写透明窗口、置顶、全屏上方显示

必读：

- `AgentTasks/C01_WindowBase.md`
- `DesktopPet_FunctionalDesign.md` 第 6.1 节“桌面窗口显示”
- `Project_CodeStructure_Guidelines.md` 第 5.4 节“Window”
- `Project_CodeStructure_Guidelines.md` 第 15 节“平台适配规范”
- `Implementation_CommonInterfaces.md` 第 1.2 节 `ILogger`
- `Implementation_CommonInterfaces.md` 第 2.1 节 `ISettingsService`
- `Implementation_CommonInterfaces.md` 第 3.2 节 `AppLifecycle`

注意：

- Windows API 只能放在 Window 或 System 的平台适配实现中。
- 非 Windows 平台必须安全回退。

### 5.9 我要写拖动、点击互动、自动行走

必读：

- `AgentTasks/C02_InteractionMovement.md`
- `DesktopPet_FunctionalDesign.md` 第 6.2、6.3 节
- `Project_CodeStructure_Guidelines.md` 第 11.2、11.3 节

按需读：

- `Implementation_CommonInterfaces.md` 第 1.1 节 `IEventBus`
- `Implementation_CommonInterfaces.md` 第 1.3 节 `ITimeProvider`
- `Implementation_CommonInterfaces.md` 第 2.1 节 `ISettingsService`

注意：

- 拖动状态优先级高于提醒和行走。
- 自动行走必须受 `autoWalkEnabled` 控制。

### 5.10 我要写隐藏显示、鼠标穿透、右键菜单、托盘菜单

必读：

- `AgentTasks/C03_HideMenuTray.md`
- `DesktopPet_FunctionalDesign.md` 第 6.4、6.8 节
- `Project_CodeStructure_Guidelines.md` 第 5.4、5.7、16 节
- `Implementation_CommonInterfaces.md` 第 2.1 节 `ISettingsService`
- `Implementation_CommonInterfaces.md` 第 3.2 节 `AppLifecycle`

按需读：

- `Implementation_CommonInterfaces.md` 第 1.1 节 `IEventBus`
- `Implementation_CommonInterfaces.md` 第 1.2 节 `ILogger`

注意：

- UI 不直接写磁盘，不直接调用平台 API。
- 托盘、防锁屏、事件订阅等资源要注册到生命周期清理。

### 5.11 我要写完整皮肤系统

必读：

- `AgentTasks/D01_SkinSystem.md`
- `DesktopPet_FunctionalDesign.md` 第 6.6 节
- `Project_CodeStructure_Guidelines.md` 第 5.3、10、11.5 节

按需读：

- `Implementation_CommonInterfaces.md` 第 1.2 节 `ILogger`
- `Implementation_CommonInterfaces.md` 第 2.1 节 `ISettingsService`
- `Implementation_CommonInterfaces.md` 第 2.5、2.7 节设置模型

注意：

- `skin.json` 或皮肤配置错误时必须有回退。
- Spine 接入不应污染 Core 层。

### 5.12 我要写防锁屏、全屏检测、快捷键

必读：

- `AgentTasks/D02_SystemCompatibility.md`
- `DesktopPet_FunctionalDesign.md` 第 6.5、6.1.2 节
- `Project_CodeStructure_Guidelines.md` 第 5.4、5.5、15 节
- `Implementation_CommonInterfaces.md` 第 1.2 节 `ILogger`
- `Implementation_CommonInterfaces.md` 第 2.1 节 `ISettingsService`
- `Implementation_CommonInterfaces.md` 第 3.2 节 `AppLifecycle`

注意：

- 防锁屏默认关闭。
- 应用退出必须释放系统状态。
- 平台 API 失败不能影响宠物核心显示。

### 5.13 我要写番茄钟、喝水提醒、站立提醒

必读：

- `AgentTasks/E01_ReminderPomodoroHealth.md`
- `DesktopPet_FunctionalDesign.md` 第 7.1、7.2 节
- `Project_CodeStructure_Guidelines.md` 第 5.6、16 节
- `Implementation_CommonInterfaces.md` 第 1.1 节 `IEventBus`
- `Implementation_CommonInterfaces.md` 第 1.3 节 `ITimeProvider`
- `Implementation_CommonInterfaces.md` 第 2.1 节 `ISettingsService`

按需读：

- `Implementation_CommonInterfaces.md` 第 3.2 节 `AppLifecycle`

注意：

- Feature 只发出提醒事件，不直接控制宠物动画。
- 是否进入 Notify 状态由 Core 决定。

### 5.14 我要写待办事项、消息提示扩展

必读：

- `AgentTasks/E02_TodoMessageExtensions.md`
- `DesktopPet_FunctionalDesign.md` 第 7.3、7.4 节
- `Project_CodeStructure_Guidelines.md` 第 5.6、9.2 节
- `Implementation_CommonInterfaces.md` 第 1.1 节 `IEventBus`
- `Implementation_CommonInterfaces.md` 第 2.3、2.4 节 `AppPaths` 与 `ILocalStorageService`

按需读：

- `Implementation_CommonInterfaces.md` 第 1.3 节 `ITimeProvider`
- `Implementation_CommonInterfaces.md` 第 2.1 节 `ISettingsService`

注意：

- 待办数据独立保存，不写入 `settings.json`。
- 消息提示默认保护隐私，不记录敏感正文。

### 5.15 我要写测试、构建、QA

必读：

- `AgentTasks/Q01_TestBuildQA.md`
- `DesktopPet_FunctionalDesign.md` 第 10、13 节
- `Project_CodeStructure_Guidelines.md` 第 13、14 节
- `WorkBreakdown_Priority_ModelGuide.md` 第 13 节

按需读：

- `Implementation_CommonInterfaces.md` 第 1.3 节 `ManualTimeProvider`
- `Implementation_CommonInterfaces.md` 第 2 节设置接口

注意：

- 测试不要为了方便大规模重构业务代码。
- 测试数据不要混入正式资源。

## 6. 按常见问题查询

### 我需要发跨模块事件

读：

- `Implementation_CommonInterfaces.md` 第 1.1 节 `IEventBus`
- `Project_CodeStructure_Guidelines.md` 第 8.3 节“事件规则”

规则：

- 订阅必须释放。
- 推荐把订阅注册到 `AppLifecycle` 或 `DisposableGroup`。

### 我需要保存或读取设置

读：

- `Implementation_CommonInterfaces.md` 第 2.1 节 `ISettingsService`
- `Implementation_CommonInterfaces.md` 第 2.5-2.9 节设置模型
- `Project_CodeStructure_Guidelines.md` 第 9.1 节“用户设置”

规则：

- 修改设置优先使用 `settingsService.Update(...)`。
- 不直接写 `settings.json`。

### 我需要本地保存业务数据

读：

- `Implementation_CommonInterfaces.md` 第 2.3 节 `AppPaths`
- `Implementation_CommonInterfaces.md` 第 2.4 节 `ILocalStorageService`
- `Project_CodeStructure_Guidelines.md` 第 9.2 节“本地数据路径”

规则：

- 不写入 `Assets`。
- 业务数据应独立于 `settings.json`。

### 我需要计时、定时提醒或测试时间流逝

读：

- `Implementation_CommonInterfaces.md` 第 1.3 节 `ITimeProvider`
- `AgentTasks/E01_ReminderPomodoroHealth.md`

规则：

- 运行时使用 `TimeProvider`。
- 测试使用 `ManualTimeProvider`。

### 我需要应用退出时清理资源

读：

- `Implementation_CommonInterfaces.md` 第 3.2 节 `AppLifecycle`
- `Implementation_CommonInterfaces.md` 第 1.4 节 `DisposableGroup`

规则：

- 事件订阅、托盘、防锁屏、窗口句柄相关资源都必须进入清理链路。

### 我需要新增服务或模块

读：

- `Implementation_CommonInterfaces.md` 第 3.1 节 `ServiceRegistry`
- `Implementation_CommonInterfaces.md` 第 3.3 节 `AppBootstrapper`
- `Project_CodeStructure_Guidelines.md` 第 6 节“Assembly Definition 建议”

规则：

- 服务在 Bootstrap 中组装。
- 不在业务代码中重复创建全局单例。

### 我需要判断文件应该放在哪

读：

- `Project_CodeStructure_Guidelines.md` 第 4、5、11 节
- 对应 `AgentTasks/*.md`

规则：

- 优先按模块目录落位。
- 共享接口变更要说明影响范围。

### 我需要接 Windows API

读：

- `Project_CodeStructure_Guidelines.md` 第 15 节“平台适配规范”
- `AgentTasks/C01_WindowBase.md`
- `AgentTasks/D02_SystemCompatibility.md`

规则：

- 使用条件编译。
- 非 Windows 平台提供安全回退。
- 平台 API 不写进 Core 或 UI。

## 7. 每个 agent 的最小阅读包

派单时可以按这个格式给：

```text
请先阅读：
1. /Users/littlefive/UnityProject/NewDiNoLock/Document/Design/Module_Document_Index.md
2. /Users/littlefive/UnityProject/NewDiNoLock/Document/Design/AgentTasks/对应任务文档.md
3. Module_Document_Index.md 中该任务列出的必读章节

如果需要事件、设置、时间、日志、生命周期或服务注册，请只阅读：
/Users/littlefive/UnityProject/NewDiNoLock/Document/Design/Implementation_CommonInterfaces.md 的对应小节
```

## 8. 更新规则

以下情况必须更新本文档：

- 新增 Agent 任务文档。
- 新增公共接口文档。
- 新增顶层模块或改变模块边界。
- 修改 Bootstrap 注册方式。
- 修改设置模型、事件规则、本地存储规则。
- 某个功能的必读文档发生变化。

