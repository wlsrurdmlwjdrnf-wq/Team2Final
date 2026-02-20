
using UnityEngine;
using UnityEngine.UI;

public class GachaShow : MonoBehaviour
{
 
    [Header("가챠 패널")]
    [SerializeField] private GameObject gachaPanel;

    void Start()
    {
        // 시작 시에는 패널을 꺼둠
        gachaPanel.SetActive(false);
    }

    // 가챠 패널을 보여라.
    public void OpenGacha() => gachaPanel.SetActive(true);
}
