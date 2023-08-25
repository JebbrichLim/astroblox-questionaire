using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent<bool> EvtLeftClick { get; } = new();
    public UnityEvent<bool> EvtRightClick { get; } = new();

    bool rightDown = false;

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            rightDown = true;
            EvtRightClick.Invoke(true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            rightDown = false;
            EvtRightClick.Invoke(false);
        }
    }

    public void OnMouseDown()
    {
        EvtLeftClick.Invoke(true);
    }

    public void OnMouseUp()
    {
        EvtLeftClick.Invoke(false);
    }
}
