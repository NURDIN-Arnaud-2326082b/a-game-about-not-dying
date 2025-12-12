using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Classe pour gérer la bibliothèque d'équipements
public class EquipmentLibrary : MonoBehaviour
{
    //Liste des équipements dans la bibliothèque de jeu
    public List<EquipmentLibraryItem> content = new List<EquipmentLibraryItem>();
}

//Classe pour représenter un élément dans la bibliothèque d'équipements
[System.Serializable]
public class EquipmentLibraryItem
{
    //Données de l'item d'équipement
    public ItemData itemData;

    //Prefab de l'équipement associé
    public GameObject equipmentPrefab;

    //Éléments du prefab à désactiver lors de l'équipement pour éviter un conflit visuel
    public GameObject[] ElementToDisable;
}
