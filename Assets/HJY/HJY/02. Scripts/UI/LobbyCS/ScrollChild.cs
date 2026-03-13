
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollChild : ScrollRect
{
    ScrollParent scrollParent;
    ScrollRect parentScrollRect;
    bool forParent;

    protected override void Start()
    {
        scrollParent = GameObject.FindWithTag("Scroll").GetComponent<ScrollParent>();
        parentScrollRect = GameObject.FindWithTag("Scroll").GetComponent<ScrollRect>();

    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그가 시작하는 순간 수평이동이 크면 부모가 드래그를 시작한 것이고 수직이동이 크면 자식이 드래그 시작한 것임
        forParent = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);

        if (forParent)
        {
            scrollParent.OnBeginDrag(eventData);
            parentScrollRect.OnBeginDrag(eventData);
        }
        else base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (forParent)
        {
            scrollParent.OnDrag(eventData);
            parentScrollRect.OnDrag(eventData);
        }
        else base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (forParent)
        {
            scrollParent.OnEndDrag(eventData);
            parentScrollRect.OnEndDrag(eventData);
        }
        else base.OnEndDrag(eventData);
    }
}
