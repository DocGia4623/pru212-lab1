using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}