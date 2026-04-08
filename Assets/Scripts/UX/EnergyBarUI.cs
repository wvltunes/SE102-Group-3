using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    [SerializeField] private Image[] energyBars;
    [SerializeField] private Color filledColor = Color.green;
    [SerializeField] private Color emptyColor = Color.gray;
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
        if (playerController == null || energyBars == null || energyBars.Length == 0)
        {
            return;
        }
        
        int currentEnergy = playerController.GetCurrentEnergy();
        
        // Update each energy bar rectangle
        for (int i = 0; i < energyBars.Length; i++)
        {
            if (energyBars[i] != null)
            {
                if (i < currentEnergy)
                {
                    // Filled
                    energyBars[i].color = filledColor;
                }
                else
                {
                    // Empty
                    energyBars[i].color = emptyColor;
                }
            }
        }
    }
}
