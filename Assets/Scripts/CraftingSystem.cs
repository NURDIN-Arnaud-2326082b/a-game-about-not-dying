using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    [SerializeField]
    private List<RecipeData> availableRecipes;

    [SerializeField]
    private GameObject recipeUIPrefab;

    [SerializeField]
    private Transform recipeUIParent;

    [SerializeField]
    private KeyCode openCraftingMenuInput;

    [SerializeField]
    private GameObject craftingMenuPanel;

    // Start is called before the first frame update
    void Start()
    {
        UpdateDisplayRecipes();
        craftingMenuPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(openCraftingMenuInput))
        {
            craftingMenuPanel.SetActive(!craftingMenuPanel.activeSelf);
            UpdateDisplayRecipes();
        }
    }

    public void UpdateDisplayRecipes()
    {
        //vider les recettes affichées
        foreach (Transform child in recipeUIParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < availableRecipes.Count; i++)
        {
            GameObject recipe = Instantiate(recipeUIPrefab, recipeUIParent); 
            recipe.GetComponent<Recipe>().Configure(availableRecipes[i]);
        }
    }   
}
