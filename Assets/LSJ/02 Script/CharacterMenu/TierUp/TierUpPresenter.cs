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
        view.UpdateTier();
    }

    public void TierStageButtonClick(Tier tier)
    {
        StageManager.Instance.ApplyStage(
            StageManager.Instance.GetStageData(tier));
    }

    private void OnDestroy()
    {
        if(view == null) return;
        view.OnTierStageButtonClicked -= TierStageButtonClick;
    }
}
