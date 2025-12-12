using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/new Item")]
public class ItemData : ScriptableObject
{
    [Header("Data")]
    //Nom de l'item
    public new string name;

    //Sprite de l'item
    public Sprite visual;

    //Prefab de l'item
    public GameObject prefab;

    //Description de l'item
    public string description;

     //bool indiquant si l'item est empilable
    public bool isStackable;

    //Quantité maximale dans une pile (si empilable)
    public int maxStack;

    [Header("Type")]
    //Type de l'item
    public ItemType itemType;

    //Type d'équipement (si applicable)
    public EquipmentType equipmentType;

    [Header("Effects")]
    //Valeur de soin (si consommable)
    public float healthEffects;
    //Valeur de restauration de faim (si consommable)
    public float hungerEffects;
    //Valeur de restauration de soif (si consommable)
    public float thirstEffects;

    [Header("Armor Stats")]
    //Points d'armure (si équipement)
    public float armorPoints;

    [Header("Weapon Stats")]
    //Dégâts de l'arme (si équipement)
    public float weaponDamage;

    public bool isKeyItem;

    public bool isPortalGun;
}

//Enum pour les types d'items
public enum ItemType
{
    Ressource,
    Equipment,
    Consumable
}

//Enum pour les types d'équipements
public enum EquipmentType
{
    Head,
    Chest,
    Hands,
    Legs,
    Feet,
    Weapon
}