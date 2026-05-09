using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] [Range(1.0f, 20.0f)] [Tooltip("Speed of the player movement via AddForce.")] 
    private float speed = 5.0f;

    [SerializeField] [Tooltip("Multiplied by Physics.gravity in Start.")] 
    private float gravityModifier = 1.0f; 
    
    private Rigidbody playerRb;
    private GameObject focalPoint;

    [Header("Powerup Settings")]
    [HideInInspector] public bool hasPowerup = false; 

    [SerializeField] [Range(5.0f, 30.0f)] [Tooltip("Strength of the knockback impulse applied to enemies.")] 
    private float powerupStrength = 15.0f;

    [SerializeField] private GameObject powerupIndicator;

    // Animation reference for the indicator
    private Animator indicatorAnim;

    [Header("Effects & Audio")]
    [SerializeField] [Tooltip("Particle system triggered upon collecting a powerup.")] 
    private ParticleSystem powerupParticle;

    [SerializeField] private AudioClip collisionSound; 
    private AudioSource playerAudio; 

    [Header("Section 3: Animation (Camera Shake)")]
    [SerializeField] [Tooltip("The Animator attached to the Main Camera.")]
    private Animator cameraAnimator;

    [Header("Section 5: Camera (Dynamic FOV)")]
    [SerializeField] [Tooltip("Reference to the Main Camera.")]
    private Camera mainCamera;

    [SerializeField] [Tooltip("The standard Field of View.")]
    private float normalFOV = 60f;

    [SerializeField] [Tooltip("The Field of View when powered up.")]
    private float powerupFOV = 75f;

    [SerializeField] [Tooltip("How fast the camera transitions between FOV values.")]
    private float fovLerpSpeed = 5f;

    private float targetFOV;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");

        Physics.gravity *= gravityModifier;

        // Get the Animator component from the indicator child object
        indicatorAnim = powerupIndicator.GetComponent<Animator>();

        // AudioSource cached
        playerAudio = GetComponent<AudioSource>();

        // Section 5: Initialize Camera and FOV
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        targetFOV = normalFOV;
    }

    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * speed * forwardInput);

        // Section 5: Smoothly Lerp FOV based on target
        if (mainCamera != null)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
        }

        // Lose condition: falling off the edge
        if (transform.position.y < -10)
        {
            gameManager.TriggerGameOver();
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

            powerupParticle.Play(); 

            // Start the animation state via code 
            indicatorAnim.SetBool("isSpinning", true);

            // Section 5: Trigger FOV change
            targetFOV = powerupFOV;

            Destroy(other.gameObject);
            
            // Start the countdown timer
            StartCoroutine(PowerupCountdownRoutine());
        }
    }

    // Coroutine to handle the powerup timer
    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7); 
        hasPowerup = false;

        // Return to idle animation state via code 
        indicatorAnim.SetBool("isSpinning", false);

        powerupIndicator.SetActive(false);

        // Section 5: Reset FOV back to normal
        targetFOV = normalFOV;
    }

    // Handle Knockback on Collision
    private void OnCollisionEnter(Collision collision)
    {
        // Play collision sound
        playerAudio.PlayOneShot(collisionSound, 1.0f);

        // Section 3: Trigger Camera Shake on ANY collision with an enemy
        if (collision.gameObject.CompareTag("Enemy") && cameraAnimator != null)
        {
            cameraAnimator.SetTrigger("shake");
        }

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