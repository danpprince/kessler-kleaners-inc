using UnityEngine;

public class TimeManager : MonoBehaviour
{

    public float slowdownFactor = 0.75f;
    public float slowdownLength = 1f;
    public GameObject player;
    private float current_velocity;
    private float standard_velocity;
    


    public void Start()
    {
        standard_velocity = player.GetComponent<KatamariMovement>().flyStrength;
        current_velocity = standard_velocity;
        

    }



    public void SlowMotion()
    {
        Time.timeScale -= (1f / slowdownLength) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, slowdownFactor, 1f);
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        if (Time.timeScale == slowdownFactor)
        {
            current_velocity = standard_velocity * 10f; 
        }
        else
        {
            current_velocity = standard_velocity;
        }

        player.GetComponent<KatamariMovement>().flyStrength = current_velocity;


    }


    /// <summary>
    /// NOTES Where I am:
    /// 
    /// Currently eases into velocty of slow mo aka makes it faster, everything i think now scales properly including fuel consumption
    /// Objective: Make the velocity magnitude go back down slightly after slow mo!
    /// 
    /// 
    /// </summary>

    public void NormalTime()
    {
        Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, slowdownFactor, 1f);
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        if(Time.timeScale == 1)
        {
            current_velocity = standard_velocity;

        }

        player.GetComponent<KatamariMovement>().flyStrength = current_velocity;
    }
    


}