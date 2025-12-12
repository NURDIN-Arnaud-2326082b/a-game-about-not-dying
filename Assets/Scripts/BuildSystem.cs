using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class BuildSystem : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private Structure[] structures;

    [SerializeField]
    private Transform placedStructuresParent;

    [SerializeField]
    private Material canBuildMaterial;

    [SerializeField]
    private Material cannotBuildMaterial;

    [SerializeField]
    private KeyCode floorKey;

    [SerializeField]
    private KeyCode wallKey;

    [SerializeField]
    private KeyCode stairsKey;

    [SerializeField]
    private KeyCode rotateKey;

    [SerializeField]
    private Transform rotatePoint;

    [Header("UI References")]

    [SerializeField]
    private Transform buildSystemUIPanel;

    [SerializeField]
    private GameObject buildingRequiredElement;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip buildSound;

    private Structure currentStructure;

    private bool canBuild ;

    private Vector3 finalPosition;

    public bool inPlace;
    private bool systemEnabled = false;

    public List<PlacedStructure> placedStructures;

    public void Awake()
    {
        currentStructure = structures.First();  
        DisableSystem();
    }

    private void FixedUpdate()
    {
        if (!systemEnabled) return;
        canBuild = currentStructure.placementPrefab.GetComponentInChildren<CollisionDetectionEdge>().CheckConnection();
        finalPosition = grid.GetNearestPointOnGrid(transform.position);
        CheckPosition();
        RoundRotation();
        UpdateMaterial();
    }
    private void Update()
    {
        if (Input.GetKeyDown(wallKey))
        {
            if(currentStructure.structureType == StructureType.Wall && systemEnabled)
            {
                DisableSystem();

            }
            else 
            {
                ChangeStructureType(GetStructureByType(StructureType.Wall));
            }
            
        }
        else if (Input.GetKeyDown(floorKey))
        {
            if(currentStructure.structureType == StructureType.Floor && systemEnabled)
            {
                DisableSystem();

            }
            else 
            {
                ChangeStructureType(GetStructureByType(StructureType.Floor));
            }
        }
        else if (Input.GetKeyDown(stairsKey))
        {
            if(currentStructure.structureType == StructureType.Stairs && systemEnabled)
            {
                DisableSystem();

            }
            else 
            {
                ChangeStructureType(GetStructureByType(StructureType.Stairs));
            }
        }
        if (Input.GetKeyDown(rotateKey))
        {
            RotateStructure();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && canBuild && inPlace && systemEnabled && HasAllResources())
        {
            BuildStructure();
        }

    }

    public void BuildStructure()
    {
        audioSource.PlayOneShot(buildSound);
        Instantiate(currentStructure.instantiationPrefab, currentStructure.placementPrefab.transform.position, currentStructure.placementPrefab.transform.GetChild(0).transform.rotation, placedStructuresParent);
        placedStructures.Add(new PlacedStructure { prefab = currentStructure.instantiationPrefab, position = currentStructure.placementPrefab.transform.position, rotation = currentStructure.placementPrefab.transform.GetChild(0).transform.rotation.eulerAngles });
        for (int i = 0; i < currentStructure.cost.Length; i++)
        {
            Inventory.instance.RemoveItem(currentStructure.cost[i].itemData);
        }
    }

    private bool HasAllResources()
    {
        BuildingRequiredElement[] requiredElements = GameObject.FindObjectsOfType<BuildingRequiredElement>();
        return requiredElements.All(e => e.hasResources);
    }
    private void DisableSystem()
    {
        systemEnabled = false;
        currentStructure.placementPrefab.SetActive(false);
        buildSystemUIPanel.gameObject.SetActive(false);
    
    }

    void RoundRotation()
    {
        int roundedRotation = 0;
        float Yangle = rotatePoint.eulerAngles.y;
        if(Yangle > -45 || Yangle <= 45)
        {
            roundedRotation = 0;
        }
        else if (Yangle > 45 && Yangle < 135)
        {
            roundedRotation = 90;
        }
        else if (Yangle >= 135 && Yangle < 225)
        {
            roundedRotation = 180;
        }
        else if (Yangle >= 225 && Yangle < 315)
        {
            roundedRotation = 270;
        }
        currentStructure.placementPrefab.transform.rotation = Quaternion.Euler(0, roundedRotation, 0);
        
    }
    private void RotateStructure()
    {
        currentStructure.placementPrefab.transform.GetChild(0).Rotate(0, 90, 0);
    }

    private void UpdateMaterial()
    {
        MeshRenderer meshRenderer = currentStructure.placementPrefab.GetComponentInChildren<CollisionDetectionEdge>().meshRenderer;
        if (canBuild && inPlace && HasAllResources())
        {
            meshRenderer.material = canBuildMaterial;
        }
        else
        {
            meshRenderer.material = cannotBuildMaterial;
        }
    }

    void ChangeStructureType(Structure newStructure)
    {
        buildSystemUIPanel.gameObject.SetActive(true);
        systemEnabled = true;
        currentStructure = newStructure;
        foreach (Structure structure in structures)
        {
            structure.placementPrefab.SetActive(structure.structureType == newStructure.structureType);
        }

        UpdateCosts();

    }

    private Structure GetStructureByType(StructureType structureType)
    {
        return structures.Where(s => s.structureType == structureType).FirstOrDefault();
    }
    

    public void CheckPosition()
    {
        inPlace = currentStructure.placementPrefab.transform.position == finalPosition;
        if(!inPlace)
        {
            SetPosition(finalPosition);
        }
    }

    public void SetPosition(Vector3 position)
    {
        Transform placementPrefabTransform = currentStructure.placementPrefab.transform;
        Vector3 positionVelocity = Vector3.zero;

        if(Vector3.Distance(placementPrefabTransform.position, position) > 10)
        {
            placementPrefabTransform.position = position;
        }
        else 
        {
            Vector3 targetPosition = Vector3.SmoothDamp(placementPrefabTransform.position, position, ref positionVelocity, 0, 15000);
            placementPrefabTransform.position = targetPosition;
        }
    }

    public void UpdateCosts()
    {
        //reset l'ui
        foreach(Transform child in buildSystemUIPanel)
        {
            Destroy(child.gameObject);
        }
        //ajouter l'élément requis pour la construction
        foreach(ItemInInventory item in currentStructure.cost)
        {
            GameObject element = Instantiate(buildingRequiredElement, buildSystemUIPanel);
            element.GetComponent<BuildingRequiredElement>().SetUp(item);
        }
    }

    public void LoadPlacedStructures(PlacedStructure[] placedStructures)
    {
        foreach (PlacedStructure structure in placedStructures)
        {
            Instantiate(structure.prefab, structure.position, Quaternion.Euler(structure.rotation), placedStructuresParent);
            this.placedStructures.Add(structure);
        }
    }

}

[System.Serializable]
public class Structure
{
    public StructureType structureType;
    public GameObject placementPrefab;
    public GameObject instantiationPrefab;
    public ItemInInventory[] cost;
}

public enum StructureType
{
    Wall,
    Floor,
    Stairs
}

[System.Serializable]
public class PlacedStructure
{
    public GameObject prefab;
    public  Vector3 position;
    public Vector3 rotation;
}