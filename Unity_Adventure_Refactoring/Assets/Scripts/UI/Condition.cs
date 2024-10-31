using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue; // ���� ��
    public float startvalue; // �ʱ� ��
    public float maxValue; // �ִ� ��
    public float passiveValue; // ��ȭ ��

    public Image uiBar;

    void Start()
    {
        curValue = startvalue;
    }


    void Update()
    {
        uiBar.fillAmount = GetPercentage(); // �� ����
    }


    float GetPercentage()
    {
        return curValue / maxValue;
    }

    public void Add(float value)
    {
        curValue = Mathf.Min(curValue + value, maxValue);
        // �ִ밪�� ���� �ʰ� ��� ��
    }

    public void Subtract(float value)
    {
        curValue = Mathf.Max(curValue - value, 0);
        // 0 ������ �������� �ʰ� ���
    }
}
