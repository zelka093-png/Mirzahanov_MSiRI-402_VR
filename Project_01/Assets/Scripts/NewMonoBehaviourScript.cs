
using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float x = 0;
        float z = 0;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) z = 1;
            if (Keyboard.current.sKey.isPressed) z = -1;
            if (Keyboard.current.aKey.isPressed) x = -1;
            if (Keyboard.current.dKey.isPressed) x = 1;

            // Прыжок
            if (Keyboard.current.spaceKey.wasPressedThisFrame && IsGrounded())
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }

        Vector3 movement = new Vector3(x, 0, z).normalized;
        transform.Translate(movement * speed * Time.deltaTime);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
