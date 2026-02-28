using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static event Action OnGameOver;

    [Header("UI References")]
    [SerializeField] [Tooltip("Text element for displaying current score.")] 
    private TextMeshProUGUI scoreText;

    [SerializeField] [Tooltip("Text element for displaying current wave number.")] 
    private TextMeshProUGUI waveText;

    [SerializeField] private GameObject titleScreen;

    [Header("Advanced Extension")]
    [SerializeField] private CanvasGroup gameOverCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private int score = 0;

    [HideInInspector]
    public bool isGameActive = false;


    private void Start()
    {
        UpdateScore(0);
    }

// Public method called by the DifficultyButtons
    public void StartGame(int difficulty)
    {
        isGameActive = true;
        score = 0;
        UpdateScore(0);

        titleScreen.SetActive(false);
        
        // Trigger your first wave here instead of in SpawnManager's Start
        GameObject.Find("SpawnManager").GetComponent<SpawnManager>().StartInitialWave();
    }

    public void UpdateScore(int scoreToAdd)
    {
        if (isGameActive)
        {
            score += scoreToAdd;
            scoreText.text = "Score: " + score;

            StartCoroutine(PunchScale(scoreText.transform, 1.2f, 0.1f));
        }
    }

    public void UpdateWave(int waveNumber)
    {
        if (isGameActive)
        {
            waveText.text = "Wave: " + waveNumber;
        }
    }

    public void TriggerGameOver()
    {
        if (isGameActive)
        {
            isGameActive = false;

            // Activate Game Over UI so the coroutine can run
            gameOverCanvasGroup.gameObject.SetActive(true);

            // Start the fade-in animation
            StartCoroutine(FadeIn(gameOverCanvasGroup, fadeDuration));
            
            if (OnGameOver != null)
            {
                OnGameOver();
                Debug.Log("Game Over!");
            }
        }
    }

    // Public method for the Restart Button to call 
    public void RestartGame()
    { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Coroutine using while loop and Time.deltaTime
    IEnumerator FadeIn(CanvasGroup cg, float dur)
    {
        float t = 0;
        while (t < dur)
        {
            t += Time.deltaTime;
            cg.alpha = t / dur; // Calculate progress from 0 to 1
            yield return null; // Wait for the next frame
        }
        cg.alpha = 1; // Ensure it's fully visible at the end
    }

    private void PulseScore()
    {
        StopCoroutine("PunchScale"); // Stop existing pulse to prevent overlapping
        StartCoroutine(PunchScale(scoreText.transform, 1.2f, 0.1f));
    }

    IEnumerator PunchScale(Transform target, float scaleUp, float dur)
    {
        Vector3 initialScale = Vector3.one;
        Vector3 targetScale = Vector3.one * scaleUp;

        float t = 0;
        // Scale Up
        while (t < dur)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(initialScale, targetScale, t / dur);
            yield return null;
        }

        t = 0;
        // Scale Down
        while (t < dur)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(targetScale, initialScale, t / dur);
            yield return null;
        }
        target.localScale = initialScale;
    }
}