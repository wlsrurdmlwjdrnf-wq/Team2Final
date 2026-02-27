
using UnityEngine;


[System.Serializable]
public class StageView
{
    [Header("스테이지 순서")]
    public string stageTurn;

    [Header("스테이지 이름")]
    public string stageName;

    [Header("스테이지 이미지")]
    public Sprite stageImage;
}

[CreateAssetMenu(fileName = "StageViewSO", menuName = "View/StageViewSO")]
public class StageViewSO : ScriptableObject
{
    public StageView[] stages;
}