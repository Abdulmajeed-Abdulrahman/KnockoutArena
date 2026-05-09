using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static event Action OnGameOver;

    [Header("UI References")]
    [SerializeField] [Tooltip("Text element for displaying current score.")] 
    private TextMeshProUGUI scoreText;

    [SerializeField] [Tooltip("Text element for displaying current wave number.")] 
    private TextMeshProUGUI waveText;

    [SerializeField] private GameObject titleScreen;

    [Header("Visual Enhancements (Assignment 3)")]
    [SerializeField] [Tooltip("Reference to the script on the Arena that changes its color.")]
    private ArenaColorShifter colorShifter;

    [Header("Section 2: Lighting")]
    [SerializeField] [Tooltip("The 4 Spot Lights at the corners of the arena.")]
    private Light[] edgeLights;

    [SerializeField] [Tooltip("How much edge light intensity increases per wave.")]
    private float lightGrowthFactor = 0.5f;

    [Header("Section 4: VFX")]
    [SerializeField] [Tooltip("Particle system that bursts when a new wave starts.")]
    private ParticleSystem waveDustEffect;

    [Header("Advanced Extension")]
    [SerializeField] private CanvasGroup gameOverCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private int score = 0;
    private float initialEdgeIntensity;

    [HideInInspector]
    public bool isGameActive = false;


    private void Start()
    {
        UpdateScore(0);
        
        // Store the starting intensity of lights
        if (edgeLights != null && edgeLights.Length > 0)
        {
            initialEdgeIntensity = edgeLights[0].intensity;
        }
    }

    public void StartGame(int difficulty)
    {
        isGameActive = true;
        score = 0;
        UpdateScore(0);

        titleScreen.SetActive(false);
        
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

            // Section 1: Shaders and Materials
            if (colorShifter != null)
            {
                colorShifter.ChangeArenaColor(waveNumber);
            }

            // Section 2: Lighting
            if (edgeLights != null)
            {
                foreach (Light light in edgeLights)
                {
                    if (light != null)
                    {
                        light.intensity = initialEdgeIntensity + (waveNumber * lightGrowthFactor);
                    }
                }
            }

            // Section 4: VFX - Play the dust burst when a new wave starts
            if (waveDustEffect != null)
            {
                waveDustEffect.Play();
            }
        }
    }

    public void TriggerGameOver()
    {
        if (isGameActive)
        {
            isGameActive = false;
            gameOverCanvasGroup.gameObject.SetActive(true);
            StartCoroutine(FadeIn(gameOverCanvasGroup, fadeDuration));
            
            if (OnGameOver != null)
            {
                OnGameOver();
            }
        }
    }

    public void RestartGame()
    { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator FadeIn(CanvasGroup cg, float dur)
    {
        float t = 0;
        while (t < dur)
        {
            t += Time.deltaTime;
            cg.alpha = t / dur;
            yield return null;
        }
        cg.alpha = 1;
    }

    IEnumerator PunchScale(Transform target, float scaleUp, float dur)
    {
        Vector3 initialScale = Vector3.one;
        Vector3 targetScale = Vector3.one * scaleUp;

        float t = 0;
        while (t < dur)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(initialScale, targetScale, t / dur);
            yield return null;
        }

        t = 0;
        while (t < dur)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(targetScale, initialScale, t / dur);
            yield return null;
        }
        target.localScale = initialScale;
    }
}