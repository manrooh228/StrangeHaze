using UnityEngine;
using StrangeHaze.Bootstrap;

namespace Assets.Scripts.Needs
{
    /// <summary>
    /// Двигает тикер IPlayerNeedsService каждый кадр.
    /// Повесить на Player в каждой игровой сцене (Level1, Level2 и т.п.),
    /// НЕ нужен в MainMenu/house — там время выживания не должно идти.
    /// </summary>
    public class PlayerNeedsTicker : MonoBehaviour
    {
        private void Update()
        {
            ServiceLocator.Get<IPlayerNeedsService>().Tick(Time.deltaTime);
        }
    }
}
