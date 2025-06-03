using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public float rotatespeed = 180.0f;
    private Vector3 moveDirection = Vector3.down; // Default direction

    // Call this after instantiating the enemy to set its movement angle
    public void SetMoveAngle(float angle)
    {
        // Convert angle (degrees) to radians
        float rad = angle * Mathf.Deg2Rad;
        // Calculate direction: angle 0 = straight down, positive = right, negative = left
        moveDirection = new Vector3(Mathf.Sin(rad), -Mathf.Cos(rad), 0f).normalized;
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
        transform.Rotate(0, 0, rotatespeed * Time.deltaTime);
    }
}