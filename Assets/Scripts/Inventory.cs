using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using Unity.VisualScripting.ReorderableList;

public class Inventory : MonoBehaviour
{
    //référence au système d'équipements
    [SerializeField]
    private Equipment equipment;

    //référence au item actons system
    [SerializeField]
    private ItemActionsSystem itemActionsSystem;

    [SerializeField]
    private CraftingSystem craftingSystem;

    [Header("Inventory Panel Reference")]
    //Contenu de l'inventaire
    [SerializeField]
    private List<ItemInInventory> content = new List<ItemInInventory>();

    //Référence aux slots de l'inventaire
    [SerializeField]
    private Transform inventorySlotsParent;

    //Référence au panel de l'inventaire
    [SerializeField]
    private GameObject inventoryPanel;

    //Taille maximale de l'inventaire
    const int inventorySize = 32;

    //Point de drop des items
    [SerializeField]
    public Transform dropPoint;
    //Item sélectionné pour les actions
    public ItemData selectedItem;

    //Sprite pour un slot vide
    [SerializeField]
    public Sprite emptySlotVisual;

    //Singleton instance de l'inventaire
    public static Inventory instance;

    [SerializeField]
    private KeyCode openInventoryInput;

    [SerializeField]
    private BuildSystem buildSystem;

    //Initialisation du singleton
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    public void Start()
    {
        //cacher l'inventaire au démarrage
        inventoryPanel.SetActive(false);
        //cacher le panneau d'action au démarrage
        itemActionsSystem.actionPanel.SetActive(false);
        //cacher le panneau d'action équipement au démarrage
        equipment.actionEquipmentPanel.SetActive(false);

        //rafraîchir l'affichage de l'inventaire
        RefreshContent();
    }

    //Ajouter un item à l'inventaire
    public void AddItem(ItemData item)
    {
        //rechercher si l'item est déjà dans l'inventaire
        ItemInInventory[] existingItem = content.Where(i => i.itemData == item).ToArray();
        bool itemAdded = false;
        if (existingItem.Length > 0 && item.isStackable)
        {
            for (int i = 0; i < existingItem.Length; i++)
            {
                if (existingItem[i].quantity < item.maxStack)
                {
                    existingItem[i].quantity++;
                    itemAdded = true;
                    break;
                }
            }
            if (!itemAdded)
            {
                content.Add(new ItemInInventory { itemData = item, quantity = 1 });
            }
            
        }
        else
        {
            content.Add(new ItemInInventory { itemData = item, quantity = 1 });
        }
        //rafraîchir l'affichage de l'inventaire
        RefreshContent();
        //message pour débuguer
        Debug.Log("Item added to inventory: " + item.name);
    }

    //Retirer un item à l'inventaire
    public void RemoveItem(ItemData item)
    {
        //rechercher si l'item est déjà dans l'inventaire
        ItemInInventory existingItem = content.Where(i => i.itemData == item).FirstOrDefault();
        if (existingItem != null  &&  existingItem.quantity > 1)
        {
            existingItem.quantity--;
            
        }
        else
        {
            content.Remove(existingItem);
        }
        //rafraîchir l'affichage de l'inventaire
        RefreshContent();
        //message pour débuguer
        Debug.Log("Item removed from inventory: " + item.name);
    } 

    // Update is called once per frame
    public void Update()
    {
        //afficher/cacher l'inventaire avec la touche I
        if (Input.GetKeyDown(openInventoryInput))
        {
            if (inventoryPanel.activeSelf)
            {
                itemActionsSystem.CloseItemActionPanel();
                //cacher le tooltip si l'inventaire est fermé
                ToolTipSystem.instance.Hide();
                //cacher le panneau d'action équipement si l'inventaire est fermé
                equipment.CloseEquipmentActionPanel();
            }
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }

    //rafraîchir l'affichage de l'inventaire
    public void RefreshContent()
    {
        //vider tous les slots de l'inventaire
        for (int i = 0; i < inventorySlotsParent.childCount; i++)
        {
            Slot currentSlot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();
            currentSlot.item = null;
            currentSlot.itemVisual.sprite = emptySlotVisual;
            currentSlot.countText.enabled = false ;
        }
        //vider tous les slots de l'équipement
        for (int i = 0; i < equipment.equipmentSlotsParent.childCount; i++)
        {
            Slot currentSlot = equipment.equipmentSlotsParent.GetChild(i).GetComponent<Slot>();
            currentSlot.item = null;
            currentSlot.itemVisual.sprite = emptySlotVisual;
        }
        //rafraîchir le contenu de l'inventaire en le remplissant
        for (int i = 0; i < content.Count; i++)
        {
            Slot currentSlot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();
            currentSlot.item = content[i].itemData;
            currentSlot.itemVisual.sprite = content[i].itemData.visual;
            if (content[i].itemData.isStackable)
            {
                currentSlot.countText.enabled = true;
                currentSlot.countText.text = content[i].quantity.ToString();
            }
        }
        //rafraîchir le contenu de l'équipement en le remplissant
        if (equipment.equippedHeadItem != null)
        {
            Slot headSlot = equipment.equipmentSlotsParent.GetChild(0).GetComponent<Slot>();
            headSlot.item = equipment.equippedHeadItem;
            headSlot.itemVisual.sprite = equipment.equippedHeadItem.visual;
        }
        if (equipment.equippedChestItem != null)
        {
            Slot chestSlot = equipment.equipmentSlotsParent.GetChild(1).GetComponent<Slot>();
            chestSlot.item = equipment.equippedChestItem;
            chestSlot.itemVisual.sprite = equipment.equippedChestItem.visual;
        }
        if (equipment.equippedHandsItem != null)
        {
            Slot handsSlot = equipment.equipmentSlotsParent.GetChild(2).GetComponent<Slot>();
            handsSlot.item = equipment.equippedHandsItem;
            handsSlot.itemVisual.sprite = equipment.equippedHandsItem.visual;
        }
        if (equipment.equippedLegsItem != null)
        {
            Slot legsSlot = equipment.equipmentSlotsParent.GetChild(3).GetComponent<Slot>();
            legsSlot.item = equipment.equippedLegsItem;
            legsSlot.itemVisual.sprite = equipment.equippedLegsItem.visual;
        }
        if (equipment.equippedFeetItem != null)
        {
            Slot feetSlot = equipment.equipmentSlotsParent.GetChild(4).GetComponent<Slot>();
            feetSlot.item = equipment.equippedFeetItem;
            feetSlot.itemVisual.sprite = equipment.equippedFeetItem.visual;
        }
        if (equipment.equippedWeaponItem != null)
        {
            Slot weaponSlot = equipment.equipmentSlotsParent.GetChild(5).GetComponent<Slot>();
            weaponSlot.item = equipment.equippedWeaponItem;
            weaponSlot.itemVisual.sprite = equipment.equippedWeaponItem.visual;
        }
        //mettre à jour l'affichage des recettes dans le crafting system
        craftingSystem.UpdateDisplayRecipes();
        buildSystem.UpdateCosts();
    }

    //méthode pour vérifier si l'inventaire est plein
    public bool IsFull()
    {
        //vérifier si l'inventaire est plein
        return content.Count >= inventorySize;
    }

    //méthode pour obtenir le contenu de l'inventaire
    public List<ItemInInventory> GetContent()
    {
        return content;
    }

    public void LoadContent(List<ItemInInventory> items)
    {
        content = items;
        RefreshContent();
    }

    public void ClearInventory()
    {
        content.Clear();
        RefreshContent();
    }

    public int CheckForKeyItem()
    {
        //comptage des items clés dans l'inventaire
        int keyItemCount = content.Where(i => i.itemData.isKeyItem).Sum(i => i.quantity);
        Debug.Log("Number of key items in inventory: " + keyItemCount);
        return keyItemCount;
    }

}

[System.Serializable]
public class ItemInInventory
{
    public ItemData itemData;
    public int quantity;
}
