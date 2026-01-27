namespace GameResources.Scripts.Signals
{
    public sealed class ExperienceProgressChangedSignal
    {
        public ExperienceProgressChangedSignal(float progress, int level)
        {
            Progress = progress;
            Level = level;
        }

        public readonly float Progress;
        public readonly int Level;
    }
}
