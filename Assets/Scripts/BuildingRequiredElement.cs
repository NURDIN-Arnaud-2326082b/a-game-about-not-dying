using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BuildingRequiredElement : MonoBehaviour
{
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private Text costText;

    [SerializeField]
    private Image slotImage;

    [SerializeField]
    private Color greenColor;

    [SerializeField]
    private Color redColor;

    public bool hasResources;

    public void SetUp(ItemInInventory item)
    {
        iconImage.sprite = item.itemData.visual;
        costText.text = item.quantity.ToString();
        ItemInInventory[] itemInInventory = Inventory.instance.GetContent().Where(i => i.itemData == item.itemData).ToArray();

        int totalQuantity = 0;

        for (int i = 0; i < itemInInventory.Length; i++)
        {
            totalQuantity += itemInInventory[i].quantity;
        }

        if (totalQuantity >= item.quantity)
        {
            slotImage.color = greenColor;
            hasResources = true;
        }
        else
        {
            slotImage.color = redColor;
            hasResources = false;
        }
    }
}