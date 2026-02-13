using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 재화 드롭 시스템 (EXP, Gold, EnhancementCube)
public class PlayerResourceDropSystem : Singleton<PlayerResourceDropSystem>
{
    [SerializeField] private Transform stageRoot;

    [SerializeField] private GameObject expPrefab;
    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private GameObject cubePrefab;

    [SerializeField] private DropBaseAmountSO amountSO;

    public void TriggerDrop(Vector3 monsterPosition)
    {
        PlayerResourceManager.Instance.AddResource(ResourceType.EXP, DropAmountCorrection(amountSO.baseExpAmount));
        PlayerResourceManager.Instance.AddResource(ResourceType.Gold, DropAmountCorrection(amountSO.baseGoldAmount));
        SpawnDropItem(expPrefab, monsterPosition);
        SpawnDropItem(goldPrefab, monsterPosition);

        int rand = Random.Range(0, 100);
        if (rand < 80) return;
        PlayerResourceManager.Instance.AddResource(ResourceType.EnhancementCube, DropAmountCorrection(amountSO.baseEnhancementCubeAmount));
        SpawnDropItem(cubePrefab, monsterPosition);
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
