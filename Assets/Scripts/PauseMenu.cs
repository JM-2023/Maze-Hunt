using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Listen for ESC key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(isPaused);
        }

        // Lock or unlock the cursor based on pause state
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;

        // Pause or resume time
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void PlayAgain()
    {
        // Call GameManager's RestartGame method
        FindObjectOfType<GameManager>().RestartGame();
    }

    public void ExitGame()
    {
        // Call GameManager's ExitGame method
        FindObjectOfType<GameManager>().ExitGame();
    }
}
