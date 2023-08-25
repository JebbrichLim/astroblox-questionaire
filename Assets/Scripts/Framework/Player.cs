using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory Inventory { get; private set; }

    private void Awake()
    {
        Inventory = GetComponent<Inventory>();
    }
}
