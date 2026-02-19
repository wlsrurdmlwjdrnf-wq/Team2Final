using UnityEngine;

// 재화 드롭 시스템 (EXP, Gold, EnhancementCube)
public class PlayerResourceDropSystem : Singleton<PlayerResourceDropSystem>
{
    [Header("재화들이 이동함에 따라 움직일 수 있도록 연결할 것")]
    [SerializeField] private Transform stageRoot;

    [Header("스테이지에서 드롭될 재화")]
    [SerializeField] private GameObject expPrefab;
    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject fireStonePrefab;
    [SerializeField] private GameObject waterStonePrefab;
    [SerializeField] private GameObject windStonePrefab;
    [SerializeField] private GameObject earthStonePrefab;

    [Header("기본 드롭량")]
    [SerializeField] private DropBaseAmountSO amountSO;

    private const int CUBE_PROBABILITY = 20;
    private const int STONE_PROBABILITY = 5;

    private int _tmpRandomNumber; // 랜덤 집어 넣을 임의의 변수

    public void TriggerDrop(Vector3 monsterPosition)
    {
        PlayerResourceManager.Instance.AddResource(ResourceType.EXP, DropAmountCorrection(amountSO.baseExpAmount));
        PlayerResourceManager.Instance.AddResource(ResourceType.Gold, DropAmountCorrection(amountSO.baseGoldAmount));
        SpawnDropItem(expPrefab, monsterPosition);
        SpawnDropItem(goldPrefab, monsterPosition);

        _tmpRandomNumber = Random.Range(0, 100);
        if (_tmpRandomNumber < CUBE_PROBABILITY)
        {
            PlayerResourceManager.Instance.AddResource(ResourceType.EnhancementCube, DropAmountCorrection(amountSO.baseEnhancementCubeAmount));
            SpawnDropItem(cubePrefab, monsterPosition);
        }

        _tmpRandomNumber = Random.Range(0, 100);
        if (_tmpRandomNumber < STONE_PROBABILITY)
        {
            _tmpRandomNumber = Random.Range(0, 4);
            switch (_tmpRandomNumber)
            {
                case 0: 
                    PlayerResourceManager.Instance.AddResource(ResourceType.FireStone, DropAmountCorrection(amountSO.baseElementalStone));
                    SpawnDropItem(fireStonePrefab, monsterPosition);
                    break;
                case 1:
                    PlayerResourceManager.Instance.AddResource(ResourceType.WaterStone, DropAmountCorrection(amountSO.baseElementalStone));
                    SpawnDropItem(waterStonePrefab, monsterPosition);
                    break;
                case 2:
                    PlayerResourceManager.Instance.AddResource(ResourceType.WindStone, DropAmountCorrection(amountSO.baseElementalStone));
                    SpawnDropItem(windStonePrefab, monsterPosition);
                    break;
                case 3:
                    PlayerResourceManager.Instance.AddResource(ResourceType.EarthStone, DropAmountCorrection(amountSO.baseElementalStone));
                    SpawnDropItem(earthStonePrefab, monsterPosition);
                    break;
            }
        }
    }

    // 드롭 연출
    private void SpawnDropItem(GameObject prefab, Vector3 pos)
    {
        PoolManager2.Instance.Get(prefab, pos, Quaternion.identity, stageRoot);
    }

    // 스테이지에 따른 드롭량 보정
    private BigNumber DropAmountCorrection(float amount)
    {
        BigNumber bn = new BigNumber(amount) *
            new BigNumber(Mathf.Pow(StageManager.Instance.CurrentMainNumber, 5)) *
            new BigNumber((StageManager.Instance.CurrentSubNumber + StageManager.Instance.CurrentMainNumber - 2) * 2);

        if (bn <= new BigNumber(0)) return new BigNumber(amount);
        else return bn;
    }
}
