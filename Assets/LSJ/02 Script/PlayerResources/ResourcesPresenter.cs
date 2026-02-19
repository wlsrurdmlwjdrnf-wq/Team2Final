using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesPresenter : MonoBehaviour
{
    [SerializeField] private ResourcesView view;

    private Action<ResourceType, BigNumber> _resourceChangedHandler;
    private void Awake()
    {
        // 핸들러 정의 및 구독
        _resourceChangedHandler = OnResourceChanged;
        ResourcesModel.OnResourceChanged += _resourceChangedHandler;
    }
    private void Start()
    {
        // 초기 UI 갱신
        RefreshAllDisplays();
    }

    private void OnResourceChanged(ResourceType type, BigNumber newValue)
    {
        string formatted = PlayerResourceManager.Instance.GetFormatted(type);
        view.UpdateResourceDisplay(type, formatted);
    }

    private void RefreshAllDisplays()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            string formatted = PlayerResourceManager.Instance.GetFormatted(type);
            view.UpdateResourceDisplay(type, formatted);
        }
    }

    private void OnDestroy()
    {
        ResourcesModel.OnResourceChanged -= _resourceChangedHandler;
        _resourceChangedHandler = null;
    }
}
