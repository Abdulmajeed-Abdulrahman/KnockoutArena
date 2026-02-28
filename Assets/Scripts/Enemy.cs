using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] 
    [Range(1.0f, 10.0f)] 
    [Tooltip("Controls how fast the enemy chases the player.")]
    private float speed = 3.0f; 
    
    private Rigidbody enemyRb;
    private GameObject player;

    private GameManager gameManager;

    void Start()
    { 
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
            gameManager.UpdateScore(10);
            Destroy(gameObject);
        }
    }
}