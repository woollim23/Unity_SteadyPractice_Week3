using UnityEngine;

public class Equip : MonoBehaviour
{
    [Header("Resource Gathering")]
    public bool doesGatherResources; // �ڿ� ä�� �� �� �ִ���

    [Header("Combat")]
    public bool doesDealDamage; // ���� �������� �� �� �ִ���
    public int damage; // ������ �󸶸�ŭ �ٰ���

    [Header("Equipment")]
    public bool doesIncrease; // ���� �������� �ִ���
    public float increase; // ���� ��ġ

    public virtual void OnAttackInput()
    {

    }
}
