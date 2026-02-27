using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurePresenter : MonoBehaviour
{
    [SerializeField] private AdventureView view;

    private void Awake()
    {
        view.OnAdventureStageButtonClicked += AdventureStageClick;
    }

    public void AdventureStageClick(int number)
    {
        // 깃털 하나도 없으면 입장불가
        if (!PlayerResourceManager.Instance.SpendResource(ResourceType.Feather, new BigNumber(1))) return;

        StageManager.Instance.ApplyStage(
            StageManager.Instance.GetStageData(number)
            );
    }
}
