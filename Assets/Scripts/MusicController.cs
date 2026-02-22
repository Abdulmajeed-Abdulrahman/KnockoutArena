using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioSource backgroundMusic;

    void Start()
    {
        backgroundMusic = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += StopMusic;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= StopMusic;
    }

    void StopMusic()
    {
        backgroundMusic.Stop(); // Music stops when player falls 
        Debug.Log("Music stopped via Event.");
    }
}