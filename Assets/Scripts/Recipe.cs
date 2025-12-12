using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Recipe : MonoBehaviour
{
    private RecipeData currentRecipe;
    [SerializeField]
    private Image craftableItemImage;

    [SerializeField]
    private GameObject elementRequiredPrefab;

    [SerializeField]
    private Transform elementRequiredParent;

    [SerializeField]
    private Button craftButton;

    [SerializeField]
    private Sprite canCraftSprite;

    [SerializeField]
    private Sprite cannotCraftSprite;

    [SerializeField]
    private Color missingItemColor;

    [SerializeField]
    private Color availableItemColor;

    public void Configure(RecipeData recipe)
    {
        currentRecipe = recipe;

        bool canCraft = true;

        craftableItemImage.sprite = recipe.itemToCraft.visual;

       for (int i = 0; i < recipe.requiredItems.Length; i++)
        {
            // Récupère tous les éléments nécessaires pour cette recette
            GameObject requiredItemGO = Instantiate(elementRequiredPrefab, elementRequiredParent);
            Image requiredItemGOImage = requiredItemGO.GetComponent<Image>();
            ItemData requiredItem = recipe.requiredItems[i].itemData;
            ElementRequired elementRequired = requiredItemGO.GetComponent<ElementRequired>();
         
            // Si l'inventaire contient l'élément requis on le retire de l'inventaire et on passe au suivant
            ItemInInventory[] itemInInventory = Inventory.instance.GetContent().Where(elem => elem.itemData == requiredItem).ToArray();

            int quantityInInventory = 0;
            for (int j = 0; j < itemInInventory.Length; j++)
            {
                quantityInInventory += itemInInventory[j].quantity;
            }

            if (quantityInInventory >= recipe.requiredItems[i].quantity)
            {
                requiredItemGOImage.color = availableItemColor;
            }
            else
            {
                requiredItemGOImage.color = missingItemColor;
                canCraft = false;
            }
            //Configurer l'élément requis
            elementRequired.elementImage.sprite = recipe.requiredItems[i].itemData.visual;
            elementRequired.elementCountText.text = recipe.requiredItems[i].quantity.ToString();
        }


        //mettre à jour le bouton de craft en fonction de si on peut crafter ou pas
        if (canCraft)
        {
            craftButton.image.sprite = canCraftSprite;
            craftButton.enabled = true;
        }
        else
        {
            craftButton.image.sprite = cannotCraftSprite;
            craftButton.enabled = false;
        }

        ResizeElementRequiredParent();
    }

    //méthode pour redimensionner le parent des éléments requis (bug UI) c'est pas optimal mais ça marche pour l'instant
    public void ResizeElementRequiredParent()
    {
        Canvas.ForceUpdateCanvases();
        elementRequiredParent.GetComponent<ContentSizeFitter>().enabled = false;
        elementRequiredParent.GetComponent<ContentSizeFitter>().enabled = true;
    }

    public void CraftItem()
    {
        Inventory.instance.AddItem(currentRecipe.itemToCraft);
        foreach (ItemInInventory item in currentRecipe.requiredItems)
        {
            for (int i = 0; i < item.quantity; i++)
            {
                Inventory.instance.RemoveItem(item.itemData);
            }
        }
    }
}
