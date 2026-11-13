
using UnityEngine;

public class WorkpieceSnap : MonoBehaviour
{
    public Transform attachPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Workpiece"))
            return;

        Rigidbody rb = other.GetComponent<Rigidbody>();

        other.transform.SetParent(attachPoint);
        other.transform.localPosition = Vector3.zero;
        other.transform.localRotation = Quaternion.identity;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }
    }
}
