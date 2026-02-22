using System.Collections;
using UnityEngine;

namespace Assets.Scripts.weapons
{
    public class InventoryManager : MonoBehaviour
    {

        public static InventoryManager Instance;

        [Header("UI Layout")]
        public GameObject bulletUIPrefab;
        public Transform bagContainer;

        [Header("Ammo Data")]
        public int currentAmmoInBag = 0; // Сколько пуль просто лежит в сумке

        [Header("References")]
        [SerializeField] private Player player; // Ссылка на твой скрипт игрока

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void AddAmmo(int amount)
        {
            if(currentAmmoInBag < 10)
            {
                currentAmmoInBag += amount;
                Debug.Log($"Подобрано патронов: {amount}. Всего в сумке: {currentAmmoInBag}");
            }
        }


        public void EquipWeapon()
        {
            player.UnlockWeapon();
        }

        public void RefreshInventoryUI()
        {
            // 1. Сначала очищаем ПАТРОНЫ внутри слотов, не трогая сами слоты
            foreach (Transform slot in bagContainer)
            {
                // Удаляем все дочерние объекты (пульки) внутри текущего слота
                foreach (Transform bullet in slot)
                {
                    Destroy(bullet.gameObject);
                }
            }

            // 2. Раскладываем текущее количество патронов по слотам
            // Используем Mathf.Min, чтобы не попытаться положить больше пуль, чем у нас есть физических слотов
            int slotsAvailable = bagContainer.childCount;
            int ammoToDisplay = Mathf.Min(currentAmmoInBag, slotsAvailable);

            for (int i = 0; i < ammoToDisplay; i++)
            {
                // Берем i-ый слот по счету
                Transform targetSlot = bagContainer.GetChild(i);

                // Спавним префаб пули как дочерний объект этого слота
                GameObject newBullet = Instantiate(bulletUIPrefab, targetSlot);

                // Сбрасываем позицию в 0, чтобы пуля встала ровно в центр слота
                newBullet.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
        }
    }
}