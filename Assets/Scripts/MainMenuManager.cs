using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text subtitleText;

    private void Start()
    {
        if (titleText != null)
            titleText.text = "FRAGMENTED REALITY BREAKER";

        if (subtitleText != null)
            subtitleText.text = "Collect all energy nodes to escape";

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Assigned to the Start button's OnClick in the Inspector
    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    // Assigned to the Quit button's OnClick in the Inspector
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
