
using UnityEngine;

// 이 스크립트를 사용할 거면 반드시 Alt + Enter 문제를 막아야 함!!!
// Project Settings -> Player Settings -> Resolution and Presentation 에서 Allow Fullscreen Switch 설정을 끄기!!!
public class FixedDisplay : MonoBehaviour
{
    private void Start()
    {
        // 초기에 게임 해상도 고정
        SetResolution();
    }

    // 해상도 설정하는 함수
    public void SetResolution()
    {
        int setWidth = 720;    // 사용자 설정 너비
        int setHeight = 1280;  // 사용자 설정 높이

        int deviceWidth = Screen.width;    // 기기 너비 저장
        int deviceHeight = Screen.height;  // 기기 높이 저장

        // SetResolution 함수 사용
        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true);

        // 기기의 해상도 비가 더 큰 경우
        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight)
        {
            // 새로운 너비
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight);

            // 새로운 Rect 적용
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
        }
        // 게임의 해상도 비가 더 큰 경우
        else
        {
            // 새로운 높이
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight);

            // 새로운 Rect 적용
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
        }
    }
}