using System.Collections;
using UnityEngine;

public class PlayMenuController : MonoBehaviour
{
    public Animator buttonsAnim;

    private bool isOpen = false;
    private bool isPlaying = false;

    public void OnPlayClick()
    {
        StartCoroutine(HandleMenu());
    }

    IEnumerator HandleMenu()
    {
        if (isPlaying) yield break; 
        isPlaying = true;

    
        buttonsAnim.ResetTrigger("show");
        buttonsAnim.ResetTrigger("hide");

        if (!isOpen)
        {
            buttonsAnim.SetTrigger("show");
        }
        else
        {
            buttonsAnim.SetTrigger("hide");
        }

        isOpen = !isOpen;

      
        yield return new WaitForSeconds(0.3f);

        isPlaying = false;
    }

    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}