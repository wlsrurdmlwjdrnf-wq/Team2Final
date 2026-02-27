
using UnityEngine;

public class MapUI : MonoBehaviour
{
    [Header("퀘스트 패널")]
    [SerializeField] private GameObject questPanel; // 퀘스트를 주는 패널
    [SerializeField] private GameObject ClearPanel; // 퀘스트 클리어시 뜨는 패널
    [SerializeField] private GameObject FailPanel;  // 퀘스트 실패시 뜨는 패널

    public void OpenQuest()
    {
        questPanel.SetActive(true);
    }

    public void CloseQuest()
    {
        questPanel.SetActive(false);
    }
}
