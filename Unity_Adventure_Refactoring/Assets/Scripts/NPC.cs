using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Wandering, // ���ƴٴϴ� ����
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
    public float minWanderDistance; // �ּ� �Ÿ�
    public float maxWanderDistance; // �ִ� �Ÿ�
    public float minWanderWaitTime; // �Ŵұ� ��� �ּ� �ð�(���ο� ��ǥ���� ���������� ���)
    public float maxWanderWaitTime; // �Ŵұ� ��� �ִ� �ð�

    [Header("Combat")]
    public int damage;
    public float attackRate; // �����ֱ�
    private float lastAttackTime;
    public float attackDistance; // ���ݰ����� �Ÿ�

    private float playerDistance; // �÷��̾���� �Ÿ�

    public float fieldOfView = 120f; // ������ �þ߰�
    
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
        SetState(AIState.Wandering); // ������ �Ŵұ�
    }

    private void Update()
    {
        playerDistance = Vector3.Distance(transform.position, CharacterManager.Instance.Player.transform.position);
        // �÷��̾���� �Ÿ�

        animator.SetBool("Moving", aiState != AIState.Idle); // ���°� ��Ⱑ �ƴϸ� ���� �� 

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
                // ���
                agent.speed = walkSpeed;
                agent.isStopped = true; // �������� ����
                break;
            case AIState.Wandering:
                // �Ŵұ�
                agent.speed = walkSpeed;
                agent.isStopped = false;
                break;
            case AIState.Attacking:
                // ����
                agent.speed = runSpeed; // ���� ������ 
                agent.isStopped = false;
                break;
        }

        animator.speed = agent.speed / walkSpeed;
    }

    void PassiveUpdate()
    {
        // �������� �ƴϸ� �׻� ȣ��
        // remainingDistance : ��ǥ�������� ���� �Ÿ�
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle); // ��� ���߱�
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
            // ������ �ð��� ����ϴٰ�, WanderToNewLocation �Լ� ȣ��
        }

        // �÷��̾ ���������
        if (playerDistance < detectDistance)
        {
            // ���� ���·� �ٲ� -> ���ݾ�����Ʈ �Լ��� ����ǰ� ��
            SetState(AIState.Attacking);
        }
    }

    void AttackingUpdate()
    {
        // ������ �϶� ȣ���ϴ� ������Ʈ �Լ�

        if (playerDistance < attackDistance && IsPlayerInFieldOfView()) // �÷��̾ ��������ְ� �þ� �ȿ� ����
        {
            agent.isStopped = true;
            // ���� �ð� üũ
            if (Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;
                CharacterManager.Instance.Player.controller.GetComponent<IDamagable>().TakePhysicalDamage(damage); // ������ �԰� ��
                animator.speed = 1;
                animator.SetTrigger("Attack");
            }
        }
        else
        {
            if(playerDistance < detectDistance) // �÷��̾ Ž�� �Ÿ� �ȿ�������
            {
                agent.isStopped = false;
                NavMeshPath path = new NavMeshPath();
                // �÷��̾� ��ġ�� �� ���ִ� ������ ���
                if (agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path))
                {
                    // ��ǥ ����, �÷��̾� ����
                    agent.SetDestination(CharacterManager.Instance.Player.transform.position);
                }
                else
                {
                    // ���� ���� ���̴� ���߰� ���� ����
                    agent.SetDestination(transform.position);
                    agent.isStopped = true;
                    SetState(AIState.Wandering);
                }
            }
            else
            {
                // Ž�� �Ÿ� ���� ������� ����
                agent.SetDestination(transform.position);
                agent.isStopped = true;
                SetState(AIState.Wandering);
            }
        }
    }

    void WanderToNewLocation()
    {
        // �ݺ������� ���� ��ǥ������ ȣ�����ִ� �Լ�
        if (aiState != AIState.Idle)
        {
            return;
        }
        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation()); // ��ǥ���� �����ִ� �Լ�
    }

    bool IsPlayerInFieldOfView()
    {
        // �þ߰� �˻� �Լ�

        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position; // ��ǥ�������� �� ��ġ�� ���� ������ ����
        float angle = Vector3.Angle(transform.forward, directionToPlayer); // ���鿡�� �ٶ󺸰��ִ� ��ġ�� �÷��̾� ��ġ���� ���� ����
        return angle < fieldOfView * 0.5f; // �������� ���� �ݾ� ����
    }

    Vector3 GetWanderLocation()
    {
        // ��ǥ���� �����ִ� �Լ�

        NavMeshHit hit;

        // �������� �˷��ָ� �̵��� �� �ִ� �� �ִܰŸ��� ��ȯ
        // SamplePosition(Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int areaMask)
        // onUnitSphere : �������� 1�� �� (������ ���� ����)
        // NavMesh.AllAreas : ��� ���� 
        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);

        int i = 0;
        while (Vector3.Distance(transform.position, hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30) // 30�� ���� �õ�
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

        // ������ ȿ��
        StartCoroutine(DamageFlash());
    }

    void Die()
    {
        for (int i = 0; i < dropOnDeath.Length; i++)
        {
            // �װԵǸ� ������ ����
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
