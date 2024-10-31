using Unity.VisualScripting;
using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    private float jumpForce = 100f;
    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        // Player ���� Ȯ��
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRigidbody != null)
            {
                playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }
}
