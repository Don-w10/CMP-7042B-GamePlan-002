using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level2Manager : GameManager
{
    [Header("Win Sequence Timing")]
    [SerializeField] private float textFadeInDuration = 0.6f;
    [SerializeField] private float messageHoldDuration = 2.5f;
    [SerializeField] private float screenFadeDuration = 1.5f;

    protected override void OnAllNodesCollected()
    {
        StartCoroutine(WinSequence());
    }

    private IEnumerator WinSequence()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // ── Build a procedural overlay Canvas so it works even if scene UI isn't wired ──
        var root = new GameObject("WinCanvas");
        var canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 99;
        root.AddComponent<CanvasScaler>();
        root.AddComponent<GraphicRaycaster>();

        // Dark vignette / background strip so text is legible over any scene
        var bgGO = new GameObject("TextBG");
        bgGO.transform.SetParent(root.transform, false);
        var bgRect = bgGO.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0f, 0.35f);
        bgRect.anchorMax = new Vector2(1f, 0.65f);
        bgRect.offsetMin = bgRect.offsetMax = Vector2.zero;
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0f, 0f, 0f, 0f);
        bgImg.raycastTarget = false;

        // Win message text
        var textGO = new GameObject("WinText");
        textGO.transform.SetParent(root.transform, false);
        var textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.05f, 0.35f);
        textRect.anchorMax = new Vector2(0.95f, 0.65f);
        textRect.offsetMin = textRect.offsetMax = Vector2.zero;
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = "REALITY RESTORED\nYOU WIN!";
        tmp.fontSize = 64;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.25f, 1f, 0.85f, 0f);  // start transparent, cyan-white
        tmp.raycastTarget = false;

        // Full-screen black fade panel (starts transparent, drawn on top)
        var fadeGO = new GameObject("FadePanel");
        fadeGO.transform.SetParent(root.transform, false);
        var fadeRect = fadeGO.AddComponent<RectTransform>();
        fadeRect.anchorMin = Vector2.zero;
        fadeRect.anchorMax = Vector2.one;
        fadeRect.offsetMin = fadeRect.offsetMax = Vector2.zero;
        var fadeImg = fadeGO.AddComponent<Image>();
        fadeImg.color = new Color(0f, 0f, 0f, 0f);
        fadeImg.raycastTarget = false;

        // ── Phase 1: Fade text + background in ──────────────────────────────
        float elapsed = 0f;
        while (elapsed < textFadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / textFadeInDuration);
            tmp.color   = new Color(0.25f, 1f, 0.85f, t);
            bgImg.color = new Color(0f, 0f, 0f, t * 0.6f);
            yield return null;
        }
        tmp.color   = new Color(0.25f, 1f, 0.85f, 1f);
        bgImg.color = new Color(0f, 0f, 0f, 0.6f);

        // ── Phase 2: Hold on win message ─────────────────────────────────────
        yield return new WaitForSecondsRealtime(messageHoldDuration);

        // ── Phase 3: Fade entire screen to black ─────────────────────────────
        elapsed = 0f;
        while (elapsed < screenFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / screenFadeDuration);
            fadeImg.color = new Color(0f, 0f, 0f, t);
            yield return null;
        }
        fadeImg.color = new Color(0f, 0f, 0f, 1f);

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
