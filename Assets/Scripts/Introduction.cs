using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Introduction : MonoBehaviour
{
    [SerializeField]
    private Text introductionText;

    [Header("Dialogues d'introduction")]
    [SerializeField]
    [TextArea(3, 10)]
    private string[] dialogues = new string[]
    {
        "One day, during a regular travel...",
        "A ship wrecked and you found yourself stranded on a mysterious island.",
        "Gather resources, build a shelter, try to repair your boat.",
        "And above all, avoid the dangers that lurk in the island.",
        "Good luck, survivor!"
    };

    [SerializeField]
    private float typingSpeed = 0.05f; // Vitesse du typing effect

    [SerializeField]
    private float delayBetweenDialogues = 2f; // Délai avant de passer au dialogue suivant

    [SerializeField]
    private float fadeDuration = 0.5f; // Durée du fade in/out

    [SerializeField]
    private bool autoAdvance = true; // Passer automatiquement au dialogue suivant

    [Header("Chargement de scène")]
    [SerializeField]
    private string sceneToLoad = "GameScene"; // Nom de la scène à charger

    [SerializeField]
    private bool loadSceneAsync = true; // Charger la scène en arrière-plan

    private int currentDialogueIndex = 0;
    private Coroutine currentCoroutine;
    private AsyncOperation sceneLoadOperation;

    void Start()
    {
        if (introductionText != null && dialogues.Length > 0)
        {
            StartIntroduction();
            
            // Démarrer le chargement asynchrone de la scène
            if (loadSceneAsync && !string.IsNullOrEmpty(sceneToLoad))
            {
                StartCoroutine(LoadSceneAsync());
            }
        }
        else
        {
            Debug.LogWarning("IntroductionText ou dialogues non configurés!");
        }
    }

    void Update()
    {
        // Appuyer sur Espace ou clic pour passer au dialogue suivant
        if (!autoAdvance && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            NextDialogue();
        }
    }

    public void StartIntroduction()
    {
        currentDialogueIndex = 0;
        ShowDialogue(currentDialogueIndex);
    }

    public void NextDialogue()
    {
        if (currentDialogueIndex < dialogues.Length - 1)
        {
            currentDialogueIndex++;
            ShowDialogue(currentDialogueIndex);
        }
        else
        {
            EndIntroduction();
        }
    }

    private void ShowDialogue(int index)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(DisplayDialogueWithEffect(dialogues[index]));
    }

    private IEnumerator DisplayDialogueWithEffect(string dialogue)
    {
        // Fade out du texte précédent
        yield return StartCoroutine(FadeText(0f));

        // Effet de typing
        introductionText.text = "";
        
        foreach (char letter in dialogue.ToCharArray())
        {
            introductionText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Fade in complet
        yield return StartCoroutine(FadeText(1f));

        // Attendre avant le dialogue suivant (si auto-advance activé)
        if (autoAdvance)
        {
            yield return new WaitForSeconds(delayBetweenDialogues);
            NextDialogue();
        }
    }

    private IEnumerator FadeText(float targetAlpha)
    {
        Color startColor = introductionText.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            introductionText.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        introductionText.color = targetColor;
    }

    private IEnumerator LoadSceneAsync()
    {
        // Commencer le chargement de la scène
        sceneLoadOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        
        // Empêcher la scène de s'activer automatiquement
        sceneLoadOperation.allowSceneActivation = false;
        
        Debug.Log($"Chargement de {sceneToLoad} en arrière-plan...");
        
        // Attendre que le chargement soit presque terminé (90%)
        while (sceneLoadOperation.progress < 0.9f)
        {
            yield return null;
        }
        
        Debug.Log($"Scène {sceneToLoad} chargée et prête!");
    }

    private void EndIntroduction()
    {
        Debug.Log("Introduction terminée!");
        
        // Si la scène est chargée en async, l'activer maintenant
        if (sceneLoadOperation != null)
        {
            sceneLoadOperation.allowSceneActivation = true;
        }
        else if (!string.IsNullOrEmpty(sceneToLoad))
        {
            // Chargement direct si pas d'async
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    // Méthode publique pour sauter l'introduction
    public void SkipIntroduction()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        EndIntroduction();
    }
}
