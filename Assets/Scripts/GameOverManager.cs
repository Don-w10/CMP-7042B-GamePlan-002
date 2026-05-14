using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private TMP_Text scoreText;

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (gameOverText != null)
            gameOverText.text = "GAME OVER";

        if (scoreText != null)
            scoreText.text = "Nodes collected: " + GameManager.LastNodesCollected;
    }

    // Assigned to the Restart button's OnClick in the Inspector
    public void RestartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    // Assigned to the Main Menu button's OnClick in the Inspector
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
