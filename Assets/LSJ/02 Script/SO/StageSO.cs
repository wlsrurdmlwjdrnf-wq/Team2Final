using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameData/Stage/Data", fileName = "StageData")]
public class StageSO : ScriptableObject
{
    public bool isTierStage;
    public bool isBossStage;

    [Header("이 숫자들에 따라 몬스터스탯 및 재화 보정")]
    public int mainNumber;
    public int subNumber;

    [Header("MonsterContainer")]
    public List<GameObject> monsterPrefabs;
    public int spawnCount;
    public float spawnAreaWidth;
    public float minDistance;
    public float yFixedPosition;

    [Header("StageBase")]
    public Sprite backgroundSprite;
}