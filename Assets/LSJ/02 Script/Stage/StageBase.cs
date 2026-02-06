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
}
