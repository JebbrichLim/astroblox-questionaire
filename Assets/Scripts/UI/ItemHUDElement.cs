using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemHUDElement : MonoBehaviour
{
    [Header("References")]
    public Canvas Canvas;
    public TextMeshProUGUI StackAmountText;

    private ItemStack stack;
    private Vector3 offset;

    public void Setup(ItemStack stack, Vector3 offset)
    {
        this.stack = stack;
        this.offset = offset;
        Canvas.worldCamera = Camera.main;
    }

    private void Update()
    {
        this.transform.position = stack.transform.position + offset;
        StackAmountText.text = stack.Amount <= 1 ? string.Empty : $"({stack.Amount})";
    }
}
