using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TierUpView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tierText;
    [SerializeField] private Button bronzeStageButton;

    public event Action OnBronzeStageButtonClicked;

    private void Awake()
    {
        bronzeStageButton.onClick.AddListener(() => OnBronzeStageButtonClicked?.Invoke());
        StageManager.Instance.OnAllMonstersCleared += UpdateTier;
    }

    public void UpdateTier()
    {
        tierText.text = PlayerStatManager.Instance.PlayerTier.ToString();
    }

    private void OnDestroy()
    {
        if (StageManager.Instance == null) return;
        StageManager.Instance.OnAllMonstersCleared -= UpdateTier;
    }
}
