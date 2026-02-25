using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TierUpView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tierText;
    [SerializeField] private Button[] stageButtons;

    public event Action<Tier> OnTierStageButtonClicked;

    private void Awake()
    {
        ButtonsAddListener();

        StageManager.Instance.OnAllMonstersCleared += UpdateTier;
    }
    private void ButtonsAddListener()
    {
        if (stageButtons.Length == 0) return;
        for (int i = 0; i < stageButtons.Length; i++)
        {
            Tier tier = (Tier)(i + 1);
            stageButtons[i].onClick.AddListener(() => OnTierStageButtonClicked(tier));
        }
    }
    public void UpdateTier()
    {
        tierText.text = PlayerStatManager.Instance.PlayerTier.ToString();
        for(int i = 0; i < stageButtons.Length; i++)
        {
            if(PlayerStatManager.Instance.PlayerTier == (Tier)i)
            {
                stageButtons[i].gameObject.SetActive(true);
                continue;
            }
            stageButtons[i].gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (StageManager.Instance == null) return;
        StageManager.Instance.OnAllMonstersCleared -= UpdateTier;
    }
}
