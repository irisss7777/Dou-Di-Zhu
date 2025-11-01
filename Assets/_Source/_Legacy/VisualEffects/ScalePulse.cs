using UnityEngine;

public class ScalePulse : MonoBehaviour
{
    public float amplitude = 0.2f;
    public float speed = 2f;

    private Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        float scaleOffset = Mathf.Sin(Time.time * speed) * amplitude;
        transform.localScale = startScale + new Vector3(scaleOffset, scaleOffset, scaleOffset);
    }
}