using UnityEngine;
using UnityEngine.UI;

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
    private Text interactText;

    // Start is called before the first frame update
    void Start()
    {
        interactText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //On vérifie si le joueur est proche d'un item
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactRange, layerMask))
        {
            //On vérifie le type d'objet face au joueur pour afficher le texte d'instruction approprié
            if (hit.transform.CompareTag("Item"))
            {
                interactText.text = "Press E to pick up";
                interactText.gameObject.SetActive(true);
            }
            else if (hit.transform.CompareTag("Harvestable"))
            {
                interactText.text = "Press E to harvest";
                interactText.gameObject.SetActive(true);
            }
            else if (hit.transform.CompareTag("Boat"))
            {
                interactText.text = "Press E to interact with the boat";
                interactText.gameObject.SetActive(true);
            }
            else if (hit.transform.CompareTag("Chest"))
            {
                interactText.text = "Press E to open the chest";
                interactText.gameObject.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                    
                //On vérifie que l'objet est bien tagué "Item"
                if (hit.transform.CompareTag("Item"))
                {
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
            interactText.gameObject.SetActive(false); 
        } 

        if (Input.GetKeyDown(KeyCode.P))
        {
            playerInteractBehavior.GivePortalGun();
        }  
    }
}
