
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// 하이어라키 창에서 선택한 오브젝트 속 Image 컴포넌트의 maskable을 꺼서 이미지가 보이게 만듦
// mask 영역 아니어도 보이게끔 조절해서 이미지 편집하기 쉽게 만드는 기능임
// 편집이 끝나면 maskable을 다시 켜서 불필요한 마스크 계산을 줄여서 성능 개선할 것!!!
public class MaskableTool
{
    [MenuItem("Tools/Maskable/Disable Selected")]
    // 선택한 모든 오브젝트의 Image 컴포넌트 maskable을 꺼라
    static void DisableSelected()
    {
        foreach (var obj in Selection.gameObjects)
        {
            foreach (var img in obj.GetComponentsInChildren<Image>(true))
            {
                img.maskable = false;
            }
        }
    }

    [MenuItem("Tools/Maskable/Enable Selected")]
    // 선택한 모든 오브젝트의 Image 컴포넌트 maskable을 켜라
    static void EnableSelected()
    {
        foreach (var obj in Selection.gameObjects)
        {
            foreach (var img in obj.GetComponentsInChildren<Image>(true))
            {
                img.maskable = true;
            }
        }
    }
}