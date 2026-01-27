namespace GameResources.Scripts.LevelUpSystem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data.Entities;
    using PauseSystem;
    using Signals;
    using UniRx;
    using Zenject;

    public sealed class LevelUpSystem : IInitializable, IDisposable
    {
        public LevelUpSystem(SignalBus signalBus)
        {
            _signalBus = signalBus;
            GenerateLevelExperience();
        }
        private readonly SignalBus _signalBus;

        private const int MAX_LEVEL = 9999;
        private const int SCALE_FACTOR = 5;
        
        public IReadOnlyReactiveProperty<int> CurrentLevel => _currentLevel;
        public IReadOnlyReactiveProperty<int> CurrentExperience => _currentExperience;
        
        private List<int> _levelExperiences;
        private RewardConfig _rewardConfig;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly ReactiveProperty<int> _currentLevel = new ReactiveProperty<int>(1);
        private readonly ReactiveProperty<int> _currentExperience = new ReactiveProperty<int>(0);

        public void Initialize()
        {
            _signalBus.Subscribe<ExperienceCollectedSignal>(OnExperienceCollected);
            _signalBus.Subscribe<GameConfigLoadSignal>(OnGameConfigLoad);
            
            float progress = GetLevelProgress();
            _signalBus.Fire(new ExperienceProgressChangedSignal(progress, _currentLevel.Value));
        }

        public int GetExperienceForLevel(int level)
        {
            if (level < 1 || level > MAX_LEVEL)
            {
                return 0;
            }

            return _levelExperiences[level - 1];
        }

        public float GetLevelProgress()
        {
            if (_currentLevel.Value >= MAX_LEVEL)
            {
                return 1f;
            }

            int requiredExperience = GetExperienceForLevel(_currentLevel.Value + 1);
            if (requiredExperience == 0)
            {
                return 0f;
            }

            return (float)_currentExperience.Value / requiredExperience;
        }

        private void OnGameConfigLoad(GameConfigLoadSignal signal) => _rewardConfig = signal.GameConfigs.RewardConfig;

        private void OnExperienceCollected(ExperienceCollectedSignal signal) => AddExperience((int)signal.Experience);

        private void AddExperience(int experience)
        {
            _currentExperience.Value += experience;

            while (_currentLevel.Value < MAX_LEVEL && _currentExperience.Value >= GetExperienceForLevel(_currentLevel.Value + 1))
            {
                _currentExperience.Value -= GetExperienceForLevel(_currentLevel.Value + 1);
                _currentLevel.Value++;
                
                OnLevelUp(_currentLevel.Value);
            }

            if (_currentLevel.Value >= MAX_LEVEL)
            {
                _currentExperience.Value = GetExperienceForLevel(MAX_LEVEL);
            }

            float progress = GetLevelProgress();
            _signalBus.Fire(new ExperienceProgressChangedSignal(progress, _currentLevel.Value));
        }

        private void OnLevelUp(int newLevel)
        {
            if (_rewardConfig == null || _rewardConfig.RewardDescriptions == null)
            {
                return;
            }

            RewardDescription rewardDescription = _rewardConfig.RewardDescriptions
                .FirstOrDefault(r => r.PlayerLevel == newLevel);

            if (rewardDescription != null && rewardDescription.EntityTypes != null && rewardDescription.EntityTypes.Count > 0)
            {
                _signalBus.Fire(new LevelUpSignal(rewardDescription));
                ApplicationPauseSystem.Pause();
            }
        }

        private void GenerateLevelExperience()
        {
            _levelExperiences = new List<int>(MAX_LEVEL);

            _levelExperiences.Add(0);

            for (int level = 2; level <= MAX_LEVEL; level++)
            {
                int xp = 100 / SCALE_FACTOR * (level - 1) * (level - 1);
                _levelExperiences.Add(xp);
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
