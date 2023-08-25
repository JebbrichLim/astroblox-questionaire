using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Databases/GameConfig")]
public class GameConfig : ScriptableObject
{
    public static GameConfig Global => ConfigRepository.GetConfig<GameConfig>();

    public int InventorySlotAmount;
}
