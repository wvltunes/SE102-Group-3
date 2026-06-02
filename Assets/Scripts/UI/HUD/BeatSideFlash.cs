using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BeatSideFlash : MonoBehaviour
{
    [SerializeField] private Image leftFlash;
    [SerializeField] private Image rightFlash;
    [SerializeField] private float flashStrength = 0.6f;
    [SerializeField] private float duration = 0.1f;

    public void Flash()
    {
        leftFlash.DOKill();
        rightFlash.DOKill();

        leftFlash.color = new Color(1, 1, 1, 0);
        rightFlash.color = new Color(1, 1, 1, 0);

        leftFlash.DOFade(flashStrength, duration).SetLoops(2, LoopType.Yoyo);
        rightFlash.DOFade(flashStrength, duration).SetLoops(2, LoopType.Yoyo);
    }
}