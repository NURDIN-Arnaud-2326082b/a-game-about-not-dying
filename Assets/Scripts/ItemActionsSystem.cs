using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemActionsSystem : MonoBehaviour
{
    [SerializeField]
    private Equipment equipment;

    [SerializeField]
    private PlayerStats playerStats;

    //Référence au panneau d'action d'item
    [Header("Action Panel Reference")]
    [SerializeField]
    public GameObject actionPanel;

    //bouton de laction d'utilisation
    [SerializeField]
    private GameObject useItemButton;

    //bouton de laction d'équiper
    [SerializeField]
    private GameObject equipItemButton;

    //bouton de laction de drop
    [SerializeField]
    private GameObject dropItemButton;

    //bouton de laction de destruction
    [SerializeField]
    private GameObject destroyItemButton;

    //méthode pour ouvrir le panneau d'action
    public void OpenItemActionPanel(ItemData item, UnityEngine.Vector3 slotPosition)
    {
        Inventory.instance.selectedItem = item;
        //vérifier si le slot est vide
        if (item == null)
        {
            CloseItemActionPanel();
            return;
        }
        //configurer les boutons d'action en fonction du type d'item
        switch (item.itemType)
        {
            case ItemType.Ressource:
                useItemButton.SetActive(false);
                equipItemButton.SetActive(false);
                break;
            case ItemType.Equipment:
                useItemButton.SetActive(false);
                equipItemButton.SetActive(true);
                break;
            case ItemType.Consumable:
                useItemButton.SetActive(true);
                equipItemButton.SetActive(false);
                break;
        }
        actionPanel.transform.position = slotPosition;
        actionPanel.SetActive(true);
    }

    //méthode pour fermer le panneau d'action
    public void CloseItemActionPanel()
    {
        actionPanel.SetActive(false);
    }

    //méthode pour le bouton utiliser
    public void UseItemActionButton()
    {
        ItemData current = Inventory.instance.selectedItem;
        playerStats.ConsumeItem(current.healthEffects, current.hungerEffects, current.thirstEffects);
        Inventory.instance.RemoveItem(current);
        CloseItemActionPanel();
        Inventory.instance.selectedItem = null;
    }

    

    //méthode pour le bouton drop item
    public void DropItemActionButton()
    {
        //instancier l'item à dropper au point de drop
        GameObject instantiatedItem = Instantiate(Inventory.instance.selectedItem.prefab);
        instantiatedItem.GetComponent<Item>().itemData = Inventory.instance.selectedItem;
        instantiatedItem.layer = LayerMask.NameToLayer("Item");
        instantiatedItem.transform.position = Inventory.instance.dropPoint.position;
        //retirer l'item de l'inventaire
        Inventory.instance.RemoveItem(Inventory.instance.selectedItem);
        Inventory.instance.RefreshContent();
        CloseItemActionPanel();
        Inventory.instance.selectedItem = null;
    }

    //méthode pour le bouton détruire item
    public void DestroyItemActionButton()
    {
        //retirer l'item de l'inventaire
        Inventory.instance.RemoveItem(Inventory.instance.selectedItem);
        Inventory.instance.RefreshContent();
        CloseItemActionPanel();
        Inventory.instance.selectedItem = null;
    }

    public void EquipItemActionButton()
    {
        equipment.EquipItemActionButton();
        CloseItemActionPanel();
    }   
}
