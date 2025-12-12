using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenuPanel;

    [SerializeField]
    private GameObject optionsMenuPanel;

    [SerializeField]
    private KeyCode pauseKey;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePauseMenu();
        }
    }
    public void TogglePauseMenu()
    {
        bool isActive = pauseMenuPanel.activeSelf;
        pauseMenuPanel.SetActive(!isActive);
        optionsMenuPanel.SetActive(false);
        if (isActive)
        {
            Time.timeScale = 1f; // Reprendre le jeu
        }
        else
        {
            Time.timeScale = 0f; // Mettre le jeu en pause
        }
    }
}
