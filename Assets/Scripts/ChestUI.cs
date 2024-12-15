using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChestUI : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<StorableType, UIItem> ItemsSlots = new();
    [SerializeField] private Chest chest;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private EventSystem eventSystem;

    private void Start()
    {
        chest.OnStore.AddListener(AddToUI);
        chest.OnRemove.AddListener(RemoveFromUI);
    }

    private void OnDestroy()
    {
        chest.OnStore.RemoveListener(AddToUI);
        chest.OnRemove.RemoveListener(RemoveFromUI);
    }

    private void AddToUI(IStorable item)
    {
        var type = item.GetAttributes.type;
        if (ItemsSlots.ContainsKey(type))
        {
            ItemsSlots[type].SetEnabled();
        }
    }

    private void RemoveFromUI(IStorable item)
    {
        var type = item.GetAttributes.type;
        if (ItemsSlots.ContainsKey(type) && chest.Storables[type].Count < 1)
        {
            ItemsSlots[type].SetDisabled();
        }
    }

    public void ButtonClicked(StorableType type)
    {
        Debug.Log(type);
    }

    public void HandleInput() //Check for UI click
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        PointerEventData pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        graphicRaycaster.Raycast(pointerEventData, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach (RaycastResult result in raycastResults)
            {
                UIItem item;
                if (result.gameObject.TryGetComponent(out item))
                {
                    chest.RemoveItemByType(item.storableType);
                    return;
                }
            }
        }

    }
}
