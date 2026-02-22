using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject powerupPrefab;
    private float spawnRange = 9.0f;
    
    public int enemyCount;
    public int waveNumber = 1;

    private bool isGameOver = false;

    void Start()
    {
        // Initial spawn call
        SpawnEnemyWave(waveNumber);
        Instantiate(powerupPrefab, GenerateSpawnPosition(), powerupPrefab.transform.rotation);
    }

    void Update()
    {
        if (isGameOver)
        {
            return;
        }

        // Detect when all enemies are defeated
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemyCount == 0)
        {
            waveNumber++; // Increment wave number
            SpawnEnemyWave(waveNumber); // Spawn more enemies than previous wave
            
            // Spawn a new powerup for the new wave
            Instantiate(powerupPrefab, GenerateSpawnPosition(), powerupPrefab.transform.rotation);
        }
    }

    // Method with a for-loop to instantiate 'count' enemies
    void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
        }
    }

    // Separate method with a Vector3 return type for random positions
    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);
        
        Vector3 randomPos = new Vector3(spawnPosX, 1, spawnPosZ);
        return randomPos;
    }

    private void OnEnable()
    {
        // Subscribe to the event 
        GameManager.OnGameOver += StopSpawning;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event 
        GameManager.OnGameOver -= StopSpawning;
    }

    void StopSpawning()
    {
        isGameOver = true; 
        Debug.Log("SpawnManager has stopped spawning.");
    }
}