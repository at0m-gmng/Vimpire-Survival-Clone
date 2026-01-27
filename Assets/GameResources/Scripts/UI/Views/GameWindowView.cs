namespace GameResources.Scripts.UI.Views
{
    using Signals;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class GameWindowView : MonoBehaviour
    {
        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
            _signalBus.Subscribe<ExperienceProgressChangedSignal>(OnExperienceProgressChanged);

        }
        private SignalBus _signalBus;

        private const string LEVEL_FORMAT = "LEVEL {0}";

        [SerializeField] private Image _experienceImage = default;
        [SerializeField] private Text _playerLevel = default;

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

        private void OnDestroy() => _signalBus.TryUnsubscribe<ExperienceProgressChangedSignal>(OnExperienceProgressChanged);
    }
}
