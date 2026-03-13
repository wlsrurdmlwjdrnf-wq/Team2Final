using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameData/Stage/Data", fileName = "StageData")]
public class StageSO : ScriptableObject
{
    public bool isTierStage;    // 승급 스테이지?
    public bool isBossStage;    // 보스 스테이지?
    public bool isAdventureStage;   // 모험 스테이지?

    [Header("이 숫자들에 따라 몬스터스탯 및 재화 보정")]
    public int mainNumber; // 메인 스테이지 넘버
    public int subNumber;  // 보조 스테이지 넘버

    [Header("MonsterContainer")]
    public List<GameObject> monsterPrefabs; // 스테이지를 구성하는 몬스터리스트
    public int spawnCount;  // 스테이지 몬스터 수
    public float spawnAreaWidth; // 몬스터 스폰 범위
    public float minDistance;   // 몬스터 간 최소 거리
    public float yFixedPosition; // 스폰될 y 포지션

    [Header("StageBase")]
    public Sprite backgroundSprite; // 배경 스프라이트
}