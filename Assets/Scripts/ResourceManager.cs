using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public static float initialFuel = 75f;
    private static float fuelRemaining, massCollected, timeElapsedSec, lastHitTimeSec;
    private static float timeBetweenHits = 5.0f;
    private static float totalMass = 0;
    private static int strokeCount;
    private static bool goalHasBeenReached;
    private float timeElapsedAfterGoalReached;
    private float endOfLevelTime = 5;
    private static bool isNextLevelRequested;
    private static bool isLoadingNextLevelAvailable;

    void Start()
    {
        fuelRemaining = initialFuel;
        // Initialize so a hit can be performed at the start
        lastHitTimeSec = Time.time - timeBetweenHits;
        strokeCount = 0;
        massCollected = 0;

        isNextLevelRequested = false;
        isLoadingNextLevelAvailable = false;
        timeElapsedAfterGoalReached = 0;
        goalHasBeenReached = false;
    }

    void Update()
    {
        timeElapsedSec += Time.deltaTime;

        if (goalHasBeenReached)
        {
            timeElapsedAfterGoalReached += Time.unscaledDeltaTime;
            if (timeElapsedAfterGoalReached <= endOfLevelTime)
            {
                Time.timeScale -= 0.3f * Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Clamp(Time.timeScale, 0.1f, 1f);
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                isLoadingNextLevelAvailable = false;
            }
            else
            {
                if (Input.GetButtonDown("Jump"))
                {
                    isNextLevelRequested = true;
                }
                isLoadingNextLevelAvailable = true;
            }
        }
    }

    public static int GetStrokeCount()
    {
        return strokeCount;
    }

    public static float GetFuelRemaining()
    {
        return fuelRemaining;
    }

    public static float GetFuelRemainingRatio()
    {
        return fuelRemaining / initialFuel;
    }

    public static float GetMassCollected()
    {
        return massCollected;
    }

    public static float GetMassCollectedRatio()
    {
        return massCollected / totalMass;
    }

    public static float UseFuel(float amountRequested) 
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
    public static void AddMass(float massToAdd)
    {
        massCollected += massToAdd;
    }
    
    /// <summary>
    /// Add to the total amount of mass in the level
    /// </summary>
    /// <param name="massToAdd"></param>
    public static void AddToTotalMass(float massToAdd)
    {
        totalMass += massToAdd;
    }

    public static bool TryToHitKleaner()
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

    public static bool CatHitKleaner()
    {
        bool isTimeForNextHit = GetTimeSinceLastHit() >= timeBetweenHits;
        return isTimeForNextHit;
    }

    public static float GetTimeSinceLastHit()
    {
        return Time.time - lastHitTimeSec;
    }

    public static void SetGoalHasBeenReached()
    {
        goalHasBeenReached = true;
    }

    public static bool GetGoalHasBeenReached()
    {
        return goalHasBeenReached;
    }

    /// <summary>
    /// Returns true if player has requested that the next level be loaded
    /// </summary>
    /// <returns></returns>
    public static bool GetIsNextLevelRequested()
    {
        return isNextLevelRequested;
    }

    /// <summary>
    /// Returns true if enough time has elapsed after goal has been reached 
    /// for the next level loading to be available
    /// </summary>
    /// <returns></returns>
    public static bool GetIsLoadingNextLevelAvailable()
    {
        return isLoadingNextLevelAvailable;
    }
}
