using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ResourceManager : MonoBehaviour
{
    public float initialFuel;
    private float fuelRemaining, massCollected;

    public Text resourceText;

    void Start()
    {
        fuelRemaining = initialFuel;
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        resourceText.text =
            "Fuel remaining: " + fuelRemaining + "\n"
            + "Mass collected: " + massCollected;
    }

    public float UseFuel(float amountRequested) 
    {
        // Returns the actual amount of fuel that has been used from this call

        bool allFuelRequestedIsAvailable = fuelRemaining - amountRequested >= 0;
        if (allFuelRequestedIsAvailable)
        {
            fuelRemaining -= amountRequested;
            return amountRequested;

        } else
        {
            float fuelUsed = fuelRemaining;
            fuelRemaining = 0;
            return fuelUsed;
        }
    }

    public void AddMass(float massToAdd)
    {
        massCollected += massToAdd;
    }
}
