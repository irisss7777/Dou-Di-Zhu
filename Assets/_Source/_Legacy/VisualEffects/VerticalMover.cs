using UnityEngine;

public class VerticalMover : MonoBehaviour
{
    public float amplitude = 1f;
    public float speed = 1f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = startPosition + new Vector3(0, offset, 0);
    }
}