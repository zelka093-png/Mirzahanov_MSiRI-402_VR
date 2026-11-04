
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(gameObject.name + " столкнулся с " + collision.gameObject.name);

        Renderer rend = GetComponent<Renderer>();

        if (rend != null)
        {
            rend.material.color = Random.ColorHSV();
        }
    }
}
