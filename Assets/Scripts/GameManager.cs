using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject[] levelPrefabs; // index 0 = Level 1, index 1 = Level 2, etc.

    [Header("Camera")]
    [SerializeField] private CameraFollow cameraFollow;

    [Header("UI")]
    [SerializeField] private TerritoryPercentage territoryPercentage;
    [SerializeField] private UIManager uiManager;

    private TerritoryManager currentTerritoryManager;
    private GameObject currentLevelInstance;
    private int currentLevelNumber;

    public void StartLevel(int levelNumber)
    {
        int index = levelNumber - 1;

        if (index < 0 || index >= levelPrefabs.Length || levelPrefabs[index] == null)
        {
            Debug.LogError($"GameManager: No level prefab assigned for level {levelNumber}.", this);
            return;
        }

        // Clean up a previously loaded level, if any (lets you restart/replay)
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }

        currentLevelNumber = levelNumber;

        // Instantiate Level at (0, 0, 0)
        currentLevelInstance = Instantiate(
            levelPrefabs[index],
            Vector3.zero,
            Quaternion.identity
        );

        TerritoryManager territoryManager = currentLevelInstance.GetComponentInChildren<TerritoryManager>();

        if (territoryManager == null)
        {
            Debug.LogError("GameManager: Instantiated level has no TerritoryManager.", currentLevelInstance);
        }

        // Instantiate Player at (0, 0.586, 0)
        GameObject player = Instantiate(
            playerPrefab,
            new Vector3(0f, 0.586f, 0f),
            Quaternion.identity
        );

        // Assign the newly spawned Player to the camera
        cameraFollow.SetTarget(player.transform);

        // Bind the percentage UI to this level's TerritoryManager
        if (territoryPercentage != null)
        {
            territoryPercentage.Bind(territoryManager);
        }

        // Unsubscribe from the previous level's manager, if any
        if (currentTerritoryManager != null)
        {
            currentTerritoryManager.OnWinningPercentageReached -= HandleWinningPercentageReached;
        }

        currentTerritoryManager = territoryManager;

        if (currentTerritoryManager != null)
        {
            currentTerritoryManager.OnWinningPercentageReached += HandleWinningPercentageReached;
        }
    }

    private void HandleWinningPercentageReached()
    {
        // Unlock the next level
        LevelProgress.UnlockLevel(currentLevelNumber + 1);

        if (uiManager != null)
        {
            uiManager.ActiveWinningPanel();
        }
    }

    private void OnDestroy()
    {
        if (currentTerritoryManager != null)
        {
            currentTerritoryManager.OnWinningPercentageReached -= HandleWinningPercentageReached;
        }
    }
}