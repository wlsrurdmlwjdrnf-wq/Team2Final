using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSJ_DTO : MonoBehaviour
{
    // 데이터 저장할 때
    //public string resourcesJson = PlayerResourceManager.Instance.GetSaveJson();
    //public string statsJson = PlayerStatManager.Instance.GetSaveJson();
    //public string powerLevelJson = CharacterUpgradeSaveManager.Instance.SavePowerUpData();
    //public string growLevelJson = CharacterUpgradeSaveManager.Instance.SaveGrowUpData();
    //public string stageRecordJson = StageRecordDataManager.Instance.SaveStageData();

    [Serializable]
    public class PlayerDataSaveRequest
    {
        public string resourcesJson; // 재화 ( enum ResourcesType에 있는 모든 것)
        public string statsJson; // 스탯 ( 레벨, 티어, 모디파이어(enum StatType에서 공속,이속을 제외한 스탯들의 강화정보) )
        public string powerLevelJson; // 강화 레벨
        public string growLevelJson; // 성장 레벨
        public string stageRecordJson; // 스테이지 최고 기록
    }
    [SerializeField]
    public class PlayerDataSaveResponse : BaseResponse { }

    [SerializeField]
    public class PlayerDataLoadRequest { }
    [Serializable]
    public class PlayerDataLoadResponse : BaseResponse
    {
        public string resourcesJson;
        public string statsJson;
        public string powerLevelJson;
        public string growLevelJson;
        public string stageRecordJson;
    }

    // 데이터 불러올 때
    //bool loadSuccess1 = PlayerResourceManager.Instance.LoadFromJson(res.resourcesJson);
    //bool loadSuccess2 = PlayerStatManager.Instance.LoadFromJson(res.statsJson);
    //bool loadSuccess3 = CharacterUpgradeSaveManager.Instance.LoadPowerUpData(res.powerLevelJson);
    //bool loadSuccess4 = CharacterUpgradeSaveManager.Instance.LoadGrowUpData(res.growLevelJson);
    //bool loadSuccess5 = StageRecordDataManager.Instance.LoadStageData(res.stageRecordJson);

}
