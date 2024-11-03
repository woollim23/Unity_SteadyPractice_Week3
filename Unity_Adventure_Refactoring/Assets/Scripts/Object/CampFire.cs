using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    public int damage;
    public float damageRate;
    private bool isTrigger;

    List<IDamagable> things = new List<IDamagable>();

    void Start()
    {
        isTrigger = false;
        //InvokeRepeating("DealDamage", 0, damageRate);
    }

    //void DealDamage()
    //{
    //    for(int i = 0; i < things.Count; i++)
    //    {
    //        things[i].TakePhysicalDamage(damage);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamagable damagaIbe))
        {
            // �������̽� IDamagaIbe�� ������ �ִٸ� ����Ʈ�� ����
            //things.Add(damagaIbe);
            isTrigger = true;
            StartCoroutine(DealDamage(damagaIbe));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out IDamagable damagaIbe))
        {
            isTrigger = false;
            //things.Remove(damagaIbe);
        }
    }

    IEnumerator DealDamage(IDamagable damagaIbe)
    {
        while(true)
        {
            // TODO : �ڷ�ƾ���� ����� �ֱ�
            damagaIbe.TakePhysicalDamage(damage);
            yield return isTrigger;
        }
    }
}
