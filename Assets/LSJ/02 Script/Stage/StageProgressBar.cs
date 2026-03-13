using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageProgressBar : MonoBehaviour
{
    [SerializeField] private Slider stageProgressBar;

    private float _spawnCount;
    private float _currentCount;

    private void Start()
    {
        StageManager.Instance.OnStageChanged += InitBar;
    }
    private void OnDestroy()
    {
        if (StageManager.Instance == null) return;
        StageManager.Instance.OnStageChanged -= InitBar;
    }

    private void InitBar()
    {
        if(StageManager.Instance.CurrentStageData.isBossStage) gameObject.SetActive(false);
        else gameObject.SetActive(true);

        _spawnCount = StageManager.Instance.CurrentStageData.spawnCount;
        _currentCount = _spawnCount;
        float progress = _currentCount / _spawnCount;
        stageProgressBar.value = progress;
    }
    public void UpdateBar()
    {
        _currentCount--;
        float progress = _currentCount / _spawnCount;
        stageProgressBar.value = progress;
    }
}
