using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageUIPresenter : MonoBehaviour
{
    [SerializeField] private StageUIView view;

    private void Awake()
    {
        view.OnBossStageButtonClicked += BossStageButtonClick;
    }

    public void BossStageButtonClick()
    {
        StageManager.Instance.ApplyStage(
            StageManager.Instance.GetStageData(
                StageManager.Instance.CurrentMainNumber,
                StageManager.Instance.CurrentSubNumber,
                true
            )
        );
    }

    private void OnDestroy()
    {
        if (view == null) return;
        view.OnBossStageButtonClicked -= BossStageButtonClick;
    }
}
