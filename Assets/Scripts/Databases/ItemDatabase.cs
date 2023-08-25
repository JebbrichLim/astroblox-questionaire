using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Databases/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public static ItemDatabase Global => ConfigRepository.GetConfig<ItemDatabase>();

    [Header("References")]
    [SerializeField] private Item[] items;

    public IReadOnlyList<Item> Items => items;

    public bool IsItemInDatabase(string id)
    {
        return Items.SingleOrDefault(i => i.Id == id) != null;
    }

    public Item GetItem(string id)
    {
        return Items.SingleOrDefault(i => i.Id == id);
    }
}
