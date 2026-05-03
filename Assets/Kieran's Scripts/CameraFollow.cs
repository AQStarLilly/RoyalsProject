using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 8f, -6f);

    void LateUpdate()
    {
        transform.position = target.position + offset;
    }
}
