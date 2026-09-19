using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject winningPanel;

    public void ActiveWinningPanel()
    {
        if (winningPanel != null)
        {
            winningPanel.SetActive(true);
        }
    }
}