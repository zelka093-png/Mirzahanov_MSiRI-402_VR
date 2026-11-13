using UnityEngine;
using Valve.VR.InteractionSystem;

// Ходьба без шлема: W A S D – шаги по полу, Shift – быстрее, правая кнопка мыши – повернуть голову.
// Используется CharacterController: он не даёт пройти сквозь стены и станок и держит игрока на полу.
// Скрипт висит на запасной камере Player (FallbackObjects), со шлемом этот объект выключен.
[RequireComponent(typeof(CharacterController))]
public class PcWalk : MonoBehaviour
{
    public float speed = 2.2f;
    public float fastSpeed = 4f;
    public float lookSpeed = 3f;
    public float eyeHeight = 1.65f;

    private CharacterController body;
    private float yaw, pitch, fall;

    private void Start()
    {
        // встроенный «полёт» камеры SteamVR выключаем
        var fly = GetComponent<FallbackCameraController>();
        if (fly) fly.enabled = false;

        body = GetComponent<CharacterController>();
        body.height = eyeHeight;
        body.radius = 0.25f;
        body.center = new Vector3(0f, -eyeHeight / 2f + 0.1f, 0f);   // капсула от пола до глаз

        Vector3 p = transform.position; p.y = eyeHeight; transform.position = p;
        yaw = transform.eulerAngles.y;
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * lookSpeed;
            pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * lookSpeed, -70f, 70f);
        }
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 forward = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;
        Vector3 right = Quaternion.Euler(0f, yaw, 0f) * Vector3.right;
        Vector3 move = (forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal")).normalized;
        move *= Input.GetKey(KeyCode.LeftShift) ? fastSpeed : speed;

        fall = body.isGrounded ? -1f : fall - 9.8f * Time.deltaTime;   // прижимает к полу
        body.Move((move + Vector3.up * fall) * Time.deltaTime);
    }
}
