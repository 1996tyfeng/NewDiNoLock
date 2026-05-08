# B02 动画接口与占位动画任务

## 任务目标

建立皮肤无关的动画播放接口、统一动作名和基础占位动画播放器。

## 推荐模型

GPT-5.5 中或 gpt-5.3-codex。

## 必读文档

- `../DesktopPet_FunctionalDesign.md` 中 6.6
- `../Project_CodeStructure_Guidelines.md` 中 Rendering
- `../WorkBreakdown_Priority_ModelGuide.md` 中 T1.2

## 负责范围

主要目录：

```text
Assets/NewDiNoLock/Scripts/Rendering/
Assets/NewDiNoLock/Prefabs/Pet/
Assets/NewDiNoLock/Art/Skins/
```

建议文件：

```text
IPetAnimationPlayer.cs
AnimationName.cs
PlaceholderAnimationPlayer.cs
```

## 功能要求

- Core 通过 `IPetAnimationPlayer` 播放动作。
- 定义统一动作名：Idle、Walk、Lift、Drop、Click、Happy、Sleep、Notify。
- 占位动画播放器至少支持 Idle、Walk、Lift、Click。
- 缺失动作回退到 Idle。

## 接口建议

```csharp
public interface IPetAnimationPlayer
{
    string CurrentAnimation { get; }
    bool HasAnimation(string animationName);
    void Play(string animationName, bool loop = false);
    void SetFlipX(bool flipX);
    void SetSkin(string skinId);
}
```

## 不要修改

- 不接 Spine 运行库。
- 不做完整皮肤选择 UI。
- 不把状态机逻辑写进动画播放器。

## 验收标准

- 宠物根对象可播放占位动作。
- 没有散落动画字符串。
- 后续 Spine/帧动画可实现同一接口。

## 交付说明

说明接口、动作名、缺失动作回退规则。

