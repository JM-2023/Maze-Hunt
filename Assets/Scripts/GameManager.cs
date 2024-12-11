using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets; // Ensure you have this if StarterAssetsInputs is in this namespace

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject winScreenPanel;
    public GameObject gameOverScreenPanel; // <-- Add this reference to your GameOverScreen panel
    public float delayBeforeWinScreen = 1f; // Delay after opening the door

    private bool gameWon = false;
    private bool gameOver = false; // Track if the game is over

    void Start()
    {
        if (winScreenPanel != null)
        {
            winScreenPanel.SetActive(false);
        }
        
        if (gameOverScreenPanel != null)
        {
            gameOverScreenPanel.SetActive(false);
        }
    }

    public void ShowWinScreen()
    {
        if (!gameWon)
        {
            gameWon = true;
            // Unlock and show the cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Prevent StarterAssetsInputs from re-locking the cursor
            StarterAssetsInputs inputs = FindObjectOfType<StarterAssetsInputs>();
            if (inputs != null)
            {
                inputs.cursorLocked = false;
            }

            // Show win screen after delay
            Invoke("DisplayWinScreen", delayBeforeWinScreen);
        }
    }

    void DisplayWinScreen()
    {
        if (winScreenPanel != null)
        {
            winScreenPanel.SetActive(true);
        }

        // Pause the game after showing the win screen
        Time.timeScale = 0f;
    }

    public void ShowGameOverScreen()
    {
        if (!gameOver)
        {
            gameOver = true;
            // Unlock and show the cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Prevent StarterAssetsInputs from re-locking the cursor
            StarterAssetsInputs inputs = FindObjectOfType<StarterAssetsInputs>();

            Time.timeScale = 0f;

            if (inputs != null)
            {
                inputs.cursorLocked = false;
            }

            if (gameOverScreenPanel != null)
            {
                gameOverScreenPanel.SetActive(true);
            }
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restart Game!");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // After loading the scene, re-lock the cursor:
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game!");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
