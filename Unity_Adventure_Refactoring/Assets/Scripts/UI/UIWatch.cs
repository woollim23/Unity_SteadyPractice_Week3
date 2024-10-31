using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIWatch : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayTime;
    private IEnumerator Start()
    {
        while (true)
        {
            displayTime.text = GetCurrntTime();
            yield return new WaitForSeconds(1f);
        }
    }

    public static string GetCurrntTime()
    {
        // 현재 시, 분 출력
        return DateTime.Now.ToString(("HH : mm"));
    }
}
