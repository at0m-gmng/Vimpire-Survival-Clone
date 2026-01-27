namespace GameResources.Scripts.Signals
{
    public class ExperienceCollectedSignal
    {
        public ExperienceCollectedSignal(float experience)
        {
            Experience = experience;
        }
        public readonly float Experience;
    }
}