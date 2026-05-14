using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("HUD Elements")]
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI nodeText;

    [Header("References")]
    public PlayerHealth playerHealth;

    // FPS smoothing
    private float fpsTimer;
    private float smoothedFPS;
    private const float FPS_SMOOTH_INTERVAL = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= OnHealthChanged;
    }

    private void Start()
    {
        // Sync health display on load in case OnEnable fired before playerHealth was assigned
        if (playerHealth != null)
            SetHealth(playerHealth.currentHealth, playerHealth.maxHealth);
    }

    private void Update()
    {
        UpdateFPS();
    }

    // ── FPS ───────────────────────────────────────────────────────────────────
    // Accumulates raw frame rate samples and updates the display every 0.5 s
    // so the counter is readable rather than flickering every frame.

    private void UpdateFPS()
    {
        float rawFPS = 1f / Time.deltaTime;
        // Lerp toward the raw value to smooth over the interval
        smoothedFPS = Mathf.Lerp(smoothedFPS, rawFPS, Time.deltaTime / FPS_SMOOTH_INTERVAL);

        fpsTimer += Time.deltaTime;
        if (fpsTimer >= FPS_SMOOTH_INTERVAL)
        {
            fpsTimer = 0f;
            if (fpsText != null)
                fpsText.text = "FPS: " + Mathf.RoundToInt(smoothedFPS);
        }
    }

    // ── Health ────────────────────────────────────────────────────────────────

    private void OnHealthChanged(float current, float max)
    {
        SetHealth(current, max);
    }

    public void SetHealth(float current, float max)
    {
        if (healthText != null)
            healthText.text = "HP: " + Mathf.CeilToInt(current) + "/" + Mathf.RoundToInt(max);
    }

    // ── Nodes ─────────────────────────────────────────────────────────────────

    public void SetNodes(int collected, int total)
    {
        if (nodeText != null)
            nodeText.text = "Nodes: " + collected + "/" + total;
    }
}
