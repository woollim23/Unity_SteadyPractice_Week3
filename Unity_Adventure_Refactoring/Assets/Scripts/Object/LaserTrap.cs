using UnityEngine;

public class LaserTrap : MonoBehaviour
{
    public Transform startPoint; // 레이저 시작점
    public Transform endPoint; // 레이저 끝점
    public LayerMask playerLayer; // 플레이어 레이어
    [SerializeField] private int laserDamage = 1000;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            return;
        }

        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);
    }

    void Update()
    {
        // 레이저를 그리는 부분
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);

        // 레이저 감지
        RaycastHit hit;
        if (Physics.Linecast(startPoint.position, endPoint.position, out hit, playerLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // 플레이어가 레이저를 통과했을 때의 행동
                ActivateTrap();
            }
        }
    }

    void ActivateTrap()
    {
        // 경고 메시지 출력
        Debug.Log("즉사 레이저에 감지되었습니다!");
        CharacterManager.Instance.Player.condition.TakePhysicalDamage(laserDamage);
    }
}
