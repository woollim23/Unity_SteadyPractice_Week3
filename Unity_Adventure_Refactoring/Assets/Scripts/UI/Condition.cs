using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue; // 현재 값
    public float startvalue; // 초기 값
    public float maxValue; // 최대 값
    public float passiveValue; // 변화 값

    public Image uiBar;

    void Start()
    {
        curValue = startvalue;
    }


    void Update()
    {
        uiBar.fillAmount = GetPercentage(); // 바 변경
    }


    float GetPercentage()
    {
        return curValue / maxValue;
    }

    public void Add(float value)
    {
        curValue = Mathf.Min(curValue + value, maxValue);
        // 최대값을 넘지 않게 방어 함
    }

    public void Subtract(float value)
    {
        curValue = Mathf.Max(curValue - value, 0);
        // 0 밑으로 내려가지 않게 방어
    }
}
