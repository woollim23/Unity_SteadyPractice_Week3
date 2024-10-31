using UnityEngine;

public class MusicZone : MonoBehaviour
{
    public AudioSource audioSource;
    public float fadeTime;
    public float maxVolume;
    private float targetVolume;

    private void Start()
    {
        targetVolume = 0.0f;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = targetVolume;
        audioSource.Play();

    }

    private void Update()
    {
        // Mathf.Approximately : 근사값이면 같은 값으로 인식
        // 근사값이 아닐때
        if (!Mathf.Approximately(audioSource.volume, targetVolume))
        {
            // Mathf.MoveTowards : 점진적으로 늘어남
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, (maxVolume / fadeTime) * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            targetVolume = maxVolume;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            targetVolume = 0.0f;
    }
}
