using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private PlayerStats playerStats;

    [SerializeField]
    private Equipment equipment;

    [SerializeField]
    private BuildSystem buildSystem;

    [Header("Configuration")]
    [SerializeField]
    private KeyCode saveKey;

    [SerializeField]
    private KeyCode loadKey;

    [SerializeField]
    private MainMenu mainMenu;

    [SerializeField]
    private GameObject deathPanel;

    public void Start()
    {
        if (MainMenu.loadSavedGame)
        {
            LoadGame();
            deathPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(saveKey))
        {
            SaveGame();
        }
        if (Input.GetKeyDown(loadKey))
        {
            LoadGame();
        }
    }

    public void SaveGame()
    {
        SavedData data = new SavedData();
        data.playerPosition = playerTransform.position;
        data.playerHealth = playerStats.currentHealth;
        data.playerHunger = playerStats.currentHunger;
        data.playerThirst = playerStats.currentThirst;
        data.inventoryItems = Inventory.instance.GetContent();
        data.headEquipped = equipment.equippedHeadItem;
        data.bodyEquipped = equipment.equippedChestItem;
        data.legsEquipped = equipment.equippedLegsItem;
        data.feetEquipped = equipment.equippedFeetItem;
        data.handsEquipped = equipment.equippedHandsItem;
        data.weaponEquipped = equipment.equippedWeaponItem;
        data.placedStructures = buildSystem.placedStructures.ToArray();

        string json = JsonUtility.ToJson(data);
        string filePath = Application.persistentDataPath + "/savefile.json";
        System.IO.File.WriteAllText(filePath, json);
        mainMenu.loadGameButton.interactable = true;
        mainMenu.deleteSaveFileButton.interactable = true;
    }

    public void LoadGame()
    {
        if(deathPanel.activeSelf)
        {
            playerStats.isDead = false;
            playerStats.playerMoveBehaviour.canMove = true;
            playerStats.playerAnimator.SetTrigger("Respawn");
            deathPanel.SetActive(false);
        }
        string filePath = Application.persistentDataPath + "/savefile.json";
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            SavedData data = JsonUtility.FromJson<SavedData>(json);

            playerTransform.position = data.playerPosition;
            playerStats.currentHealth = data.playerHealth;
            playerStats.currentHunger = data.playerHunger;
            playerStats.currentThirst = data.playerThirst;
            Inventory.instance.LoadContent(data.inventoryItems);
            buildSystem.LoadPlacedStructures(data.placedStructures);
        }
        else
        {
            Debug.LogWarning("Save file not found at " + filePath);
        }
    }
}

public class SavedData
{
    // Données à sauvegarder
    public Vector3 playerPosition;
    public float playerHealth;
    public float playerHunger;
    public float playerThirst;
    
    public List<ItemInInventory> inventoryItems;

    public ItemData headEquipped;
    public ItemData bodyEquipped;
    public ItemData legsEquipped;
    public ItemData feetEquipped;
    public ItemData handsEquipped;
    public ItemData weaponEquipped;

    public PlacedStructure[] placedStructures;
    
}
