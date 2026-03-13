using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TierUpPresenter : MonoBehaviour
{
    [SerializeField] private TierUpView view;

    private void Awake()
    {
        view.OnTierStageButtonClicked += TierStageButtonClick;
    }
    private void Start()
    {
        view.ToggleIsTier();
        view.UpdateTier();
    }

    public void TierStageButtonClick(Tier tier)
    {
        SoundManager.Instance.PlaySFX(ESFXType.Button);
        StageManager.Instance.ApplyStage(
            StageManager.Instance.GetStageData(tier));
    }

    private void OnDestroy()
    {
        if(view == null) return;
        view.OnTierStageButtonClicked -= TierStageButtonClick;
    }
}
