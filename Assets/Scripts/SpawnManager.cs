using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] [Tooltip("The enemy object to instantiate.")] 
    private GameObject enemyPrefab;

    [SerializeField] [Tooltip("The powerup object to instantiate.")] 
    private GameObject powerupPrefab;

    [Header("Spawn Settings")]
    [SerializeField] [Range(5.0f, 15.0f)] [Tooltip("The radius from center (0,0,0) where objects can spawn.")] 
    private float spawnRange = 9.0f;

    [HideInInspector] public int enemyCount;
    [HideInInspector] public int waveNumber = 1;

    private bool isGameOver = false;
    private bool isGameStarted = false;
    private GameManager gameManager;

    void Start()
    {
        // Cache the GameManager reference
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
    }

    // Called by GameManager.StartGame() when a difficulty button is pressed
    public void StartInitialWave()
    {
        isGameStarted = true; // Allow the Update loop to start checking enemy counts
        
        // Initialize UI display for the first wave
        gameManager.UpdateWave(waveNumber);
        
        // Spawn the first wave
        SpawnEnemyWave(waveNumber);
        Instantiate(powerupPrefab, GenerateSpawnPosition(), powerupPrefab.transform.rotation);
        
        Debug.Log("Initial wave started via GameManager.");
    }

    void Update()
    {
        //nothing happens after Game Over or before the game starts
        if (isGameOver || !isGameStarted) return;

        // Detect when all enemies are defeated 
        enemyCount = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;

        if (enemyCount == 0)
        {
            waveNumber++;
            gameManager.UpdateWave(waveNumber); // Update UI for new wave 
            
            SpawnEnemyWave(waveNumber);
            Instantiate(powerupPrefab, GenerateSpawnPosition(), Quaternion.identity);
        }
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);
        return new Vector3(spawnPosX, 1, spawnPosZ);
    }

    // Events and Delegates for Game Over
    private void OnEnable() => GameManager.OnGameOver += StopSpawning;
    private void OnDisable() => GameManager.OnGameOver -= StopSpawning;

    void StopSpawning() => isGameOver = true;
}