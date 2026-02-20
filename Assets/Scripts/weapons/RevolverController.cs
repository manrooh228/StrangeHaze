using UnityEngine;

public class RevolverController : MonoBehaviour
{
    public RevolverSlot[] slots; // Твои 6 UI-слотов
    private int _currentSlotIndex = 0;

    public bool RequestShot()
    {
        foreach (var slot in slots)
        {
            if (slot.currentState == RevolverSlot.SlotState.Loaded)
            {
                slot.currentState = RevolverSlot.SlotState.Spent;
                slot.GetComponent<UnityEngine.UI.Image>().sprite = slot.spentSprite;

                Debug.Log("Выстрел из доступного слота!");
                return true;
            }
        }
        Debug.Log("Все слоты пусты или в них гильзы!");
        return false;
    }
}
