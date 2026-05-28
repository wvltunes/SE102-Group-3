using UnityEngine;
using UnityEngine.EventSystems;
public class SelectionUIController : MonoBehaviour
{


    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

   
   
}
