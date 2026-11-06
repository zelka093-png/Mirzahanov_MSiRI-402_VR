
using UnityEngine;
using UnityEngine.InputSystem;

public class FigureClickPush : MonoBehaviour
{
    public float pushForce = 12f;
    public float upwardForce = 4f;

    void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(
                Mouse.current.position.ReadValue()
            );

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    Vector3 direction = ray.direction;

                    rb.AddForce(
                        direction * pushForce +
                        Vector3.up * upwardForce,
                        ForceMode.Impulse
                    );

                    rb.AddTorque(
                        Random.insideUnitSphere * 5f,
                        ForceMode.Impulse
                    );
                }
            }
        }
    }
}
