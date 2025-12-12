using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvestable : MonoBehaviour
{
    public Tool requiredTool;
    public Resource[] haverstableItems;

    public bool isDisableKinematics;

    public int destroyDelay;
}

[System.Serializable]
public class Resource
{
    public ItemData itemdata;

    [Range(0, 100)]
    public int dropChance;
}

public enum Tool
{
    Axe,
    Pickaxe
}
