using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private float initialDelay = 0.5f;
    [SerializeField] private float repeatInterval = 0.1f;

    [SerializeField] private UnityEvent onHoldRepeat;

    private Coroutine _holdRoutine;
    private WaitForSeconds _initialDelay;
    private WaitForSeconds _repeatInterval;

    private void Awake()
    {
        _initialDelay = new WaitForSeconds(initialDelay);
        _repeatInterval = new WaitForSeconds(repeatInterval);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _holdRoutine = StartCoroutine(HoldRoutine());
    }
    public void OnPointerUp(PointerEventData eventData) 
    {
        StopHold();
    }
    public void OnPointerExit(PointerEventData eventData) 
    {
        StopHold();
    }

    private IEnumerator HoldRoutine()
    {
        yield return _initialDelay;
        onHoldRepeat.Invoke();

        while (true)
        {
            yield return _repeatInterval;
            onHoldRepeat.Invoke();
        }
    }
    private void StopHold()
    {
        if (_holdRoutine != null)
        {
            StopCoroutine(_holdRoutine);
            _holdRoutine = null;
        }
    }

    private void OnDisable()
    {
        StopHold();
    }
}
