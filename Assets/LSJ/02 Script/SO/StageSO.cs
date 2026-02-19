using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameData/Stage/Data", fileName = "StageData")]
public class StageSO : ScriptableObject
{
    public bool isTierStage;
    public bool isBossStage;
    public bool isAdventureStage;

    [Header("이 숫자들에 따라 몬스터스탯 및 재화 보정")]
    public int mainNumber; // 메인 스테이지 넘버
    public int subNumber;  // 보조 스테이지 넘버

    [Header("MonsterContainer")]
    public List<GameObject> monsterPrefabs;
    public int spawnCount;
    public float spawnAreaWidth;
    public float minDistance;
    public float yFixedPosition;

    [Header("StageBase")]
    public Sprite backgroundSprite;
}