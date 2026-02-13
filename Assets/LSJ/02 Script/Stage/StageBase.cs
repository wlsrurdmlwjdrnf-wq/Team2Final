using UnityEngine;

public class StageBase : MonoBehaviour
{
    [SerializeField] private float _resetPositionX = 10.2f;  // 재배치 거리

    private Camera _mainCamera;
    private float _leftBound;
    private float _rightBound;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _leftBound = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        _rightBound = _leftBound + _resetPositionX;
    }
    private void OnEnable()
    {
        if (StageManager.Instance == null) return;
        StageManager.Instance.OnStageChanged += ChangeAllChildSprites;
    }
    private void OnDisable()
    {
        if (StageManager.Instance == null) return;
        StageManager.Instance.OnStageChanged -= ChangeAllChildSprites;
    }
    private void Update()
    {
        // 재배치
        if (transform.position.x < _leftBound)
        {
            transform.position += Vector3.right * _resetPositionX;
        }
        else if (transform.position.x > _rightBound)
        {
            transform.position -= Vector3.right * _resetPositionX;
        }
    }

    private void ChangeAllChildSprites()
    {
        Sprite newSprite = StageManager.Instance.CurrentStageData.backgroundSprite;
        foreach (Transform child in transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = newSprite;
            }
        }
    }

}
