using UnityEngine;
using UnityEngine.UI;

public partial class DifficultyButton : MonoBehaviour
{
    private Button button;
    private GameManager gameManager;

    [SerializeField]
    [Range(1, 3)]
    [Tooltip("1 = Easy, 2 = Medium, 3 = Hard. Affects spawn rates and enemy speed.")]
    private int difficulty;

    void Start()
    {
        button = GetComponent<Button>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Add a listener to call SetDifficulty when the button is clicked
        button.onClick.AddListener(SetDifficulty);
    }

    void SetDifficulty()
    {
        Debug.Log(gameObject.name + " was clicked");
        // Pass the difficulty value to the GameManager to start the game
        gameManager.StartGame(difficulty);
    }
}