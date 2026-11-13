
using UnityEngine;

public class MachineController : MonoBehaviour
{
    public Transform saw;
    public Renderer workpiece;

    public float rotationSpeed = 500f;

    private bool isRunning = false;

    void Update()
    {
        if (isRunning)
        {
            saw.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
    }

    public void StartMachine()
    {
        isRunning = true;

        if (workpiece != null)
        {
            workpiece.material.color = Color.green;
        }
    }

    public void StopMachine()
    {
        isRunning = false;
    }
}
