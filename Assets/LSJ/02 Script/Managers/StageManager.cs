using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageInfo
{
    public int mainNumber;
    public int subNumber;
    public bool isBoss;
    public StageSO stageData;
}
[System.Serializable]
public class TierStageInfo
{
    public Tier tier;
    public StageSO stageData;
}
[System.Serializable]
public class AdventureStageInfo
{
    public int number;
    public StageSO stageData;
}
public class StageManager : Singleton<StageManager>
{
    [Header("스테이지 정보")]
    [SerializeField] private List<StageInfo> allStages;
    [SerializeField] private List<TierStageInfo> allTierStages;
    [SerializeField] private List<AdventureStageInfo> allAdventureStages;

    [Header("씬에 있는 플레이어 연결")]
    [SerializeField] private Player _player;

    [Header("씬에 있는 제한시간캔버스 연결")]
    [SerializeField] private LimitTimeBar _limitTimeBar;

    private const int MAX_SUBNUMBER = 20; // 최대 보조 스테이지 수
    public int CurrentMainNumber { get; private set; }
    public int CurrentSubNumber { get; private set; }
    public StageSO CurrentStageData => _currentStageData;

    private WaitForSeconds _waitFadeOut = new WaitForSeconds(1f);
    private WaitForSeconds _delayClear = new WaitForSeconds(0.5f);

    // 최고 기록
    private int _bestMainNumber;
    private int _bestSubNumber;

    // 상태
    private int _currentMonsterCount = 0;
    private StageSO _currentStageData;
    private StageSO _tmpStageData;

    // 이벤트
    public event Action OnStageChanged;   // 스테이지 변경 시
    public event Action OnBossStageChanged;
    public event Action OnTierStageChanged;
    public event Action OnAdventureStageChanged;
    public event Action OnAllMonstersCleared; // 전부 처치 시
    public event Action OnGameOver;     // 게임 오버 시 (플레이어가 죽거나 시간 초과)
    public event Action OnNeedMonsterClear;
    public event Action OnNeedScreenFader;

    protected override void Init()
    {
        base.Init();
        _currentStageData = allStages[0].stageData; // 임시 테스트
        // TODO : 최고스테이지 기록을 _currentStageData에 불러오기
    }
    private void Start()
    {
        ApplyStage(_currentStageData);  // 게임 시작 시 스테이지 적용
    }

    // 스테이지 적용 메인 로직
    public void ApplyStage(StageSO targetStage)
    {
        if (targetStage == null) return;

        if (targetStage.isTierStage || targetStage.isAdventureStage)
        {
            _tmpStageData = _currentStageData;
        }

        OnNeedMonsterClear?.Invoke(); // 남아 있는 몬스터가 있으면 정리

        BestStageRecord(targetStage);

        CurrentMainNumber = targetStage.mainNumber;
        CurrentSubNumber = targetStage.subNumber;

        // 현재 스테이지에 적용
        _currentStageData = targetStage;
        _currentMonsterCount = _currentStageData.spawnCount;

        StartCoroutine(FadeOutInEventCo());
    }
    private IEnumerator FadeOutInEventCo()
    {
        // 화면이 잠시 어두워졌다 원래대로 되는 연출
        OnNeedScreenFader?.Invoke();
        _player.gameObject.SetActive(false);

        yield return _waitFadeOut;

        _player.gameObject.SetActive(true);

        // 보스, 티어, 모험 스테이지일 경우 제한시간타이머 ON
        if(_currentStageData.isBossStage || _currentStageData.isTierStage || _currentStageData.isAdventureStage)
        {
            if (_limitTimeBar != null)
                _limitTimeBar.StartTimer();
        }

        // 이벤트 실행
        OnStageChanged?.Invoke();
        if (_currentStageData.isBossStage) OnBossStageChanged?.Invoke();
        if (_currentStageData.isTierStage) OnTierStageChanged?.Invoke();
        if (_currentStageData.isAdventureStage) OnAdventureStageChanged?.Invoke();
    }

    // 몬스터 사망 시 호출 (MonsterBase.cs에서)
    public void OnMonsterDeath()
    {
        _currentMonsterCount--;

        if (_currentMonsterCount <= 0) // 스테이지 클리어
        {
            OnAllMonstersCleared?.Invoke();

            StartCoroutine(DelayClearCo());
        }
    }

    // 스테이지 클리어 시
    private IEnumerator DelayClearCo()
    {
        // 타이머 OFF
        if (_currentStageData.isBossStage || _currentStageData.isTierStage || _currentStageData.isAdventureStage)
        {
            if (_limitTimeBar != null)
                _limitTimeBar.StopTimer();
        }

        yield return _delayClear;

        if (_currentStageData.isBossStage)
            ApplyStage(GetStageData(CurrentMainNumber, CurrentSubNumber + 1)); // 다음 스테이지
        else if (_currentStageData.isTierStage)
            TierStageClear();
        else if(_currentStageData.isAdventureStage)
            AdventureStageClear();
        else 
            ApplyStage(GetStageData(CurrentMainNumber, CurrentSubNumber)); // 현재 스테이지 반복
    }
    private void TierStageClear()
    {
        ApplyStage(GetStageData(_tmpStageData.mainNumber, _tmpStageData.subNumber));
        PlayerStatManager.Instance.PromoteTier();
    }
    private void AdventureStageClear()
    {
        ApplyStage(GetStageData(_tmpStageData.mainNumber, _tmpStageData.subNumber));
        // 에메랄드와 다이아를 얻을 로직 필요
    }

    // 게임 오버 시 (플레이어가 죽거나 시간 초과)
    public void GameOver()
    {
        OnGameOver?.Invoke();
        // 타이머 OFF
        if (_currentStageData.isBossStage || _currentStageData.isTierStage || _currentStageData.isAdventureStage)
        {
            if (_limitTimeBar != null)
                _limitTimeBar.StopTimer();
        }

        if (_currentStageData.isTierStage || _currentStageData.isAdventureStage)
            ApplyStage(GetStageData(_tmpStageData.mainNumber, _tmpStageData.subNumber)); // 승급,모험 스테이지 전 스테이지 적용
        else ApplyStage(GetStageData(CurrentMainNumber, CurrentSubNumber)); // 현재 스테이지 반복
    }

    // 매개변수에 맞는 StageSO 가져오기
    public StageSO GetStageData(int mainNumber, int subNumber, bool isBoss = false)
    {
        if(subNumber > MAX_SUBNUMBER)
        {
            subNumber -= MAX_SUBNUMBER;
            mainNumber += 1;
        }

        foreach (var stage in allStages)
        {
            if (stage.mainNumber == mainNumber &&
                stage.subNumber == subNumber &&
                stage.isBoss == isBoss)
            {
                return stage.stageData;
            }
        }
        return null; // 조건에 맞는 StageInfo가 없을 경우
    }
    public StageSO GetStageData(Tier tier)
    {
        foreach (var stage in allTierStages)
        {
            if(stage.tier == tier) return stage.stageData;
        }
        return null;
    }
    public StageSO GetStageData(int number)
    {
        foreach(var stage in allAdventureStages)
        {
            if(stage.number == number) return stage.stageData;
        }
        return null;
    }

    // 최고 스테이지 기록
    public void BestStageRecord(StageSO stage)
    {
        if (stage.isTierStage || stage.isAdventureStage) return;
        _bestMainNumber = stage.mainNumber;
        _bestSubNumber = stage.subNumber;
    }

    // TODO : 최고 기록 저장
}
