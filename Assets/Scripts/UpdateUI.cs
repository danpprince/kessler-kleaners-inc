using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateUI : MonoBehaviour
{
    public Image fuelBar;
    public Image massBar;
    public TextMeshProUGUI fuelText, strokesText, endOfLevelText;
    public GameObject endOfLevelCanvas;

    void Update()
    {
        strokesText.text = "Strokes: " + ResourceManager.GetStrokeCount();
        fuelBar.fillAmount = ResourceManager.GetFuelRemainingRatio();

        if (ResourceManager.GetFuelRemaining() > 0)
        {
            fuelText.text = "Fuel";
            fuelText.color = Color.white;
        }
        else
        {
            fuelText.text = "No fuel remaining";
            fuelText.color = Color.red;
        }

        massBar.fillAmount = ResourceManager.GetMassCollectedRatio();
    }
}
