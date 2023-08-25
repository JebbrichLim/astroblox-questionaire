using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    [Header("References")]
    public SlotUIElement SlotElementPrefab;
    public InventoryPreviewUIElement PreviewElement;

    private List<SlotUIElement> slotElements = new();

    private GameMode gameMode;
    private SlotUIElement hoveringSlot;
    private SlotUIElement holdingSlot;

    private bool isPointerInsidePanel;

    private Inventory Inventory => gameMode.Player.Inventory;

    private void Start()
    {
        gameMode = SingletonManager.Get<GameMode>();

        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return new WaitUntil(() => gameMode.Ready);

        Inventory.EvtUpdated.AddListener(OnInventoryUpdated);

        int slotAmount = Inventory.Slots.Count;

        for (int i = 0; i < slotAmount; i++)
        {
            SlotUIElement newSlot = Instantiate(SlotElementPrefab, this.transform);
            newSlot.Initialize(Inventory.Slots[i]);

            newSlot.EvtHover.AddListener(OnHoverSlot);
            newSlot.EvtHolding.AddListener(OnHoldingSlot);
            newSlot.EvtRelease.AddListener(OnReleaseSlot);

            slotElements.Add(newSlot);
        }

        PreviewElement.transform.SetParent(this.transform.parent);
    }

    public void OnPointerEnter()
    {
        isPointerInsidePanel = true;
    }

    public void OnPointerExit()
    {
        isPointerInsidePanel = false;
    }

    private void OnInventoryUpdated()
    {
        for (int i = 0;i < slotElements.Count; i++)
        {
            slotElements[i].UpdateElement();
        }
    }

    private void OnHoverSlot(SlotUIElement slot, bool hovered)
    {
        if (hovered) hoveringSlot = slot;
        else hoveringSlot = null;
    }

    private void OnHoldingSlot(SlotUIElement slot)
    {
        holdingSlot = slot;
        PreviewElement.Show(slot.AssignedSlot.AssignedItem);
    }

    private void OnReleaseSlot(SlotUIElement slot)
    {
        PreviewElement.Hide();

        if (!isPointerInsidePanel)
        {
            MoveSlotOutside();
            return;
        }

        MoveSlotItem();
    }

    private void MoveSlotOutside()
    {
        Inventory.MoveSlotItemOutside(holdingSlot.AssignedSlot);
    }

    private void MoveSlotItem()
    {
        if (hoveringSlot == null) return;
        if (holdingSlot == hoveringSlot) return;

        Inventory.MoveSlotItemToSlot(holdingSlot.AssignedSlot, hoveringSlot.AssignedSlot);
    }
}
