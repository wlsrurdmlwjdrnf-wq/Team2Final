
using UnityEngine;

public abstract class RecycleScrollSlot<T> : MonoBehaviour
{
    // 슬롯 크기 -> StageRangeSlot에다 구현!!!
    public abstract float Height { get; }
    public abstract float Width { get; }

    // 슬롯 데이터 업데이트 -> 상속받는 클래스에서 구현!!!
    public abstract void UpdateSlot(T data);

    // RectTransform 캐싱!!!
    public RectTransform RectTransform => (RectTransform)transform;

    // 초기화!!!
    public virtual void Init() { }
}