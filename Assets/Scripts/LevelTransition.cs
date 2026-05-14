using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    public static LevelTransition Instance { get; private set; }

    [Header("UI")]
    public TMP_Text    messageText;   // centred text shown before fade
    public CanvasGroup fadeOverlay;   // full-screen black Image + CanvasGroup on same GO

    [Header("Timing")]
    [SerializeField] private float holdBeforeFade = 1.0f;  // seconds message shows before fade
    [SerializeField] private float fadeDuration   = 1.5f;  // seconds the fade-to-black takes
    // total visible time = holdBeforeFade + fadeDuration = 2.5 s by default

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (fadeOverlay != null) { fadeOverlay.alpha = 0f; fadeOverlay.gameObject.SetActive(false); }
        if (messageText  != null)  messageText.gameObject.SetActive(false);
    }

    // Called by GameManager when all Level 1 nodes are collected
    public void StartTransition() => StartCoroutine(TransitionRoutine());

    private IEnumerator TransitionRoutine()
    {
        // Show message immediately
        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);
            messageText.text = "LEVEL COMPLETE!\nEntering the Core...";
        }

        yield return new WaitForSeconds(holdBeforeFade);

        // Fade screen to black
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeOverlay.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }
            fadeOverlay.alpha = 1f;
        }
        else
        {
            yield return new WaitForSeconds(fadeDuration);
        }

        SceneManager.LoadScene("Level2");
    }
}
