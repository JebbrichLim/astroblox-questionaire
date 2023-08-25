using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Progress;

public class GameMode : MonoBehaviour
{
    public UnityEvent EvtReady { get; } = new();
    public UnityEvent<Item> EvtSpawnedItem { get; } = new();
    public UnityEvent<Item> EvtDespawningItem { get; } = new();

    public Player Player;

    public bool Ready { get; private set; }

    private void Awake()
    {
        SingletonManager.Register(this);
    }

    private void Start()
    {
        Ready = true;
        EvtReady.Invoke();

        Invoke("AddDebugWorldItems", 0.1f);
    }

    private void AddDebugWorldItems()
    {
        IReadOnlyList<Item> items = ItemDatabase.Global.Items;

        for (int i = 0; items.Count > i; i++)
        {
            int instanceCount = UnityEngine.Random.Range(1, 10);

            for (int j = 0; j < instanceCount; j++)
            {
                int amount = UnityEngine.Random.Range(1, Mathf.RoundToInt(items[i].MaxStack * 2.0f));

                Item newItem = Instantiate(items[i], Vector3.zero, Quaternion.identity);
                newItem.GetComponent<ItemStack>().Setup(amount);

                EvtSpawnedItem.Invoke(newItem);
            }
        }
    }

    public void SpawnItem(Item item, int stackAmount)
    {
        Vector2 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Item newItem = Instantiate(item, new Vector3(spawnPos.x, spawnPos.y, 0.0f), Quaternion.identity);
        newItem.GetComponent<ItemStack>().Setup(stackAmount);

        EvtSpawnedItem.Invoke(newItem);
    }

    public void TakeItem(Item item)
    {
        if (item.gameObject.scene == null) return;

        Item itemData = ItemDatabase.Global.GetItem(item.Id);

        bool success = Player.Inventory.AddItem(itemData, item.Stack.Amount, out int excess);

        if (!success) return;

        item.Stack.RemoveStack(item.Stack.Amount - excess);

        if (item.Stack.Amount == 0)
        {
            EvtDespawningItem.Invoke(item);
            Destroy(item.gameObject);
        }
    }
}
