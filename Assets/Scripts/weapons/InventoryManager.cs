using System.Collections;
using UnityEngine;

namespace Assets.Scripts.weapons
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance;

        [Header("UI Layout")]
        public GameObject bulletUIPrefab;
        public Transform bagContainer; // Родительский объект, в котором лежат пустые слоты

        [Header("Ammo Data")]
        public int currentAmmoInBag = 0;
        private int maxAmmo = 10; // Максимальное количество слотов

        [Header("References")]
        [SerializeField] private Player player;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            // Обновляем интерфейс при старте игры
            RefreshInventoryUI();
        }

        public void AddAmmo(int amount)
        {
            // Сразу ограничиваем число, чтобы не превысило 10
            if (currentAmmoInBag < maxAmmo)
            {
                currentAmmoInBag += amount;

                // Гарантируем, что не вылезем за пределы 10
                currentAmmoInBag = Mathf.Clamp(currentAmmoInBag, 0, maxAmmo);

                Debug.Log($"Подобрано патронов. Всего в сумке: {currentAmmoInBag}");

                // ВАЖНО: Обновляем UI сразу после изменения данных!
                RefreshInventoryUI();
            }
        }

        public void EquipWeapon()
        {
            player.UnlockWeapon();
        }

        public void RefreshInventoryUI()
        {
            // Проходимся по всем слотам в сумке
            for (int i = 0; i < bagContainer.childCount; i++)
            {
                Transform slot = bagContainer.GetChild(i);

                // Логика: i - это номер текущего слота (0, 1, 2...)
                // Если номер слота меньше кол-ва патронов, в нем должна быть пуля.
                if (i < currentAmmoInBag)
                {
                    // Если слот пустой — добавляем пулю
                    if (slot.childCount == 0)
                    {
                        GameObject newBullet = Instantiate(bulletUIPrefab, slot);

                        // Центруем пулю внутри слота
                        RectTransform rect = newBullet.GetComponent<RectTransform>();
                        if (rect != null) rect.anchoredPosition = Vector2.zero;
                    }
                }
                else
                {
                    // Если патронов меньше, чем этот слот (например, патронов 3, а это 4-й слот)
                    // Удаляем всё, что есть в этом слоте
                    foreach (Transform child in slot)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }
    }
}