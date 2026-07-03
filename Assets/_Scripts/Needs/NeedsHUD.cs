using UnityEngine;
using UnityEngine.UI;
using StrangeHaze.Bootstrap;

namespace Assets.Scripts.Needs
{
    /// <summary>
    /// Обновляет UI-полоски стамины/голода/сна.
    /// Повесить на объект Canvas в игровой сцене, перетащить 3 Slider (Min=0, Max=1).
    /// </summary>
    public class NeedsHUD : MonoBehaviour
    {
        [SerializeField] private Slider staminaBar;
        [SerializeField] private Slider hungerBar;
        [SerializeField] private Slider sleepBar;
        [SerializeField] private Slider healthBar;

        private Player _player;

        private void Update()
        {
            var needs = ServiceLocator.Get<IPlayerNeedsService>();

            if (staminaBar) staminaBar.value = needs.Stamina / needs.MaxStamina;
            if (hungerBar) hungerBar.value = needs.Hunger / needs.MaxHunger;
            if (sleepBar) sleepBar.value = needs.Sleep / needs.MaxSleep;

            if (healthBar)
            {
                if (!_player) _player = FindAnyObjectByType<Player>();
                if (_player) healthBar.value = (float)_player.Health / _player.MaxHealth;
            }
        }
    }
}
