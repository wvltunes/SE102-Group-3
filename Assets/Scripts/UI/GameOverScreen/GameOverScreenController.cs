using UnityEngine;

public class GameOverScreenController : MonoBehaviour
{
    public static bool gameOver = false;
    [SerializeField] private GameObject gameOverScreen;
    void Update()
    {
        //Currently have no event for player death -> manually trigger game over screen with escape key for testing purposes
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gameOver)
            {
                TriggerGameOverScreen();
            }
            else
            {
                CloseGameOverScreen();
            }
            gameOver = !gameOver;
        }

    }

    void TriggerGameOverScreen()
    {
        gameOverScreen.SetActive(true);
    }

    void CloseGameOverScreen()
    {
        gameOverScreen.SetActive(false);
    }

    
}
