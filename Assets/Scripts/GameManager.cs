using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TerritoryManager territoryManager;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Text winText;

    [Header("Win Conditions")]
    [SerializeField] private float targetPercentage = 50f;
    [SerializeField] private bool killAllEnemies = true;

    private int totalEnemies;
    private int killedEnemies;
    private bool gameWon;

    private void Awake()
    {
        if (territoryManager == null)
            territoryManager = FindFirstObjectByType<TerritoryManager>();

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    private void Start()
    {
        EnemyAI[] enemies = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        totalEnemies = enemies.Length;
    }

    private void Update()
    {
        if (gameWon) return;

        int totalCells = territoryManager.TotalCells;
        if (totalCells <= 0) return;

        float percentage = (territoryManager.OwnedCells.Count * 100f) / totalCells;
        bool enemiesDefeated = !killAllEnemies || killedEnemies >= totalEnemies;
        bool percentageReached = percentage >= targetPercentage;

        if (enemiesDefeated && percentageReached)
        {
            WinGame();
        }
    }

    public void OnEnemyKilled()
    {
        killedEnemies++;
    }

    private void WinGame()
    {
        gameWon = true;

        if (winPanel != null)
            winPanel.SetActive(true);

        if (winText != null)
            winText.text = "YOU WIN!";
    }
}