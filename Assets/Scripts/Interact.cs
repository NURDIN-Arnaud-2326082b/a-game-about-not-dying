using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interact : MonoBehaviour
{
    //distance permettant au joueur de ramasser un item
    [SerializeField]
    private float interactRange = 2.6f;

    //Référence au script comportment d'interaction
    public InteractBehavior playerInteractBehavior;

    //LayerMask pour ne détecter que les items
    [SerializeField]
    private LayerMask layerMask;

    //Référence au texte d'instruction d'interaction
    [SerializeField]
    private GameObject interactText;

    //Compteur pour afficher le texte une seule fois
    private int cpt;

    // Start is called before the first frame update
    void Start()
    {
        cpt = 0;
        interactText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //On vérifie si le joueur est proche d'un item
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactRange, layerMask))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                    
                //On vérifie que l'objet est bien tagué "Item"
                if (hit.transform.CompareTag("Item"))
                {
                    if (cpt == 0)
                    {
                        interactText.SetActive(true);
                        cpt = 1;
                    }
                    //On appelle la fonction de ramassage
                    playerInteractBehavior.DoPickup(hit.transform.gameObject.GetComponent<Item>());
                    
                }
                else if (hit.transform.CompareTag("Harvestable"))
                {
                    //On appelle la fonction de récolte
                    playerInteractBehavior.DoHarvest(hit.transform.gameObject.GetComponent<Harvestable>());
                    
                }
                else if (hit.transform.CompareTag("Boat"))
                {
                    //On affiche le panneau de réparation du bateau
                    playerInteractBehavior.InteractWithBoat();
                }
                else if (hit.transform.CompareTag("Chest"))
                {
                    //On appelle la fonction d'ouverture du coffre
                    playerInteractBehavior.OpenChest(hit.transform.gameObject.GetComponent<Chest>());
                }
            }
        }
        else
        {
            interactText.SetActive(false); 
        } 

        if (Input.GetKeyDown(KeyCode.P))
        {
            playerInteractBehavior.GivePortalGun();
        }  
    }
}
