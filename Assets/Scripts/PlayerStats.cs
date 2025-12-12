using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    public Animator playerAnimator;

    [SerializeField]
    public MoveBehaviour playerMoveBehaviour;

    [Header("Health Settings")]
    [SerializeField]
    public float maxHealth = 100f;

    public float currentHealth;

    [SerializeField]
    private Image healthBarFill;

    [Header("Hunger Settings")]
    [SerializeField]
    public float maxHunger = 100f;

    public float currentHunger;

    [SerializeField]
    private float hungerDecreaseRate;

    [SerializeField]
    private Image hungerBarFill;

    [Header("Thirst Settings")]
    [SerializeField]
    public float maxThirst = 100f;

    public float currentThirst;

    [SerializeField]
    private float thirstDecreaseRate;

    [SerializeField]
    private Image thirstBarFill;

    public float currentArmorPoints;

    [HideInInspector]
    public bool isDead = false;

    [SerializeField]
    private GameObject deathPanel;

    [SerializeField]
    private Button loadCheckpointButton;

    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentThirst = maxThirst;
    }

    // Update is called once per frame
    void Update()
    {
        
        UpdateHungerAnThirstBarFill();
        //ou exclusif (l'un ou l'autre est a zero)
        if (currentHunger <= 0 ^ currentThirst <= 0)
        {
            TakeDamage(2.5f * Time.deltaTime);
        }
        //si les deux sont a zero
        else if (currentHunger <= 0 && currentThirst <= 0)
        {
            TakeDamage(50f * Time.deltaTime);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount * (1 - currentArmorPoints / 100);
        if (currentHealth < 0 && !isDead)
        {
            currentHealth = 0;
            Die();
        }
        UpdateHealthBarFill();
    }

    public void UpdateHealthBarFill()
    {
        healthBarFill.fillAmount = currentHealth / maxHealth;
    }   


    public void UpdateHungerAnThirstBarFill()
    {
        currentHunger -= hungerDecreaseRate * Time.deltaTime;
        if (currentHunger < 0) currentHunger = 0;
        hungerBarFill.fillAmount = currentHunger / maxHunger;
        currentThirst -= thirstDecreaseRate * Time.deltaTime;
        if (currentThirst < 0) currentThirst = 0;
        thirstBarFill.fillAmount = currentThirst / maxThirst;
    }

    public void ConsumeItem(float health, float hunger, float thirst)
    {
        currentHealth += health;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHealthBarFill();

        currentHunger += hunger;
        if (currentHunger > maxHunger) currentHunger = maxHunger;

        currentThirst += thirst;
        if (currentThirst > maxThirst) currentThirst = maxThirst;
    }

    public void Die()
    {
        isDead = true;
        playerMoveBehaviour.canMove = false;
        hungerDecreaseRate = 0;
        thirstDecreaseRate = 0;
        playerAnimator.SetTrigger("Die");
        deathPanel.SetActive(true);
        string filePath = Application.persistentDataPath + "/savefile.json";
        if (System.IO.File.Exists(filePath))
        {
            loadCheckpointButton.interactable = true;
        }
        else
        {
            loadCheckpointButton.interactable = false;
        }
    }
    
}
