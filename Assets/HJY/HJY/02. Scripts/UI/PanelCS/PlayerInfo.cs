using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [Header("플레이어 정보")]
    [SerializeField] private GameObject playerPanel; // 플레이어 스탯 패널

    public void OpenInfo()
    {
        playerPanel.SetActive(true);
    }

    public void CloseInfo()
    {
        playerPanel.SetActive(false);
    }
}
