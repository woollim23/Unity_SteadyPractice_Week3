using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    private Vector3 startPosition;
    private Transform curPosition;

    private bool isRight;

    public float maxMovingDistance;
    public float movingSpeed;

    private void Start()
    {
        startPosition = transform.position;
        curPosition = transform;
        isRight = true;
    }
    private void Update()
    {
        Moving();
    }

    void Moving()
    {
        if (isRight)
        {
            curPosition.position = new Vector3(curPosition.position.x - movingSpeed, transform.position.y, transform.position.z);
            if (startPosition.x - curPosition.position.x >= maxMovingDistance) isRight = false;
        }
        else
        {
            curPosition.position = new Vector3(curPosition.position.x + movingSpeed, transform.position.y, transform.position.z);
            if (startPosition.x <= curPosition.position.x) isRight = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        collision.transform.parent = curPosition;
    }
    private void OnCollisionExit(Collision collision)
    {
        collision.transform.parent = null;
    }
}
