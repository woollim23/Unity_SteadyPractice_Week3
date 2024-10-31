using UnityEngine;

public class LaserTrap : MonoBehaviour
{
    public Transform startPoint; // ������ ������
    public Transform endPoint; // ������ ����
    public LayerMask playerLayer; // �÷��̾� ���̾�
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
        // �������� �׸��� �κ�
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);

        // ������ ����
        RaycastHit hit;
        if (Physics.Linecast(startPoint.position, endPoint.position, out hit, playerLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // �÷��̾ �������� ������� ���� �ൿ
                ActivateTrap();
            }
        }
    }

    void ActivateTrap()
    {
        // ��� �޽��� ���
        Debug.Log("��� �������� �����Ǿ����ϴ�!");
        CharacterManager.Instance.Player.condition.TakePhysicalDamage(laserDamage);
    }
}
