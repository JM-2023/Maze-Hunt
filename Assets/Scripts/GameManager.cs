using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject winScreenPanel;
    public float delayBeforeWinScreen = 1f; // Delay after opening the door

    private bool gameWon = false;

    void Start()
    {
        if (winScreenPanel != null)
        {
            winScreenPanel.SetActive(false);
        }
    }

    public void ShowWinScreen()
    {
        if (!gameWon)
        {
            gameWon = true;
            // Lock cursor and hide it
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
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
    }

    public void RestartGame()
    {
        Debug.Log("Restart Game!");

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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