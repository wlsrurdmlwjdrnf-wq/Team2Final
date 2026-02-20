using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TierUpPresenter : MonoBehaviour
{
    [SerializeField] private TierUpView view;

    private void Awake()
    {
        view.OnBronzeStageButtonClicked += BronzeStageButtonClick;
    }
    private void Start()
    {
        view.UpdateTier();
    }

    public void BronzeStageButtonClick()
    {
        StageManager.Instance.ApplyStage(
            StageManager.Instance.GetStageData(Tier.Bronze));
    }

    private void OnDestroy()
    {
        if(view == null) return;
        view.OnBronzeStageButtonClicked -= BronzeStageButtonClick;
    }
}
