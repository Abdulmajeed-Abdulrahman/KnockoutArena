using UnityEngine;
using System; 

public class GameManager : MonoBehaviour
{
    public static event Action OnGameOver;

    public static void TriggerGameOver()
    {
        if (OnGameOver != null)
        {
            OnGameOver();
            Debug.Log("Game Over!");
        }
    }
}