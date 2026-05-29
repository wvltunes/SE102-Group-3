using UnityEngine;

public class RetryButtonBehaviour : MonoBehaviour
{
    public void RetryButtonOnClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
