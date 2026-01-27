namespace GameResources.Scripts.UI
{
    using UnityEngine;

    public sealed class HealthProgressBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _fillTransform;

        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            float fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
            Vector3 scale = _fillTransform.localScale;
            scale.x = fillAmount;
            _fillTransform.localScale = scale;
        }

        public void SetVisible(bool visible) => gameObject.SetActive(visible);
    }
}
