# Agent 任务文档索引

## 使用建议

推荐给每个子 agent 下发：

1. `../Module_Document_Index.md`
2. 本任务目录中的对应任务文档。
3. 索引文档中该任务列出的必读章节。

不建议只把完整架构文档直接丢给子 agent。完整文档适合作为背景，但任务执行时还需要明确“负责范围、不要改哪里、验收标准、模型建议”。本目录的子文档负责派单，`Module_Document_Index.md` 负责把 agent 路由到最少、最相关的上下文。

## 文档列表

| 文档 | 任务 | 推荐模型 |
| --- | --- | --- |
| `A01_ProjectSkeleton.md` | 建立工程目录结构 | GPT-5.4-Mini / GPT-5.4 中 |
| `A02_Infrastructure.md` | EventBus、Logger、TimeProvider 等基础设施 | GPT-5.5 中 |
| `A03_SettingsSystem.md` | AppSettings、SettingsService、本地路径 | GPT-5.5 中 |
| `A04_BootstrapLifecycle.md` | AppBootstrapper、生命周期、服务组装 | GPT-5.5 中 |
| `B01_PetStateMachine.md` | 宠物状态机和优先级 | GPT-5.5 高 |
| `B02_RenderingAnimationBase.md` | 动画接口、动作常量、占位动画 | GPT-5.5 中 |
| `B03_PetBehaviorIntegration.md` | 状态机、动画、行为调度集成 | GPT-5.5 高 |
| `C01_WindowBase.md` | 透明窗口、无边框、置顶、全屏置顶开关 | GPT-5.5 高 |
| `C02_InteractionMovement.md` | 拖动、点击互动、自动行走、屏幕边界 | GPT-5.5 中 |
| `C03_HideMenuTray.md` | 隐藏/显示、鼠标穿透、右键菜单、托盘菜单 | GPT-5.5 中 / GPT-5.4 高 |
| `D01_SkinSystem.md` | 皮肤数据、帧动画、Spine 适配、皮肤选择 UI | GPT-5.5 中；Spine 用 GPT-5.5 高 |
| `D02_SystemCompatibility.md` | 防锁屏、全屏检测、快捷键 | GPT-5.5 高 |
| `E01_ReminderPomodoroHealth.md` | 提醒公共模型、番茄钟、喝水/站立提醒 | GPT-5.5 中 / GPT-5.4 高 |
| `E02_TodoMessageExtensions.md` | 待办 MVP、通讯消息提示接口预留 | GPT-5.5 中 |
| `Q01_TestBuildQA.md` | EditMode/PlayMode 测试、Windows 构建与 QA | GPT-5.4 高 / gpt-5.3-codex |

## 推荐派单顺序

第一批：

- `A01_ProjectSkeleton.md`
- `A02_Infrastructure.md`
- `A03_SettingsSystem.md`

第二批：

- `A04_BootstrapLifecycle.md`
- `B01_PetStateMachine.md`
- `B02_RenderingAnimationBase.md`

第三批：

- `B03_PetBehaviorIntegration.md`
- `C01_WindowBase.md`
- `C02_InteractionMovement.md`

第四批：

- `C03_HideMenuTray.md`
- `D01_SkinSystem.md`
- `D02_SystemCompatibility.md`

第五批：

- `E01_ReminderPomodoroHealth.md`
- `E02_TodoMessageExtensions.md`
- `Q01_TestBuildQA.md`

## 通用交付要求

每个子 agent 完成后必须回复：

- 修改了哪些文件。
- 新增了哪些接口、设置项或资源规范。
- 如何验证。
- 是否运行测试。
- 残余风险和后续建议。
