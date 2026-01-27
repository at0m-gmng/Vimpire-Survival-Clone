namespace GameResources.Scripts.Signals
{
    public sealed class ExperienceCollectedSignal
    {
        public ExperienceCollectedSignal(float experience)
        {
            Experience = experience;
        }
        public readonly float Experience;
    }
}