using UnityEngine;
using UnityEngine.UI;

public class Boat : MonoBehaviour
{
    // panel de matériau nécessaire pour réparer le bateau
    public GameObject repairMaterialPanel;

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

    [SerializeField]
    private Transform player;

    [SerializeField]
    private GameObject boatRepaired;

    [SerializeField]
    private Text winText;

    [SerializeField]
    private MoveBehaviour playerMoveBehaviour;

    // singleton instance
    public static Boat instance;

    private void Awake()
    {
        instance = this;
    }
    
    // méthode pour afficher le panneau de matériau de réparation
    public void ShowRepairMaterialPanel()
    {
        repairMaterialPanel.SetActive(true);
    }
    // méthode pour cacher le panneau de matériau de réparation
    public void HideRepairMaterialPanel()
    {
        repairMaterialPanel.SetActive(false);
    }

    // méthode pour changer l'état du bouton de fabrication
    public void UpdateCraftButton(bool canCraft)
    {
        if (canCraft)
        {
            craftButton.image.sprite = canCraftSprite;
            craftButton.interactable = true;
        }
        else
        {
            craftButton.image.sprite = cannotCraftSprite;
            craftButton.interactable = false;
        }
    }

    public void checkKeyItem()
    {
        int nb = Inventory.instance.CheckForKeyItem();
        if (nb >= 5)
        {
            UpdateCraftButton(true);
        }
        else
        {
            UpdateCraftButton(false);
        }
    }

    public void RepairBoat()
    {
        checkKeyItem();
        player.position = boatRepaired.transform.position + new Vector3(0, 30, 0);
        HideRepairMaterialPanel();
        ShowWinScreen();
    }

    public void ShowWinScreen()
    {
        winText.gameObject.SetActive(true);
        playerMoveBehaviour.canMove = false;
    }
}
