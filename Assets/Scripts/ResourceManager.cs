using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ResourceManager : MonoBehaviour
{
    public float initialFuel;
    private float fuelRemaining, massCollected, timeElapsedSec, lastHitTimeSec;
    private float timeBetweenHits = 5.0f;
    public KatamariMovement k_move;

    public Text resourceText;

    void Start()
    {
        fuelRemaining = initialFuel;
        // Initialize so a hit can be performed at the start
        lastHitTimeSec = Time.time - timeBetweenHits;
        
    }

    void Update()
    {
        timeElapsedSec += Time.deltaTime;
        UpdateUI();
        if (Input.GetKeyDown("r"))
        {

            SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        }
        
    }

    void UpdateUI()
    {
        resourceText.text =
            "Fuel remaining: " + fuelRemaining + "\n"
            + "Mass collected: " + massCollected + "\n"
            + "Time elapsed: " + timeElapsedSec.ToString("0.0") + "\n"
            + "Time since last hit: " + GetTimeSinceLastHit().ToString("0.0") +"\n"
            + "Current State :" + k_move.myStateMachine; 
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

    public void AddMass(float massToAdd)
    {
        massCollected += massToAdd;
    }

    public bool tryToHit()
    {
        bool isTimeForNextHit = GetTimeSinceLastHit() >= timeBetweenHits;

        if (isTimeForNextHit)
        {
            lastHitTimeSec = Time.time;
            
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
}
