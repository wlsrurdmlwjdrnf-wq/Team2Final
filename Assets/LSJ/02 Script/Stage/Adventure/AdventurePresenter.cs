using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AdventurePresenter : MonoBehaviour
{
    [SerializeField] private AdventureView view;

    [SerializeField] private Color selectedColor = new Color(0.9f, 0.9f, 1f);
    [SerializeField] private Color normalColor = Color.white;

    private StageSO _stage;
    private Button currentSelected;
    private void Awake()
    {
        view.OnAdventureStageButtonClicked += AdventureStageClick;
        view.OnAdventureStageStartButtonClicked += AdventureStageStartClick;
    }

    public void AdventureStageClick(int number, Button selected)
    {
        _stage = StageManager.Instance.GetStageData(number);
        view.UpdateStageInfo(number);
        SelectTab(selected);
    }
    private void SelectTab(Button selected)
    {
        if (currentSelected == selected) return;

        // 이전 선택 해제
        if (currentSelected != null)
        {
            var colors = currentSelected.colors;
            colors.normalColor = normalColor;
            currentSelected.colors = colors;
        }

        // 새로 선택
        currentSelected = selected;
        var selColors = selected.colors;
        selColors.normalColor = selectedColor;
        selColors.selectedColor = selectedColor;
        selected.colors = selColors;

    }
    public void AdventureStageStartClick()
    {
        // 스테이지를 선택하지 않았다면 불가
        if (_stage == null) return;
        // 깃털 하나도 없으면 입장불가
        if (!PlayerResourceManager.Instance.SpendResource(ResourceType.Feather, new BigNumber(1))) return;

        view.HideSelectAdventurePanel();
        StageManager.Instance.ApplyStage(_stage);
    }
    public void ConfirmButtonClick()
    {
        view.HideClearPanel();
    }
    public void CancelFailPanelButtonClick()
    {
        view.HideFailPanel();
    }
    private void OnDestroy()
    {
        if (view == null) return;
        view.OnAdventureStageButtonClicked -= AdventureStageClick;
        view.OnAdventureStageStartButtonClicked -= AdventureStageStartClick;
    }
}
