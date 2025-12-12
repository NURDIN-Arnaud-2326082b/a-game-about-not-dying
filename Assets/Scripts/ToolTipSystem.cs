using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipSystem : MonoBehaviour
{
    //Singleton instance du système de tooltip
    public static ToolTipSystem instance;

    //Référence au composant ToolTip
    [SerializeField]
    private ToolTip toolTip;

    //Initialisation du singleton
    private void Awake()
    {
        instance = this;
    }

    //Méthode pour afficher le tooltip
    public void Show(string content, string header = "")
    {
        toolTip.setText(content, header);
        toolTip.gameObject.SetActive(true);
    }

    //Méthode pour cacher le tooltip
    public void Hide()
    {
        toolTip.gameObject.SetActive(false);
    }
}
        

