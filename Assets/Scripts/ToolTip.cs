using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    //Référence aux composant UI du tooltip (titre)
    [SerializeField]
    private Text header;

    //Référence aux composant UI du tooltip (contenu)
    [SerializeField]
    private Text content;

    //Référence au LayoutElement pour ajuster la taille du tooltip
    [SerializeField]
    private LayoutElement layoutElement;

    //Nombre maximum de caractères avant d'ajuster la taille
    [SerializeField]
    private int maxCharacter;

    //Référence au RectTransform du tooltip
    [SerializeField]
    private RectTransform rectTransform;

    //Méthode pour définir le texte du tooltip
    public void setText(string contentText, string headerText = "")
    {
        if (headerText == "")
        {
            header.gameObject.SetActive(false);
        }
        else
        {
            header.gameObject.SetActive(true);
            header.text = headerText;
        }
        content.text = contentText;
        int headerLength = header.text.Length;
        int contentLength = content.text.Length;
        if (headerLength > maxCharacter || contentLength > maxCharacter)
        {
            layoutElement.enabled = true;
        }
        else
        {
            layoutElement.enabled = false;
        }
    }

    //Méthode Update pour suivre la position de la souris
    private void Update()
    {
        Vector2 position = Input.mousePosition;
        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;
        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = position;
    }
}
