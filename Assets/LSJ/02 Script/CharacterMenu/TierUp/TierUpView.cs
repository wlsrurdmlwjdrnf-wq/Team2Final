using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TierUpView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tierText;
    [SerializeField] private Button[] stageButtons;
    [SerializeField] private Image[] tierIconImages;
    [SerializeField] private Sprite[] tierIcons;
    [SerializeField] private TextMeshProUGUI[] promoteCompleteText;

    public event Action<Tier> OnTierStageButtonClicked;

    private bool _isTier = false;

    private void Awake()
    {
        ButtonsAddListener();

        StageManager.Instance.OnAllMonstersCleared += UpdateTier;
        StageManager.Instance.OnTierStageChanged += ToggleIsTier;
        StageManager.Instance.OnGameOver += IsTierFalse;
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
        if (!_isTier) return;

        tierText.text = PlayerStatManager.Instance.PlayerTier.ToString();
        for(int i = 0; i < stageButtons.Length; i++)
        {
            if(PlayerStatManager.Instance.PlayerTier == (Tier)i)
            {
                stageButtons[i].gameObject.SetActive(true);
                if (i != 0) promoteCompleteText[i - 1].text = "½Â±Þ ¿Ï·á";
                foreach(var icon in tierIconImages)
                {
                    icon.sprite = tierIcons[i];
                }
                continue;
            }
            stageButtons[i].gameObject.SetActive(false);
        }

        ToggleIsTier();
    }
    private void ToggleIsTier()
    {
        _isTier = !_isTier;
    }
    private void IsTierFalse()
    {
        _isTier = false;
    }
    private void OnDestroy()
    {
        if (StageManager.Instance == null) return;
        StageManager.Instance.OnAllMonstersCleared -= UpdateTier;
        StageManager.Instance.OnTierStageChanged -= ToggleIsTier;
        StageManager.Instance.OnGameOver -= IsTierFalse;

    }
}
