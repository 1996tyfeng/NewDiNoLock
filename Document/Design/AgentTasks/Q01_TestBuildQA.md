# Q01 测试、构建与质量收口任务

## 任务目标

建立项目测试基线、手动验证清单和 Windows 构建检查流程。

## 推荐模型

GPT-5.4 高或 gpt-5.3-codex。

## 必读文档

- `../DesktopPet_FunctionalDesign.md` 中 10、13
- `../Project_CodeStructure_Guidelines.md` 中 测试规范、日志规范
- `../WorkBreakdown_Priority_ModelGuide.md` 中 T7.1、T7.2、T7.3

## 负责范围

主要目录：

```text
Assets/NewDiNoLock/Tests/EditMode/
Assets/NewDiNoLock/Tests/PlayMode/
Document/Design/
ProjectSettings/
```

## 测试要求

EditMode 优先覆盖：

- PetStateMachine 状态优先级。
- SettingsService 默认值、保存加载、损坏回退。
- Reminder 调度逻辑。
- Skin 动作回退。

PlayMode 优先覆盖：

- 宠物显示。
- 拖动交互。
- 隐藏/恢复。
- 气泡生命周期。
- 皮肤切换。

## 手动验证清单

每次 MVP 关键变更后检查：

- 应用启动后宠物显示正常。
- 透明背景没有异常色块。
- 置顶开关生效。
- 自动行走不越出屏幕。
- 拖动时播放 Lift，松开后恢复。
- 隐藏/显示可恢复。
- 设置保存后重启仍生效。
- 退出后防锁屏释放。

## 构建要求

- Windows 构建可运行。
- 不依赖 Unity Editor 才能使用核心功能。
- 退出流程正常。
- 日志路径和用户数据路径合理。

## 不要修改

- 不为了测试大规模重构业务代码。
- 不删除现有资源。
- 不把测试数据混入正式资源。

## 验收标准

- Unity Test Runner 可运行新增测试。
- 有清晰手动验证文档。
- 构建问题有记录和复现方式。

## 交付说明

说明新增测试、运行结果、手动验证结果和未覆盖风险。

