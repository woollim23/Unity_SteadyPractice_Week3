using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Wandering, // 돌아다니는 상태
    Attacking
}

public class NPC : MonoBehaviour, IDamagable
{
    [Header("Stats")]
    public int health;
    public float walkSpeed;
    public float runSpeed;
    public ItemData[] dropOnDeath;

    [Header("AI")]
    private NavMeshAgent agent;
    private AIState aiState;
    public float detectDistance;
    public float safeDistance;

    [Header("Wandering")]
    public float minWanderDistance; // 최소 거리
    public float maxWanderDistance; // 최대 거리
    public float minWanderWaitTime; // 거닐기 대기 최소 시간(새로운 목표지점 찍을때까지 대기)
    public float maxWanderWaitTime; // 거닐기 대기 최대 시간

    [Header("Combat")]
    public int damage;
    public float attackRate; // 공격주기
    private float lastAttackTime;
    public float attackDistance; // 공격가능한 거리

    private float playerDistance; // 플레이어와의 거리

    public float fieldOfView = 120f; // 몬스터의 시야각
    
    private Animator animator;
    private SkinnedMeshRenderer[] meshRenderers;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        SetState(AIState.Wandering); // 시작은 거닐기
    }

    private void Update()
    {
        playerDistance = Vector3.Distance(transform.position, CharacterManager.Instance.Player.transform.position);
        // 플레이어와의 거리

        animator.SetBool("Moving", aiState != AIState.Idle); // 상태가 대기가 아니면 무빙 셋 

        switch (aiState)
        {
            case AIState.Idle:
                PassiveUpdate();
                break;
            case AIState.Wandering:
                PassiveUpdate();
                break;
            case AIState.Attacking:
                AttackingUpdate();
                break;
        }
    }

    private void SetState(AIState state)
    {
        aiState = state;

        switch (aiState)
        {
            case AIState.Idle:
                // 대기
                agent.speed = walkSpeed;
                agent.isStopped = true; // 정지상태 유무
                break;
            case AIState.Wandering:
                // 거닐기
                agent.speed = walkSpeed;
                agent.isStopped = false;
                break;
            case AIState.Attacking:
                // 공격
                agent.speed = runSpeed; // 좀더 빠르게 
                agent.isStopped = false;
                break;
        }

        animator.speed = agent.speed / walkSpeed;
    }

    void PassiveUpdate()
    {
        // 공격중이 아니면 항상 호출
        // remainingDistance : 목표지점까지 남은 거리
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle); // 잠깐 멈추기
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
            // 랜덤한 시간에 대기하다가, WanderToNewLocation 함수 호출
        }

        // 플레이어가 가까워지면
        if (playerDistance < detectDistance)
        {
            // 공격 상태로 바꿈 -> 공격업데이트 함수가 실행되게 됨
            SetState(AIState.Attacking);
        }
    }

    void AttackingUpdate()
    {
        // 공격중 일때 호출하는 업데이트 함수

        if (playerDistance < attackDistance && IsPlayerInFieldOfView()) // 플레이어가 가까워져있고 시야 안에 있음
        {
            agent.isStopped = true;
            // 공격 시간 체크
            if (Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;
                CharacterManager.Instance.Player.controller.GetComponent<IDamagable>().TakePhysicalDamage(damage); // 데미지 입게 됨
                animator.speed = 1;
                animator.SetTrigger("Attack");
            }
        }
        else
        {
            if(playerDistance < detectDistance) // 플레이어가 탐색 거리 안에있으면
            {
                agent.isStopped = false;
                NavMeshPath path = new NavMeshPath();
                // 플레이어 위치가 갈 수있는 길인지 계산
                if (agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path))
                {
                    // 목표 지점, 플레이어 추적
                    agent.SetDestination(CharacterManager.Instance.Player.transform.position);
                }
                else
                {
                    // 추적 못할 곳이니 멈추고 추적 포기
                    agent.SetDestination(transform.position);
                    agent.isStopped = true;
                    SetState(AIState.Wandering);
                }
            }
            else
            {
                // 탐색 거리 밖을 벗어났으니 포기
                agent.SetDestination(transform.position);
                agent.isStopped = true;
                SetState(AIState.Wandering);
            }
        }
    }

    void WanderToNewLocation()
    {
        // 반복적으로 다음 목표지점을 호출해주는 함수
        if (aiState != AIState.Idle)
        {
            return;
        }
        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation()); // 목표지점 정해주는 함수
    }

    bool IsPlayerInFieldOfView()
    {
        // 시야각 검사 함수

        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position; // 목표지점에서 내 위치를 빼면 방향이 나옴
        float angle = Vector3.Angle(transform.forward, directionToPlayer); // 정면에서 바라보고있는 위치서 플레이어 위치와의 각을 구함
        return angle < fieldOfView * 0.5f; // 양쪽으로 갈라서 반씩 나눔
    }

    Vector3 GetWanderLocation()
    {
        // 목표지점 정해주는 함수

        NavMeshHit hit;

        // 포지션을 알려주면 이동할 수 있는 한 최단거리를 반환
        // SamplePosition(Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int areaMask)
        // onUnitSphere : 반지름이 1인 구 (이정도 영역 범위)
        // NavMesh.AllAreas : 모든 영역 
        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);

        int i = 0;
        while (Vector3.Distance(transform.position, hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30) // 30번 정도 시도
                break;
        }

        return hit.position;
    }

    float GetDestinationAngle(Vector3 targetPos)
    {
        return Vector3.Angle(transform.position - CharacterManager.Instance.Player.transform.position, transform.position + targetPos);
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
            Die();

        // 데미지 효과
        StartCoroutine(DamageFlash());
    }

    void Die()
    {
        for (int i = 0; i < dropOnDeath.Length; i++)
        {
            // 죽게되면 아이템 꺼냄
            Instantiate(dropOnDeath[i].dropPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    IEnumerator DamageFlash()
    {
        for (int i = 0; i < meshRenderers.Length; i++)
            meshRenderers[i].material.color = new Color(1.0f, 0.6f, 0.6f);

        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < meshRenderers.Length; i++)
            meshRenderers[i].material.color = Color.white;
    }
}
