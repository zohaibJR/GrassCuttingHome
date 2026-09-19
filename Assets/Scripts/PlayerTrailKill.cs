using UnityEngine;

public class PlayerTrailKill : MonoBehaviour
{
    [SerializeField] private TerritoryManager territoryManager;
    [SerializeField] private float killRadius = 0.5f;
    [SerializeField] private GameManager gameManager;

    private void Awake()
    {
        if (territoryManager == null)
            territoryManager = FindFirstObjectByType<TerritoryManager>();

        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        if (territoryManager == null) return;

        if (territoryManager.IsPlayerInsideEnemyTrail(
            transform.position,
            killRadius,
            out EnemyAI killerEnemy))
        {
            if (killerEnemy != null && killerEnemy.IsAlive)
            {
                Debug.Log("Enemy Died - Player entered enemy trail");
                killerEnemy.Die();

                if (gameManager != null)
                    gameManager.OnEnemyKilled();
            }
        }
    }
}
