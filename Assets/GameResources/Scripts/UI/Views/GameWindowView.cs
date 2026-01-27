namespace GameResources.Scripts.UI.Views
{
    using System.Collections.Generic;
    using Data;
    using PauseSystem;
    using Signals;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public sealed class GameWindowView : MonoBehaviour
    {
        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
            _signalBus.Subscribe<ExperienceProgressChangedSignal>(OnExperienceProgressChanged);
            _signalBus.Subscribe<LevelUpSignal>(OnLevelUp);
            _signalBus.Subscribe<RewardSelectedSignal>(OnRewardSelected);
        }
        private SignalBus _signalBus;

        private const string LEVEL_FORMAT = "LEVEL {0}";

        [SerializeField] private Image _experienceImage = default;
        [SerializeField] private Text _playerLevel = default;
        [SerializeField] private GameObject _rewardPanel = default;
        [SerializeField] private Transform _rewardCardsContainer = default;
        [SerializeField] private Button _rewardCardPrefab = default;

        private List<Button> _activeRewardCards = new List<Button>();

        private void OnExperienceProgressChanged(ExperienceProgressChangedSignal signal)
        {
            if (_experienceImage != null)
            {
                Vector3 scale = _experienceImage.transform.localScale;
                scale.x = signal.Progress;
                _experienceImage.transform.localScale = scale;
            }

            if (_playerLevel != null)
            {
                _playerLevel.text = string.Format(LEVEL_FORMAT, signal.Level);
            }
        }

        private void OnLevelUp(LevelUpSignal signal)
        {
            if (signal.RewardDescription == null || signal.RewardDescription.EntityTypes == null)
            {
                return;
            }

            ClearRewardCards();

            int cardCount = Mathf.Min(signal.RewardDescription.EntityTypes.Count, 3);
            for (int i = 0; i < cardCount; i++)
            {
                CreateRewardCard(signal.RewardDescription.EntityTypes[i]);
            }

            if (_rewardPanel != null)
            {
                _rewardPanel.SetActive(true);
            }
        }

        private void CreateRewardCard(EntityType entityType)
        {
            if (_rewardCardPrefab == null || _rewardCardsContainer == null)
            {
                return;
            }

            Button card = Instantiate(_rewardCardPrefab, _rewardCardsContainer);
            Text cardText = card.GetComponentInChildren<Text>();
            if (cardText != null)
            {
                cardText.text = entityType.ToString();
            }

            card.onClick.AddListener(() => OnRewardCardClicked(entityType));

            _activeRewardCards.Add(card);
        }

        private void OnRewardCardClicked(EntityType entityType) => _signalBus.Fire(new RewardSelectedSignal(entityType));

        private void OnRewardSelected(RewardSelectedSignal signal)
        {
            ApplicationPauseSystem.Resume();

            if (_rewardPanel != null)
            {
                _rewardPanel.SetActive(false);
            }

            ClearRewardCards();
        }

        private void ClearRewardCards()
        {
            foreach (Button card in _activeRewardCards)
            {
                if (card != null)
                {
                    card.onClick.RemoveAllListeners();
                    Destroy(card.gameObject);
                }
            }
            _activeRewardCards.Clear();
        }

        private void OnDestroy()
        {
            _signalBus.TryUnsubscribe<ExperienceProgressChangedSignal>(OnExperienceProgressChanged);
            _signalBus.TryUnsubscribe<LevelUpSignal>(OnLevelUp);
            _signalBus.TryUnsubscribe<RewardSelectedSignal>(OnRewardSelected);
            ClearRewardCards();
        }
    }
}
