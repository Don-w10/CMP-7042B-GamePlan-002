using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance { get; private set; }

    [Header("Loading UI")]
    public GameObject loadingPanel;    // Panel that covers the screen during transition
    public TMP_Text   loadingText;     // Text inside the panel

    [SerializeField] private float delay = 1.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    // Called by GameManager when Level 1 is complete
    public void StartLoad()
    {
        StartCoroutine(LoadNextLevel());
    }

    // Shows "Loading..." for `delay` seconds then advances to the next build index
    private IEnumerator LoadNextLevel()
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        if (loadingText != null)
            loadingText.text = "Loading...";

        yield return new WaitForSeconds(delay);

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextIndex);
    }
}
