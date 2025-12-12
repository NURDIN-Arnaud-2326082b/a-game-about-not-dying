using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Recipes/New Recipe")]
public class RecipeData : ScriptableObject
{
    public ItemData itemToCraft;
    public ItemInInventory[] requiredItems;
}
