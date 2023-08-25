using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class SlotUIElement : MonoBehaviour
{
    public UnityEvent<SlotUIElement, bool> EvtHover { get; } = new();
    public UnityEvent<SlotUIElement> EvtHolding { get; } = new();
    public UnityEvent<SlotUIElement> EvtRelease { get; } = new();

    [Header("References")]
    public Image IconImage;
    public TextMeshProUGUI StackAmountText;
    public GameObject LockImage;

    public Slot AssignedSlot{ get; private set; }
    public bool Holding { get; private set; }

    private Coroutine holdCoroutine = null;

    public void Initialize(Slot slot)
    {
        AssignedSlot = slot;        
        UpdateElement();
    }

    public void Clear()
    {
        IconImage.sprite = null;
        IconImage.gameObject.SetActive(false);
        LockImage.gameObject.SetActive(false);
        StackAmountText.text = string.Empty;
    }

    public void UpdateElement()
    {
        if (AssignedSlot.IsEmpty)
        {
            Clear();
            return;
        }
        
        IconImage.sprite = AssignedSlot.AssignedItem.Icon;
        IconImage.gameObject.SetActive(true);
        LockImage.gameObject.SetActive(AssignedSlot.IsLocked);
        StackAmountText.text = AssignedSlot.AssignedItem.IsStackable ? AssignedSlot.StackAmount.ToString() : string.Empty;
    }

    public void OnPointerEnter()
    {
        EvtHover.Invoke(this, true);
    }

    public void OnPointerExit()
    {
        EvtHover.Invoke(this, false);
    }

    public void OnPointerDown()
    {
        if (AssignedSlot.IsEmpty) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (AssignedSlot.IsLocked) return;

            if (holdCoroutine != null)
                StopCoroutine(holdCoroutine);

            holdCoroutine = StartCoroutine(HoldSequence());
        }
        else if (Input.GetMouseButtonDown(1))
        {
            AssignedSlot.ToggleLock();
            UpdateElement();
        }
    }

    public void OnPointerUp()
    {
        if (holdCoroutine != null)
        {
            StopCoroutine(holdCoroutine);
            holdCoroutine = null;
        }

        if (Holding)
        {
            Holding = false;
            EvtRelease.Invoke(this);
        }
    }

    private IEnumerator HoldSequence()
    {
        yield return new WaitForSecondsRealtime(0.25f);

        Holding = true;
        EvtHolding.Invoke(this);

        holdCoroutine = null;
    }
}
