using UnityEngine;

// Зона, куда нужно положить деталь (на станок или на стол продукции).
// Проверяет, попала ли точка (центр детали) внутрь BoxCollider этой зоны.
[RequireComponent(typeof(BoxCollider))]
public class DropZone : MonoBehaviour
{
    private BoxCollider box;

    private void Awake()
    {
        box = GetComponent<BoxCollider>();
        box.isTrigger = true;   // зона не мешает руке и детали
    }

    public bool Contains(Vector3 worldPoint)
    {
        if (box == null) box = GetComponent<BoxCollider>();
        Vector3 local = transform.InverseTransformPoint(worldPoint) - box.center;
        Vector3 half = box.size / 2f;
        return Mathf.Abs(local.x) <= half.x && Mathf.Abs(local.y) <= half.y && Mathf.Abs(local.z) <= half.z;
    }

    // в окне Scene зона видна полупрозрачным зелёным кубом
    private void OnDrawGizmos()
    {
        var b = GetComponent<BoxCollider>();
        if (b == null) return;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(0.2f, 0.9f, 0.3f, 0.25f);
        Gizmos.DrawCube(b.center, b.size);
    }
}
