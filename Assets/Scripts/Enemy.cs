using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3.0f; 
    private Rigidbody enemyRb;
    private GameObject player;

    void Start()
    { 
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
    }

    void Update()
    {
         
        if (player != null)
        {
            // Calculate direction = (player.position - self.position).normalized
            Vector3 lookDirection = (player.transform.position - transform.position).normalized;

            // Apply AddForce toward the player 
            enemyRb.AddForce(lookDirection * speed);
        }

        // Destroy enemy if it falls below Y < -10 
        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }
}