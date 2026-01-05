using UnityEngine;

public class Chest : MonoBehaviour
{
    public Loot[] lootedItems;
}


[System.Serializable]
public class Loot
{
    public ItemData itemdata;

    [Range(0, 100)]
    public int dropChance;
}
