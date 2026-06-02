using UnityEngine;

public class SceneTransitionStartup : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(FadeInNextFrame());
    }

    private System.Collections.IEnumerator FadeInNextFrame()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("[SceneTransitionStartup] Scene loaded, triggering fade-in.");
        SceneTransitionManager.DoFadeIn();
    }
}