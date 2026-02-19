using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LimitTimeBar : MonoBehaviour
{
    [Header("UI 컴포넌트")]
    [SerializeField] private Slider _timerSlider;  // 시간 바 (0~1)

    [Header("세팅")]
    [SerializeField] private float _bossTimeLimit = 30f; // 기본 30초

    private Coroutine _timerCo;
    private float _currentTime;

    private void Awake()
    {
        if (_timerSlider == null)
            _timerSlider = GetComponentInChildren<Slider>();

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 스테이지 진입 시 외부에서 호출
    /// </summary>
    public void StartTimer()
    {
        gameObject.SetActive(true);
        _currentTime = _bossTimeLimit;

        UpdateTimerUI();
        if (_timerCo != null) StopCoroutine(_timerCo);
        _timerCo = StartCoroutine(TimerCoroutine());
    }

    /// <summary>
    /// 클리어/나갈 때 호출
    /// </summary>
    public void StopTimer()
    {
        if (_timerCo != null)
        {
            StopCoroutine(_timerCo);
            _timerCo = null;
        }
        gameObject.SetActive(false);
    }

    private IEnumerator TimerCoroutine()
    {
        while (_currentTime > 0f)
        {
            _currentTime -= Time.deltaTime;
            UpdateTimerUI();

            yield return null; // 매 프레임 체크 (정확한 타이밍 위해)
        }

        // 시간 종료
        OnTimeUp();
    }

    private void UpdateTimerUI()
    {
        if (_timerSlider == null) return;

        float normalized = Mathf.Clamp01(_currentTime / _bossTimeLimit);
        _timerSlider.value = normalized;
    }

    /// <summary>
    /// 시간 종료 시 호출 -> 게임 오버
    /// </summary>
    private void OnTimeUp()
    {
        StageManager.Instance.GameOver();
    }

}