using UnityEngine;

public class VerticalFloor : MonoBehaviour
{
    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position = startPos + new Vector3(0, Mathf.PingPong(Time.time, 3), 0);
    }
}
