using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragBoost : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    GameObject dragabbleIcon;

    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<Button>().onClick.Invoke(); //press button to get boost info

        Sprite sprite = GetComponent<Image>().sprite;
        dragabbleIcon = new GameObject(sprite.name);
        dragabbleIcon.transform.SetParent(transform.parent.parent.parent, false);
        dragabbleIcon.transform.SetAsLastSibling();
        var image = dragabbleIcon.AddComponent<Image>();
        image.sprite = sprite;
        var rectTransform = dragabbleIcon.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(120, 120);
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragabbleIcon.transform.position = Input.mousePosition;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        List<RaycastResult> allElements = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, allElements);

        foreach (var item in allElements)
        {
            if (item.gameObject.GetComponent<EquipButton>() && item.gameObject.GetComponent<BonusButton>())
            {
                item.gameObject.GetComponent<BonusButton>().EquipBonus();
                break;
            }
        }
        if (dragabbleIcon != null)
            Destroy(dragabbleIcon);
    }
}
