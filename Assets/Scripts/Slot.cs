using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class Slot
{
    public int Id { get; private set; }
    public Item AssignedItem { get; private set; }
    public int StackAmount { get; private set; }
    public bool IsLocked { get; private set; }

    public int MaxStackAmount => AssignedItem.MaxStack;
    public bool StackMaxed => StackAmount >= AssignedItem.MaxStack;
    public bool IsEmpty => AssignedItem == null;

    public Slot(int id)
    {
        Id = id;
    }

    public bool Lock()
    {
        if (IsEmpty)
        {
            IsLocked = false;
            return false;
        }

        IsLocked = true;
        return true;
    }

    public void Unlock()
    {
        IsLocked = false;
    }

    public void ToggleLock()
    {
        if (IsLocked) Unlock();
        else Lock();
    }

    public bool IsSameItem(Item item)
    {
        return item.Id == AssignedItem.Id;
    }

    public bool AddItem(Item item, int amount = 1)
    {
        if (IsLocked) return false;

        if (!IsEmpty)
        {
            if (AssignedItem.Id != item.Id) return false;
            if (StackMaxed) return false;
        }

        Assert.IsTrue(amount >= 0, $"[Slot] Cannot add negative amount of items: {item.Id}");

        int newStackAmount = StackAmount + amount;

        if (newStackAmount > item.MaxStack)
        {
            Debug.LogWarning($"[Slot] Cannot add beyond max amount of items: {item.Id}; max: {item.MaxStack}; trying to add: {amount}; total amount: {newStackAmount}");
            return false;
        }

        if (IsEmpty) AssignedItem = item;

        StackAmount += amount;

        return true;
    }

    public bool AddItem(Slot slot)
    {
        Assert.IsTrue(slot.AssignedItem, "[Slot] Invalid slot add item");
        return AddItem(slot.AssignedItem, slot.StackAmount);
    }

    public bool RemoveItem(Item item, int amount = 1)
    {
        Assert.IsFalse(IsEmpty, $"[Slot] Cannot remove item from empty slot: {item.Id}");
        if (IsLocked) return false;
        if (item.Id != AssignedItem.Id) return false;

        Assert.IsTrue(amount <= StackAmount, $"[Slot] Cannot remove more than current stack amount: {item.Id}");
        StackAmount -= amount;

        Assert.IsTrue(StackAmount >= 0, "[Slot] Cannot have negative stack amount");
        if (StackAmount == 0)
        {
            AssignedItem = null;
            IsLocked = false;
        }

        return true;
    }

    public void Clear()
    {
        AssignedItem = null;
        StackAmount = 0;
        IsLocked = false;
    }
}