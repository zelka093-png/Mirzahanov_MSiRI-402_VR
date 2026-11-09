
using UnityEngine;

public class SawRotation : MonoBehaviour
{
    public float rotationSpeed = 500f;

    void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
