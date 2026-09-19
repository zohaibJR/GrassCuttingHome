using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject levelPrefab;

    [Header("Camera")]
    [SerializeField] private CameraFollow cameraFollow;

    [Header("UI")]
    [SerializeField] private TerritoryPercentage territoryPercentage;

    public void StartLevel1()
    {
        // Instantiate Level at (0, 0, 0)
        GameObject level = Instantiate(
            levelPrefab,
            Vector3.zero,
            Quaternion.identity
        );

        TerritoryManager territoryManager = level.GetComponentInChildren<TerritoryManager>();

        if (territoryManager == null)
        {
            Debug.LogError("GameManager: Instantiated level has no TerritoryManager.", level);
        }

        // Instantiate Player at (0, 0.586, 0)
        GameObject player = Instantiate(
            playerPrefab,
            new Vector3(0f, 0.586f, 0f),
            Quaternion.identity
        );

        // Assign the newly spawned Player to the camera
        cameraFollow.SetTarget(player.transform);

        // Bind the UI to this level's TerritoryManager
        if (territoryPercentage != null)
        {
            territoryPercentage.Bind(territoryManager);
        }
        else
        {
            Debug.LogWarning("GameManager: TerritoryPercentage reference not assigned; territory UI will not update.", this);
        }
    }
}