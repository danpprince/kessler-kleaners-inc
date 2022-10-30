using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public float initialFuel;
    private float fuelRemaining, massCollected, timeElapsedSec, lastHitTimeSec;
    private float timeBetweenHits = 5.0f;
    private float totalMass = 0;
    public KleanerMovement k_move;
    public int strokeCount;
    public Image fuelBar;
    public Image massBar;
    public TextMeshProUGUI fuelText, strokesText, endOfLevelText;
    public GameObject endOfLevelCanvas;

    void Start()
    {
        fuelRemaining = initialFuel;
        // Initialize so a hit can be performed at the start
        lastHitTimeSec = Time.time - timeBetweenHits;
        strokeCount = 0;
        massCollected = 0;
    }

    void Update()
    {
        timeElapsedSec += Time.deltaTime;
        UpdateUI();
    }

    void UpdateUI()
    {
        strokesText.text = "Strokes: " + strokeCount;
        fuelBar.fillAmount = fuelRemaining / initialFuel;

        if (fuelRemaining > 0)
        {
            fuelText.text = "Fuel";
            fuelText.color = Color.white;
        }
        else
        {
            fuelText.text = "No fuel remaining";
            fuelText.color = Color.red;
        }

        massBar.fillAmount = massCollected / totalMass;
    }

    public float UseFuel(float amountRequested) 
    {
        // Returns the actual amount of fuel that has been used from this call

        bool allFuelRequestedIsAvailable = fuelRemaining - amountRequested >= 0;
        if (allFuelRequestedIsAvailable)
        {
            fuelRemaining -= amountRequested * Time.timeScale;
            return amountRequested;

        } else
        {
            float fuelUsed = fuelRemaining;
            fuelRemaining = 0;
            return fuelUsed;
        }
    }

    /// <summary>
    /// Add to the amount of mass stuck to the kleaner
    /// </summary>
    /// <param name="massToAdd"></param>
    public void AddMass(float massToAdd)
    {
        massCollected += massToAdd;
    }
    
    /// <summary>
    /// Add to the total amount of mass in the level
    /// </summary>
    /// <param name="massToAdd"></param>
    public void AddToTotalMass(float massToAdd)
    {
        totalMass += massToAdd;
    }

    public bool tryToHit()
    {
        bool isTimeForNextHit = GetTimeSinceLastHit() >= timeBetweenHits;

        if (isTimeForNextHit)
        {
            lastHitTimeSec = Time.time;
            strokeCount += 1;
            
            return true;
        }

        return false;
    }

    public bool can_hit()
    {
        bool isTimeForNextHit = GetTimeSinceLastHit() >= timeBetweenHits;
        return isTimeForNextHit;
    }

    public float GetTimeSinceLastHit()
    {
        return Time.time - lastHitTimeSec;
    }

    public float GetFuelRemaining()
    {
        return fuelRemaining;
    }

    public void ShowEndOfLevelScreen(bool showContinueText)
    {
        endOfLevelCanvas.SetActive(true);
        endOfLevelText.text = "LEVEL COMPLETE!" + "\n"
            + "Total Mass Collected: " + Mathf.RoundToInt(massCollected) + "\n"
            + "Number of Strokes: " + strokeCount + "\n"
            + "Remaining Fuel Bonus: " + Mathf.RoundToInt(fuelRemaining) + "\n"
            + "Total Score: " + (Mathf.RoundToInt(massCollected / strokeCount) + Mathf.RoundToInt(fuelRemaining));

        if (showContinueText)
        {
            endOfLevelText.text += "\n\nPress A to continue";
        }
    }
}
