
using UnityEngine;
using UnityEngine.UI;

public class FlashButton : MonoBehaviour
{
    public float speed = 2f;
    private Image img;

    void Start()
    {
        img = GetComponent<Image>();
    }

    void Update()
    {
        // 무지개 색으로 바꿔라.
        float r = 0.5f + 0.5f * Mathf.Sin(Time.time * speed);
        float g = 0.5f + 0.5f * Mathf.Sin(Time.time * speed + 2f);
        float b = 0.5f + 0.5f * Mathf.Sin(Time.time * speed + 4f);

        img.color = new Color(r, g, b, 1f);

        // 깜박거리게 만들어라.
        // float alpha = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        // Color color = img.color;
        // color.a = alpha;
        // img.color = color;
    }
}