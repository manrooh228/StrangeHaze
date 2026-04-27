using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableBullet : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Vector2 startPosition;

    void Awake() { rectTransform = GetComponent<RectTransform>(); 
        //if(!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData) {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
        rectTransform.parent as RectTransform,
        eventData.position,
        eventData.pressEventCamera,
        out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }

        
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if (eventData.pointerEnter == null || !eventData.pointerEnter.CompareTag("RevolverSlot"))
        {
            rectTransform.anchoredPosition = startPosition;
        }
        else
        {
            RevolverSlot slot = eventData.pointerEnter.GetComponent<RevolverSlot>();
            if (slot != null && slot.currentState != RevolverSlot.SlotState.Empty)
            {
                rectTransform.anchoredPosition = startPosition;
            }
        }
    }
}