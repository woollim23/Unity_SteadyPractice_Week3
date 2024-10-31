using UnityEngine;

public class RocketPlatform : MonoBehaviour
{
    public float launchDelay = 5f; // �߻������ ��� �ð�
    public float launchForce = 5000f; // �߻� ��

    private bool isPlayerOnPlatform = false; // �÷��̾ �÷��� ���� �ִ��� Ȯ��
    Rigidbody playerRigidbody; // Rigidbody ������Ʈ
    private float timer = 0f;


    void Update()
    {
        if (isPlayerOnPlatform)
        {
            timer += Time.deltaTime;

            // ������ ��� �ð��� ������ �߻�
            if (timer >= launchDelay)
            {
                Launch();
            }
        }
    }
    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        // Player ���� Ȯ��
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                isPlayerOnPlatform = true;
                timer = 0f;
            }
        }
    }

    private void OnCollisionExit(UnityEngine.Collision collision)
    {
        // �÷��̾ �÷����� ������ ����
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = false;
            timer = 0f;
        }
    }

    private void Launch()
    {
        if (playerRigidbody != null)
        {
            playerRigidbody.AddForce(Vector3.forward * launchForce, ForceMode.Impulse);
            isPlayerOnPlatform = false; // �߻� �� ��߻� ����
            timer = 0f;
        }
    }
}