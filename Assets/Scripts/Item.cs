using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class Item : MonoBehaviour
{
    [Header("Configuration")]
    public string Id;
    public string Name;
    [TextArea] public string Description;
    public int Cost;

    [Header("References")]
    public Sprite Icon;
    public GameObject ActorObject;

    public bool IsStackable;
    [SerializeField] private int maxStack;

    public int MaxStack
    {
        get
        {
            if (!IsStackable) return 1;
            return Mathf.Max(1, maxStack);
        }
    }

    public ItemStack Stack { get; private set; }
    public Rigidbody2D Rigidbody { get; private set; }

    private Coroutine updateCoroutine;

    private void Awake()
    {
        Stack = GetComponent<ItemStack>();
        Rigidbody = GetComponent<Rigidbody2D>();

        Interactable interactable = GetComponent<Interactable>();
        interactable.EvtRightClick.AddListener(OnRightClicked);
        interactable.EvtLeftClick.AddListener(OnLeftClicked);
    }

    private void OnRightClicked(bool buttonDown)
    {
        if (buttonDown) return;
        SingletonManager.Get<GameMode>().TakeItem(this);
    }

    private void OnLeftClicked(bool buttonDown)
    {
        if (buttonDown)
        {
            updateCoroutine = StartCoroutine(FollowPointerSequence());
        }
        else
        {
            StopCoroutine(updateCoroutine);
        }
    }

    private IEnumerator FollowPointerSequence()
    {
        while (true)
        {
            Vector2 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Rigidbody.MovePosition(newPos);
            yield return null;
        }
    }
}
