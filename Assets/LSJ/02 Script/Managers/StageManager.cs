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

    [Header("씬에 있는 제한시간캔버스/스테이지진행도캔버스 연결")]
    [SerializeField] private LimitTimeBar _limitTimeBar;
    [SerializeField] private StageProgressBar _stageProgressBar;

    private const int MAX_SUBNUMBER = 20; // 최대 보조 스테이지 수
    public const int ADVENTURE_BASE_DIAMOND_AMOUNT = 1000;
    public const int ADVENTURE_BASE_EMERALD_AMOUNT = 200;
    public const float ADVENTURE_BASE_ARTIFACT_PROBABILITY = 0.05f;

    private ItemDataSO _artifact;
    private bool _isClearing = false; // 클리어 중 인지 여부
    public string ArtifactName { get; set; } = string.Empty;
    public int CurrentMainNumber { get; private set; }
    public int CurrentSubNumber { get; private set; }
    public StageSO CurrentStageData => _currentStageData;

    private WaitForSeconds _waitFadeOut = new WaitForSeconds(1f);
    private WaitForSeconds _delayClear = new WaitForSeconds(0.5f);

    // 최고 기록
    private int _bestMainNumber = 1;
    private int _bestSubNumber = 1;
    private int _bestAdventureNumber = 0;

    // 상태
    private int _currentMonsterCount = 0;
    private int _currentAdventureNumber;
    private StageSO _currentStageData;
    private StageSO _tmpStageData;

    // 이벤트
    public event Action OnStageChanged;   // 스테이지 변경 시
    public event Action OnBossStageChanged;
    public event Action OnTierStageChanged;
    public event Action OnAdventureStageChanged;
    public event Action OnAllMonstersCleared; // 전부 처치 시
    public event Action OnGameOver;     // 게임 오버 시 (플레이어가 죽거나 시간 초과)
    public event Action OnNeedMonsterClear; // 몬스터 정리
    public event Action OnNeedScreenFader;  // 화면 페이드아웃 인

    protected override void Init()
    {
        base.Init();
        SetRecord(); // 최고 기록 불러오기
        _currentStageData = GetStageData(_bestMainNumber, _bestSubNumber);
    }
    private void Start()
    {
        ApplyStage(_currentStageData);  // 게임 시작 시 스테이지 적용
    }

    // 스테이지 적용 메인 로직
    public void ApplyStage(StageSO targetStage)
    {
        if (targetStage == null) return;
        if (_isClearing) return;

        if (targetStage.isTierStage || targetStage.isAdventureStage)
        {
            _tmpStageData = _currentStageData;
        }

        OnNeedMonsterClear?.Invoke(); // 남아 있는 몬스터가 있으면 정리

        BestStageRecord(targetStage); // 일반 스테이지 최고 기록이면 저장

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

        _isClearing = true; // 스테이지 클리어 중에는 다른 스테이지가 적용되지 않도록 하기
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

        if (_currentStageData.isBossStage)
        {
            SoundManager.Instance.PlayBGM(EBGMType.BossStage);
            OnBossStageChanged?.Invoke();
        }
        else if (_currentStageData.isTierStage)
        {
            SoundManager.Instance.PlayBGM(EBGMType.TierStage);
            OnTierStageChanged?.Invoke();
        }
        else if (_currentStageData.isAdventureStage)
        {
            SoundManager.Instance.PlayBGM(EBGMType.AdventureStage);
            OnAdventureStageChanged?.Invoke();
        }
        else SoundManager.Instance.PlayBGM(EBGMType.MainStage);

        _isClearing = false;
    }

    // 몬스터 사망 시 호출 (MonsterBase.cs에서)
    public void OnMonsterDeath()
    {
        _currentMonsterCount--;
        _stageProgressBar.UpdateBar();

        if (_currentMonsterCount <= 0) // 스테이지 클리어
        {
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

        OnAllMonstersCleared?.Invoke(); // 스테이지 클리어 이벤트 발송
    }
    // 티어 스테이지 클리어
    private void TierStageClear()
    {
        ApplyStage(GetStageData(_tmpStageData.mainNumber, _tmpStageData.subNumber));
        PlayerStatManager.Instance.PromoteTier();
    }
    // 모험 스테이지 클리어
    private void AdventureStageClear()
    {
        ApplyStage(GetStageData(_tmpStageData.mainNumber, _tmpStageData.subNumber));
        PlayerResourceManager.Instance.AddResource(ResourceType.Diamond, new BigNumber(_currentAdventureNumber * ADVENTURE_BASE_DIAMOND_AMOUNT));
        PlayerResourceManager.Instance.AddResource(ResourceType.Emerald, new BigNumber(_currentAdventureNumber * ADVENTURE_BASE_EMERALD_AMOUNT));

        // 확률적으로 유물 얻기
        GetArtifactReward();

        if(_bestAdventureNumber < _currentAdventureNumber) _bestAdventureNumber = _currentAdventureNumber;
    }
    private void GetArtifactReward()
    {
        float probability = _currentAdventureNumber * ADVENTURE_BASE_ARTIFACT_PROBABILITY;

        if (UnityEngine.Random.value <= probability)
        {
            _artifact = InventorySystem.Instance.GetRandomArtifact();   
            ArtifactName = _artifact.Name;
        }
        else
        {
            ArtifactName = "X";
        }
    }

    // 게임 오버 시 (플레이어가 죽거나 시간 초과나 나가기버튼 클릭 시)
    public void GameOver()
    {
        // 타이머 OFF
        if (_currentStageData.isBossStage || _currentStageData.isTierStage || _currentStageData.isAdventureStage)
        {
            if (_limitTimeBar != null)
                _limitTimeBar.StopTimer();
        }

        if (_currentStageData.isTierStage || _currentStageData.isAdventureStage)
            ApplyStage(GetStageData(_tmpStageData.mainNumber, _tmpStageData.subNumber)); // 승급,모험 스테이지 전 스테이지 적용
        else ApplyStage(GetStageData(CurrentMainNumber, CurrentSubNumber)); // 현재 스테이지 반복

        OnGameOver?.Invoke();
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
            if (stage.number == number)
            {
                _currentAdventureNumber = number;
                return stage.stageData;
            }
        }
        return null;
    }

    // 최고 스테이지 기록
    public void BestStageRecord(StageSO stage)
    {
        if (stage.isTierStage || stage.isAdventureStage) return;
        if (_bestMainNumber > stage.mainNumber) return;
        else if (_bestMainNumber == stage.mainNumber && _bestSubNumber >= stage.subNumber) return;
        else
        {
            _bestMainNumber = stage.mainNumber;
            _bestSubNumber = stage.subNumber;
        }
    }

    // 최고 기록 내보내기
    public BestStageData GetRecord()
    {
        return new BestStageData
        {
            bestMainNumber = _bestMainNumber,
            bestSubNumber = _bestSubNumber,
            bestAdventureNumber = _bestAdventureNumber,
        };
    }
    // 최고 기록 불러오기
    public void SetRecord()
    {
        _bestMainNumber = StageRecordDataManager.Instance.MainNumber;
        _bestSubNumber = StageRecordDataManager.Instance.SubNumber;
        _bestAdventureNumber = StageRecordDataManager.Instance.AdventureNumber;
    }
}
