using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPreviewUIElement : MonoBehaviour
{
    [Header("References")]
    public Image IconImage;

    private RectTransform previewRect;
    private RectTransform parentRect;

    private void Update()
    {
        SetPosition();
    }

    public void Show(Item item)
    {
        IconImage.sprite = item.Icon;

        SetPosition();
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private void SetPosition()
    {
        if (!previewRect) previewRect = GetComponent<RectTransform>();
        if (!parentRect) parentRect = this.transform.parent.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, Input.mousePosition, Camera.main, out Vector2 newPos);
        previewRect.anchoredPosition = newPos;
    }
}
