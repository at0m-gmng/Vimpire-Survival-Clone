namespace GameResources.Scripts.PauseSystem
{
    using UnityEngine;

    public static class ApplicationPauseSystem
    {
        public static bool IsPaused { get; private set; }

        public static void Pause()
        {
            if (IsPaused)
            {
                return;
            }

            IsPaused = true;
            Time.timeScale = 0f;
        }

        public static void Resume()
        {
            if (!IsPaused)
            {
                return;
            }

            IsPaused = false;
            Time.timeScale = 1f;
        }
    }
}
