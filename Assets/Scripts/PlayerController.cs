using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float gravityModifier = 1.0f; 
    
    private Rigidbody playerRb;
    private GameObject focalPoint;

    public bool hasPowerup = false;
    public float powerupStrength = 15.0f;
    public GameObject powerupIndicator;

    // Animation reference
    private Animator indicatorAnim;

    // Audio and Particle variables
    public ParticleSystem powerupParticle; // Public for Inspector assignment 
    public AudioClip collisionSound; // Public AudioClip variable 
    private AudioSource playerAudio; // Private variable for caching

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");

        Physics.gravity *= gravityModifier;

        // Get the Animator component from the indicator child object
        indicatorAnim = powerupIndicator.GetComponent<Animator>();

        // AudioSource cached
        playerAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * speed * forwardInput);

        // Lose condition: falling off the edge
        if (transform.position.y < -10)
        {
            GameManager.TriggerGameOver();
            Destroy(gameObject);
        }
    }

    // Detect Powerup Collection
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            powerupIndicator.SetActive(true);

            powerupParticle.Play(); // Triggered programmatically on collect

            // Start the animation state via code 
            indicatorAnim.SetBool("isSpinning", true);

            Destroy(other.gameObject);
            
            // Start the countdown timer
            StartCoroutine(PowerupCountdownRoutine());
        }
    }

    // Coroutine to handle the powerup timer
    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7); // Powerup lasts 7 seconds
        hasPowerup = false;

        // Return to idle animation state via code 
        indicatorAnim.SetBool("isSpinning", false);

        powerupIndicator.SetActive(false);
    }

    // Handle Knockback on Collision
    private void OnCollisionEnter(Collision collision)
    {
        // Play 1 sound effect via AudioSource.PlayOneShot 
        playerAudio.PlayOneShot(collisionSound, 1.0f);

        if (collision.gameObject.CompareTag("Enemy") && hasPowerup)
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            
            // Calculate direction away from the player
            Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position;
            
            // Apply immediate knockback force (Impulse)
            enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
        }
    }
}