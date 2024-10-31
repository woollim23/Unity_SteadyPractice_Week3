using UnityEngine;

public class FootSteps : MonoBehaviour
{
    public AudioClip[] footstepClips;
    private AudioSource audioSource;
    private Rigidbody _rigidbody;
    public float footstepThreshold; // 리지드 바디에서 어느정도 움직였을때의 기준점
    public float footstepRate; // 간격
    private float footStepTime;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Mathf.Abs(_rigidbody.velocity.y) < 0.1f)
        {
            // magnitude 크기값
            if (_rigidbody.velocity.magnitude > footstepThreshold)
            {
                if (Time.time - footStepTime > footstepRate)
                {
                    footStepTime = Time.time;
                    audioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]);
                }
            }
        }
    }
}
