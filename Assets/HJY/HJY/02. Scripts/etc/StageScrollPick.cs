
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageScrollPick : MonoBehaviour
{
    [Header("¿¬°á")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private StageRangeSlot slotPrefab;

    private List<StageRangeSlot> slotPool = new List<StageRangeSlot>();

    public void SetStageList(List<StageSO> stageList)
    {
        while (slotPool.Count < stageList.Count)
        {
            StageRangeSlot slot = Instantiate(slotPrefab, content);
            slotPool.Add(slot);
        }

        for (int i = 0; i < slotPool.Count; i++)
        {
            if (i < stageList.Count)
            {
                slotPool[i].gameObject.SetActive(true);
                slotPool[i].Setup(stageList[i]);
            }
            else
            {
                slotPool[i].gameObject.SetActive(false);
            }
        }

        scrollRect.verticalNormalizedPosition = 1f;
    }
}
