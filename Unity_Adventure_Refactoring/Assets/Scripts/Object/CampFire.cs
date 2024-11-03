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
            // 인터페이스 IDamagaIbe를 가지고 있다면 리스트에 보관
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
            // TODO : 코루틴으로 대미지 주기
            damagaIbe.TakePhysicalDamage(damage);
            yield return isTrigger;
        }
    }
}
