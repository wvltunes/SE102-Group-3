using UnityEngine;

/// <summary>
/// Automatically triggers fade-in when a scene starts.
/// 
/// Place this component in each gameplay/menu scene to ensure the fade-in
/// animation plays after scene load. It calls SceneTransitionManager.DoFadeIn()
/// during Start().
/// 
/// This component handles the fade-in portion of the transition:
/// - Fade out happens BEFORE scene load (in SceneTransitionManager)
/// - Scene loads
/// - This component triggers fade in AFTER scene load
/// 
/// Ensure this script runs early in the scene startup order.
/// </summary>
public class SceneTransitionStartup : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("[SceneTransitionStartup] Scene loaded, triggering fade-in.");
        // Trigger the fade-in animation when this scene starts
        SceneTransitionManager.DoFadeIn();
    }
}
