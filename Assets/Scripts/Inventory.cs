using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    public UnityEvent EvtUpdated { get; } = new();

    private List<Slot> slots = new();

    public IReadOnlyList<Slot> Slots => slots;

    private void Start()
    {
        int slotAmount = GameConfig.Global.InventorySlotAmount;

        Initialize(slotAmount);
        AddDebugItems();
    }

    private void AddDebugItems()
    {
        IReadOnlyList<Item> items = ItemDatabase.Global.Items;

        for (int i = 0; slots.Count > i; i++)
        {
            int index = Random.Range(0, items.Count + 1);
            if (index >= items.Count) continue;

            int amount = Random.Range(1, items[index].MaxStack + 1);

            AddItemToSlot(slots[i], items[index], amount);
        }
    }

    private void Initialize(int slotAmount)
    {
        for (int i = 0; i < slotAmount; i++)
        {
            slots.Add(new(i));
        }
    }

    private Slot GetSlot(int id)
    {
        Assert.IsTrue(id < slots.Count, $"[Inventory, Slot] Invalid slot Id: {id}");
        return slots[id];
    }

    public bool AddItem(Item item, int amount, out int excess)
    {
        int remainder = amount;
        int count = 0;

        while (remainder > 0 && count < 10)
        {
            // Check for same item slot
            Slot slot = slots.Where(s => !s.IsEmpty).FirstOrDefault(s => s.IsSameItem(item) && !s.StackMaxed && !s.IsLocked);

            if (slot == null)
            {
                // Check for empty slot
                slot = slots.FirstOrDefault(s => s.IsEmpty);
            }

            if (slot == null)
            {
                if (remainder == amount)
                {
                    excess = remainder;
                    return false;
                }

                // Partial success.
                break;
            }

            int totalStackAmount = slot.StackAmount + remainder;
            int overflow = Mathf.Max(0, totalStackAmount - item.MaxStack);
            int addAmount = remainder - overflow;

            slot.AddItem(item, addAmount);
            remainder = overflow;

            count++;
        }

        EvtUpdated.Invoke();
        excess = remainder;
        return true;
    }

    public bool AddItemToSlot(Slot slot, Item item, int amount)
    {
        bool success = slot.AddItem(item, amount);
        EvtUpdated.Invoke();
        return success;
    }

    public bool RemoveItemFromSlot(Slot slot, int amount)
    {
        if (slot.IsEmpty) return false;
        bool success = slot.RemoveItem(slot.AssignedItem, amount);
        EvtUpdated.Invoke();
        return success;
    }

    public void ClearSlot(Slot slot)
    {
        if (slot.IsEmpty) return;
        slot.Clear();
        EvtUpdated.Invoke();
    }

    public bool MoveSlotItemToSlot(Slot sourceSlot, Slot targetSlot)
    {
        if (sourceSlot.IsLocked || targetSlot.IsLocked) return false;
        if (sourceSlot.IsEmpty) return false;

        // Move
        if (targetSlot.IsEmpty)
        {
            targetSlot.AddItem(sourceSlot);
            sourceSlot.Clear();
        }
        else
        {
            // Move Stack Amount
            if (sourceSlot.IsSameItem(targetSlot.AssignedItem))
            {
                if (targetSlot.StackMaxed) return false;

                int totalStackAmount = targetSlot.StackAmount + sourceSlot.StackAmount;
                int overflow = Mathf.Max(0, totalStackAmount - targetSlot.MaxStackAmount);
                int moveAmount = sourceSlot.StackAmount - overflow;

                targetSlot.AddItem(sourceSlot.AssignedItem, moveAmount);
                sourceSlot.RemoveItem(sourceSlot.AssignedItem, moveAmount);
            }
            // Switch
            else
            {
                Item tempItem = targetSlot.AssignedItem;
                int tempStackAmount = targetSlot.StackAmount;

                targetSlot.Clear();
                targetSlot.AddItem(sourceSlot);

                sourceSlot.Clear();
                sourceSlot.AddItem(tempItem, tempStackAmount);
            }
        }

        EvtUpdated.Invoke();
        return true;
    }

    public void MoveSlotItemOutside(Slot slot)
    {
        SingletonManager.Get<GameMode>().SpawnItem(slot.AssignedItem, slot.StackAmount);
        slot.Clear();
        EvtUpdated.Invoke();
    }
}
