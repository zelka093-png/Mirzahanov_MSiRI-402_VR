
using UnityEngine;

public class FigureSpawner : MonoBehaviour
{
    public GameObject[] figures;
    public float spacing = 3f;

    void Start()
    {
        for (int i = 0; i < figures.Length; i++)
        {
            Vector3 position = new Vector3(i * spacing, 3f, 0f);

            GameObject newFigure = Instantiate(
                figures[i],
                position,
                Quaternion.identity
            );

            newFigure.name = figures[i].name;

            if (newFigure.GetComponent<Rigidbody>() == null)
                newFigure.AddComponent<Rigidbody>();
Rigidbody rb = newFigure.GetComponent<Rigidbody>();

rb.mass = 1f;
rb.linearDamping = 0.2f;
rb.angularDamping = 0.1f;

rb.AddForce(
    new Vector3(
        Random.Range(-4f, 4f),
        Random.Range(1f, 3f),
        Random.Range(-4f, 4f)
    ),
    ForceMode.Impulse
);
            if (newFigure.GetComponent<CollisionHandler>() == null)
                newFigure.AddComponent<CollisionHandler>();
        }
    }
}

