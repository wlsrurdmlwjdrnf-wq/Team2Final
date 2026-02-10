using UnityEngine;
using UnityEngine.UI;

public class UIGachaButton : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO _eventChannel;
    [SerializeField] private EDataType _gachaType;
    [SerializeField] private int _pullCount = 11;
    [SerializeField] private Button _button;

    private void Awake()
    {
        _button.onClick.AddListener(() =>
        {
            var payload = new GachaRequestPayload(_gachaType, _pullCount);
            _eventChannel.RaiseEvent(EGameEventType.GachaRequest, payload);
        });
    }
}
