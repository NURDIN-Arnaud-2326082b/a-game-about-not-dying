using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class InteractBehavior : MonoBehaviour
{
    //Référence au script de déplacement du joueur
    [SerializeField]
    private MoveBehaviour playerMoveBehaviour; 

    //Référence à l'animator du joueur
    [SerializeField]
    private Animator playerAnimator;

    //Référence à l'inventaire du joueur
    [SerializeField]
    private Inventory inventory;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private Equipment equipmentManager;

    [SerializeField]
    private EquipmentLibrary equipmentLibrary;

    [SerializeField]
    private GameObject pickaxeVisual;

    [SerializeField]
    private AudioClip pickaxeSwingSound;

    //Référence à l'item courant ramassé
    private Item currentItem;

    //Référence à l'objet récolté courant
    private Harvestable currentHarvestable;

    private Chest currentChest;

    private Vector3 newItemOffset = new Vector3(0, 0.5f, 0);

    private Tool currentTool;

    [SerializeField]
    private GameObject axeVisual;

    [SerializeField]
    private AudioClip axeSwingSound;

    [SerializeField]
    private AudioClip pickupSound;

    [SerializeField]
    public Boat boat;

    [SerializeField]
    private GameObject portalGunVisual;

    [SerializeField]
    private ItemData portalGunItem;

    public bool isBusy = false;

    public List<String> konamiCode = new List<String>();

    //Fonction de ramassage d'un item
    public void DoPickup(Item item)
    {
        //éviter les actions multiples en même temps
        if (isBusy) return;
        isBusy = true;

        //vérifier si l'inventaire est plein
        if (inventory.IsFull())
        {
            return;
        }

        currentItem = item;
        //jouer animation de ramassage
        playerAnimator.SetTrigger("Pickup");
        //bloquer déplacement du joueur pendant l'animation
        playerMoveBehaviour.canMove = false;
    }

    //Méthode pour ajouter l'item à l'inventaire (appelée par un event dans l'animation)
    public void AddItemToInventory()
    {
        //Ajouter objets ramassés à l'inventaire
        inventory.AddItem(currentItem.itemData);
        //jouer son de ramassage
        audioSource.PlayOneShot(pickupSound);
        //détruire l'objet ramassé
        Destroy(currentItem.gameObject);
        //vider la référence à l'item courant
        currentItem = null;
    }
   
    //Méthode pour réactiver le déplacement du joueur (appelée par un event dans l'animation)
   public void ReEnablePlayerMovement()
   {
        pickaxeVisual.SetActive(false);
        //réactiver le déplacement du joueur
        playerMoveBehaviour.canMove = true;
        EnableToolFromEnum(currentTool, false);
        isBusy = false;
   }

   public void DoHarvest(Harvestable harvestable)
   {
        //éviter les actions multiples en même temps
        if (isBusy) return;
        isBusy = true;

        currentTool = harvestable.requiredTool;
        EnableToolFromEnum(currentTool);
        currentHarvestable = harvestable;
         //jouer animation de récolte
        playerAnimator.SetTrigger("Harvest");
        //bloquer déplacement du joueur pendant l'animation
        playerMoveBehaviour.canMove = false;
   }

    //coroutine pour gérer la récolte (appelée par l'event harvestable dans l'animation)
   IEnumerator BreakHarvestable()
   {
        Harvestable tmpHarvestable = currentHarvestable;
        //désactiver le layer de l'objet récolté pour éviter les collectes multiples avant la disparition de l'objet
        tmpHarvestable.gameObject.layer = LayerMask.NameToLayer("Default");
        if (tmpHarvestable.isDisableKinematics)
        {
            tmpHarvestable.GetComponent<Rigidbody>().isKinematic = false;
            tmpHarvestable.GetComponent<Rigidbody>().AddForce(transform.forward*800, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(tmpHarvestable.destroyDelay);

        for(int i = 0; i < tmpHarvestable.haverstableItems.Length; i++)
        {
            Resource resource = tmpHarvestable.haverstableItems[i];
            
            if (UnityEngine.Random.Range(0, 101) < resource.dropChance)
            {
                GameObject droppedItem = Instantiate(resource.itemdata.prefab);
                droppedItem.transform.position = tmpHarvestable.transform.position + newItemOffset;
            }
            
        }
        //détruire l'objet récolté
        Destroy(tmpHarvestable.gameObject);
        //réactiver le déplacement du joueur
        playerMoveBehaviour.canMove = true;
   }

    public void EnableToolFromEnum(Tool tool, bool enable = true)
    {
         //chercher l'équipement dans la bibliothèque
        EquipmentLibraryItem equipmentToDisable = equipmentLibrary.content.Where(e => e.itemData == equipmentManager.equippedWeaponItem).FirstOrDefault();
        if (equipmentToDisable != null)
        {
            //Réactiver les éléments conflictuels et activer le prefab de l'équipement
            for (int i = 0; i < equipmentToDisable.ElementToDisable.Length; i++)
            {
                equipmentToDisable.ElementToDisable[i].SetActive(enable);
            }
            equipmentToDisable.equipmentPrefab.SetActive(!enable);
        }
        switch (tool)
        {
            case Tool.Axe:
                axeVisual.SetActive(enable);
                audioSource.clip = axeSwingSound;
                break;
            case Tool.Pickaxe:
                pickaxeVisual.SetActive(enable);
                audioSource.clip = pickaxeSwingSound;
                break;
            default:
                pickaxeVisual.SetActive(false);
                axeVisual.SetActive(false);
                break;
        }
    }

    public void PlayHarvestSound()
    {
        audioSource.Play();
    }

    public void InteractWithBoat()
    {
        boat.ShowRepairMaterialPanel();
        boat.checkKeyItem();
        boat.HideRepairMaterialPanel();

    }

    public void OpenChest(Chest chest)
    {
        currentChest = chest;
        //éviter les actions multiples en même temps
        if (isBusy) return;
        isBusy = true;
        //faire apparaître les items lootés
        for (int i = 0; i < currentChest.lootedItems.Length; i++)
        {
            Loot loot = currentChest.lootedItems[i];

            if (UnityEngine.Random.Range(0, 101) < loot.dropChance)
            {
                GameObject droppedItem = Instantiate(loot.itemdata.prefab);
                droppedItem.transform.position = currentChest.transform.position + newItemOffset;
            }

        }
        //détruire le coffre
        Destroy(currentChest.gameObject);
        isBusy = false;
    }

    public void GivePortalGun()
    {
        portalGunVisual.SetActive(true);
        equipmentManager.EquipItemActionButton(portalGunItem);
    }
}
