using UnityEngine;
using UnityEngine.EventSystems;
public class SelectionUIController : MonoBehaviour
{


    public void LoadScene(string sceneName)
    {
        SceneTransitionManager.LoadLevel(sceneName);
    }

   
   
}
