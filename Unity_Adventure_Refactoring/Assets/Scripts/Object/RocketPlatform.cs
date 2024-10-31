using UnityEngine;

public class RocketPlatform : MonoBehaviour
{
    public float launchDelay = 5f; // 발사까지의 대기 시간
    public float launchForce = 5000f; // 발사 힘

    private bool isPlayerOnPlatform = false; // 플레이어가 플랫폼 위에 있는지 확인
    Rigidbody playerRigidbody; // Rigidbody 컴포넌트
    private float timer = 0f;


    void Update()
    {
        if (isPlayerOnPlatform)
        {
            timer += Time.deltaTime;

            // 지정된 대기 시간이 지나면 발사
            if (timer >= launchDelay)
            {
                Launch();
            }
        }
    }
    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        // Player 인지 확인
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
        // 플레이어가 플랫폼을 떠나면 리셋
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
            isPlayerOnPlatform = false; // 발사 후 재발사 방지
            timer = 0f;
        }
    }
}