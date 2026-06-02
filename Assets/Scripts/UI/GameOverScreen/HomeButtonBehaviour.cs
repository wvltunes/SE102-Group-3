using UnityEngine;

public class HomeButtonBehaviour : MonoBehaviour
{
    [Tooltip("Name of the main menu scene to return to (must be in Build Settings).")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public void HomeButtonOnClick()
    {
        // The game is frozen on game-over. SceneTransitionManager restores normal time
        // before loading, otherwise the main menu would load while still paused.
        SceneTransitionManager.LoadLevel(mainMenuSceneName);
    }
}
