using UnityEngine;

public class Equip : MonoBehaviour
{
    [Header("Resource Gathering")]
    public bool doesGatherResources; // 자원 채취 할 수 있는지

    [Header("Combat")]
    public bool doesDealDamage; // 공격 데미지를 줄 수 있는지
    public int damage; // 데미지 얼마만큼 줄건지

    [Header("Equipment")]
    public bool doesIncrease; // 스탯 증가율이 있는지
    public float increase; // 증가 수치

    public virtual void OnAttackInput()
    {

    }
}
