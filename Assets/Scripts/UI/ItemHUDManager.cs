using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHUDManager : MonoBehaviour
{
    private Dictionary<Item, ItemHUDElement> itemHUDElements = new();

    private void Start()
    {
        GameMode gameMode = SingletonManager.Get<GameMode>();

        gameMode.EvtSpawnedItem.AddListener(OnSpawnedItem);
        gameMode.EvtDespawningItem.AddListener(OnDespawningItem);
    }

    private void OnSpawnedItem(Item item)
    {
        ItemStack itemStack = item.GetComponent<ItemStack>();
        ItemHUDElement element = itemStack.StackHUD.GetComponent<ItemHUDElement>();
        element.Setup(item.GetComponent<ItemStack>(), element.transform.localPosition);
        element.transform.SetParent(this.transform);

        itemHUDElements.Add(item, element);
    }

    private void OnDespawningItem(Item item)
    {
        itemHUDElements[item].transform.SetParent(item.transform);
        itemHUDElements.Remove(item);
    }
}
