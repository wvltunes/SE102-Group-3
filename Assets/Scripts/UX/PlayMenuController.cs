using System.Collections;
using UnityEngine;

public class PlayMenuController : MonoBehaviour
{
    public CanvasGroup optionCanvas;
    public Animator buttonsAnim;
    public OptionController optionController; 

    private bool isOpen = false;
    private bool isPlaying = false;
    private bool isOptionsOpen = false;

    void Start()
    {
        optionCanvas.alpha = 0;
        optionCanvas.interactable = false;
        optionCanvas.blocksRaycasts = false;
        isOptionsOpen = false;

        if (optionController != null)
            optionController.gameObject.SetActive(false);
    }

    public void OnPlayClick()
    {
        if (isPlaying) return;
        StartCoroutine(HandleMenu());
    }

    public void OnOptionsClick()
    {
        if (!isOptionsOpen)
        {
            optionCanvas.alpha = 1;
            optionCanvas.interactable = true;
            optionCanvas.blocksRaycasts = true;
            isOptionsOpen = true;

            if (optionController != null)
            optionController.gameObject.SetActive(true);
        }
        else
        {
            optionCanvas.alpha = 0;
            optionCanvas.interactable = false;
            optionCanvas.blocksRaycasts = false;
            isOptionsOpen = false;

            
            if (optionController != null)
                optionController.gameObject.SetActive(false);
        }
    }
    public void CloseOptions()
    {
        optionCanvas.alpha = 0;
        optionCanvas.interactable = false;
        optionCanvas.blocksRaycasts = false;
        isOptionsOpen = false;

        if (optionController != null)
            optionController.gameObject.SetActive(false);
    }
    IEnumerator HandleMenu()
    {
        isPlaying = true;

        if (!isOpen)
        {
            buttonsAnim.ResetTrigger("hide");
            buttonsAnim.SetTrigger("show");
            isOpen = true;
        }
        else
        {
            buttonsAnim.ResetTrigger("show");
            buttonsAnim.SetTrigger("hide");
            yield return new WaitForSeconds(0.3f);
            isOpen = false;
        }

        isPlaying = false;
    }
}