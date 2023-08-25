using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

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
