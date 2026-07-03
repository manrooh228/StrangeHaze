using UnityEngine;
using StrangeHaze.Bootstrap;

namespace Assets.Scripts.Needs
{
    /// <summary>
    /// Кровать/место отдыха. Повесить на объект в сцене "house" с Collider2D (IsTrigger = true).
    /// Игрок заходит в триггер, жмёт E — стамина и сон восстанавливаются.
    /// Требует, чтобы GameObject игрока имел тег "Player" (как и остальные триггеры в проекте).
    /// </summary>
    public class RestBed : MonoBehaviour
    {
        [SerializeField] private GameObject promptUI;

        private bool _canRest;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            _canRest = true;
            if (promptUI) promptUI.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            _canRest = false;
            if (promptUI) promptUI.SetActive(false);
        }

        private void Update()
        {
            if (_canRest && Input.GetKeyDown(KeyCode.E))
            {
                ServiceLocator.Get<IPlayerNeedsService>().RestAtHome();
            }
        }
    }
}
