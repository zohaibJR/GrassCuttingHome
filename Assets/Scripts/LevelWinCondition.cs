using UnityEngine;

public class LevelWinCondition : MonoBehaviour
{
    [Header("Win Settings")]
    [Range(1f, 100f)]
    [SerializeField] private float winningPercentage = 50f;

    public float WinningPercentage => winningPercentage;
}