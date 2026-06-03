using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    [SerializeField] private Image energyBar;

    [Header("Sprites (0 → 4 energy)")]
    [SerializeField] private Sprite[] energySprites;

    [SerializeField] private PlayerController playerController;

    private void Start()
    {
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
        }
    }

    private void Update()
    {
        if (playerController != null)
        {
            UpdateEnergyBar();
        }
    }

    private void UpdateEnergyBar()
    {
        if (energyBar == null || energySprites == null) return;

        int currentEnergy = playerController.GetCurrentEnergy();

        // clamp để tránh lỗi
        currentEnergy = Mathf.Clamp(currentEnergy, 0, energySprites.Length - 1);

        energyBar.sprite = energySprites[currentEnergy];
    }
}