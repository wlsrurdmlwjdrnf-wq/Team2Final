using UnityEngine;
using UnityEngine.UI;

public class AutoCombineButton : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO _eventChannel;
    [SerializeField] private EDataType _type;
    [SerializeField] private Button _button;

    private void Awake()
    {
        _button.onClick.AddListener(() => 
        {
            _eventChannel.RaiseEvent(EGameEventType.AutoCombine, new EDataTypePayload(_type));
        });
    }
}
