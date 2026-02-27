
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StagePickPanel : MonoBehaviour
{
   
    [Header("지역")]
    [SerializeField] private StageViewSO stageViewSO;
    [SerializeField] private TextMeshProUGUI regionText;
    [SerializeField] private Image stageImage;

    [Header("버튼")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button closeButton;

    private int currentMainNumber = 1;
    private int maxMainNumber = 10;

    private void Start()
    {
        leftButton.onClick.AddListener(OnClickLeft);
        rightButton.onClick.AddListener(OnClickRight);
        closeButton.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel()
    {
        gameObject.SetActive(true);
        UpdateButtonState();
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    private void UpdateButtonState()
    {
        rightButton.gameObject.SetActive(currentMainNumber < maxMainNumber);
        leftButton.gameObject.SetActive(currentMainNumber > 1);
    }

    public void SetStage(StageSO stageData, StageView viewData)
    {
        currentMainNumber = stageData.mainNumber;
        regionText.text = viewData.stageName;
        stageImage.sprite = viewData.stageImage;

        UpdateButtonState();
    }

    private void Refresh()
    {
        int index = currentMainNumber - 1;

        if (index >= 0 && index < stageViewSO.stages.Length)
        {
            StageView viewData = stageViewSO.stages[index];
            regionText.text = viewData.stageName;
            stageImage.sprite = viewData.stageImage;
        }

        UpdateButtonState();
    }


    private void OnClickLeft()
    {
        currentMainNumber--;
        Refresh();
    }

    private void OnClickRight()
    {
        currentMainNumber++;
        Refresh();
    }
}