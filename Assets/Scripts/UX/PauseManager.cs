using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject tutorialPanel;

    void Start()
    {
        pausePanel.SetActive(false);
        tutorialPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance.CurrentState == GameManager.GameState.Paused)
                Resume();
            else if (GameManager.Instance.CurrentState == GameManager.GameState.Playing)
                Pause();
        }
    }

    public void Pause()
    {
        Debug.Log("PAUSE CALLED");

        pausePanel.SetActive(true);
        tutorialPanel.SetActive(false);

        GameManager.Instance.PauseGame();
    }

    public void Resume()
    {
        
        pausePanel.SetActive(false);
        tutorialPanel.SetActive(false);

        StartCoroutine(ClearSelection());
        GameManager.Instance.ResumeGame();
    }

    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
        pausePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void BackToPause()
    {
        tutorialPanel.SetActive(false);
        pausePanel.SetActive(true);

        StartCoroutine(ClearSelection());
    }

    IEnumerator ClearSelection()
    {
        EventSystem.current.SetSelectedGameObject(null);

        yield return null;

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}