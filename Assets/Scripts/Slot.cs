using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //Référence à l'item dans le slot
    public ItemData item;

    //Référence à l'image de l'item
    public Image itemVisual;

    //Référence au système d'équipements
    [SerializeField]
    private Equipment equipment;

    //Référence au système d'actions d'item
    [SerializeField]
    private ItemActionsSystem itemActionsSystem;

    public Text countText;

    //Méthode pour afficher la description de l'item au survol
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            ToolTipSystem.instance.Show(item.description, item.name);
        }
    }

    //Méthode pour cacher la description de l'item quand la souris quitte le slot
    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipSystem.instance.Hide();
    }

    //Méthode pour gérer le clic sur le slot d'item
    public void CilckOnItemSlot()
    {
        itemActionsSystem.OpenItemActionPanel(item, transform.position);
    }

    //Méthode pour gérer le clic sur le slot d'équipement
    public void ClickOnEquipmentSlot()
    {
        equipment.OpenEquipmentActionPanel(item, transform.position);
    }
}
