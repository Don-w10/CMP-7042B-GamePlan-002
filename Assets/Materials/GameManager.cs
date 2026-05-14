using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Read by Level2Manager and GameOverManager
    public static int CurrentLevel       = 1;
    public static int LastNodesCollected { get; private set; }

    [Header("Level Config")]
    [SerializeField] protected int levelNumber = 1;
    public int totalNodes = 6;

    [Header("UI")]
    public TMP_Text winText;
    public TMP_Text levelText;

    [Header("Player")]
    public PlayerHealth playerHealth;

    private int collected = 0;

    protected virtual void Start()
    {
        CurrentLevel = levelNumber;
        collected    = 0;

        if (winText  != null) winText.gameObject.SetActive(false);
        if (levelText != null) levelText.text = "Level: " + CurrentLevel;

        HUDManager.Instance?.SetNodes(0, totalNodes);
    }

    public void CollectNode()
    {
        collected++;
        LastNodesCollected = collected;
        HUDManager.Instance?.SetNodes(collected, totalNodes);

        if (collected < totalNodes) return;
        OnAllNodesCollected();
    }

    // Level1 behaviour: show brief message then hand off to LevelTransition.
    // Level2Manager overrides this to show the win screen and freeze time.
    protected virtual void OnAllNodesCollected()
    {
        if (winText != null)
        {
            winText.gameObject.SetActive(true);
            winText.text = "LEVEL COMPLETE!\nEntering the Core...";
        }

        LevelTransition.Instance?.StartTransition();
    }
}
