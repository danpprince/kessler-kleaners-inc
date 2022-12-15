using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateEndOfLevelCanvas : MonoBehaviour
{
    public GameObject endOfLevelPanel;
    public TextMeshProUGUI endOfLevelText;

    private void Start()
    {
        endOfLevelPanel.SetActive(false);
    }

    void Update()
    {
        if (ResourceManager.GetGoalHasBeenReached())
        {
            endOfLevelPanel.SetActive(true);

            float massCollected = ResourceManager.GetMassCollected();
            int strokeCount = ResourceManager.GetStrokeCount();
            float fuelRemaining = ResourceManager.GetFuelRemaining();

            endOfLevelText.text = "LEVEL COMPLETE!" + "\n"
                + "Total Mass Collected: " + Mathf.RoundToInt(massCollected) + "\n"
                + "Number of Strokes: " + strokeCount + "\n"
                + "Remaining Fuel Bonus: " + Mathf.RoundToInt(fuelRemaining) + "\n"
                + "Total Score: " + (
                    Mathf.RoundToInt(massCollected / strokeCount) + Mathf.RoundToInt(fuelRemaining)
                );

            if (ResourceManager.GetIsLoadingNextLevelAvailable())
            {
                endOfLevelText.text += "\n\nPress A to continue";
            }
        }

    }
}
