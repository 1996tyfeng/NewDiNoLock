namespace NewDiNoLock.Rendering
{
    public interface IPetAnimationPlayer
    {
        string CurrentAnimation { get; }
        bool HasAnimation(string animationName);
        void Play(string animationName, bool loop = false);
        void SetFlipX(bool flipX);
        void SetSkin(string skinId);
    }
}
