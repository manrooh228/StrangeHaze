using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.weapons;

public class RevolverSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public enum SlotState { Empty, Loaded, Spent }
    public SlotState currentState = SlotState.Empty;

    public Sprite emptySprite;  // Дырка
    public Sprite fullSprite;   // Целый патрон
    public Sprite spentSprite;  // Стреляная гильза

    private Image _img;

    void Awake() => _img = GetComponent<Image>();

    // Срабатывает, когда мы отпускаем пулю НАД этим слотом
    public void OnDrop(PointerEventData eventData)
    {
        if (currentState == SlotState.Empty && eventData.pointerDrag != null)
        {
            DraggableBullet bullet = eventData.pointerDrag.GetComponent<DraggableBullet>();
            if (bullet != null)
            {
                LoadBullet();
                Destroy(eventData.pointerDrag); // Удаляем пулю из "руки" (инвентаря)
            }
        }
    }

    public void LoadBullet()
    {
        InventoryManager.Instance.currentAmmoInBag--;
        currentState = SlotState.Loaded;
        _img.sprite = fullSprite;
    }

    public void FireBullet()
    {
        if (currentState == SlotState.Loaded)
        {
            currentState = SlotState.Spent;
            _img.sprite = spentSprite;
        }
    }

    // Клик мышкой для извлечения гильзы
    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentState == SlotState.Spent)
        {
            currentState = SlotState.Empty;
            _img.sprite = emptySprite;
            Debug.Log("Гильза выброшена!");
        }
    }
}