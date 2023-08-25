using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ItemStack : MonoBehaviour
{
    [Header("Miscellaneous")]
    public GameObject StackHUD;

    public int Amount { get; private set; }

    public void Setup(int amount)
    {
        Amount = amount;
    }

    public void AddStack(int amount)
    {
        Assert.IsTrue(amount > 0, "[ItemStack] Cannot add invalid amoun.t");

        Amount += amount;
    }

    public void RemoveStack(int amount)
    {
        Assert.IsTrue(amount > 0, "[ItemStack] Cannot remove with invalid amount.");
        Assert.IsTrue(Amount - amount >= 0, "[ItemStack] Cannot remove with invalid amount.");

        Amount -= amount;
    }

    // As seperate incase there would be possible mechanics like vicinity auto merging.
}
