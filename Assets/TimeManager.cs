using UnityEngine;

public class TimeManager : MonoBehaviour
{

    public float slowdownFactor = 0.1f;
    public float slowdownLength = 1f;


    public GameObject player;
    private float current_velocity;
    private float standard_velocity;

    //For scaling down velocity\\
    private float slowmo_start_time;
    private float current_time;
    private float time_elapsed;
    private float velocity_magnitude;

    private enum stateMachine { normalSpeed, slowDown, slowMotion, speedUp };
    stateMachine myStateMachine;
    public GameObject powerBar;
    private bool powerBarActive;
    public KatamariMovement myKatamariMovement;
    public ResourceManager myResourceManager;


    public void Start()
    {
        standard_velocity = myKatamariMovement.flyStrength;
        current_velocity = standard_velocity;



        myStateMachine = stateMachine.normalSpeed;

    }


    public void time_state_machine()
    {

        float flyMovementDeadTime = .75f;
        bool isFlyMovementDeadTime = myResourceManager.GetTimeSinceLastHit() >= flyMovementDeadTime;
        float myHitInput = myKatamariMovement.hitInput;
        powerBarActive = myKatamariMovement.power_bar_active;
        player.GetComponent<KatamariMovement>().flyStrength = current_velocity;





        ///NORMAL SPEED\\\
        if (myStateMachine == stateMachine.normalSpeed)
        {

            // Transition to Slow Down\\
            if (powerBarActive == false && isFlyMovementDeadTime && myHitInput > 0.5)
            {
                //Next State\\

                myStateMachine = stateMachine.slowDown;
                

            }
        }
        ///SLOW DOWN\\\
        else if (myStateMachine == stateMachine.slowDown)
        {
            Time.timeScale -= (1f / slowdownLength) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, slowdownFactor, 1f);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            current_velocity = 0;
            
            

            

            //Transition to slowMotion\\
            if (Time.timeScale == slowdownFactor)
            {
                myStateMachine = stateMachine.slowMotion;
                current_velocity = standard_velocity * 10f;
            }   

            else if(myHitInput < 0.5)
            {
                myStateMachine = stateMachine.speedUp;
                
            }
        }
        //SLOW MOTION\\\
        else if (myStateMachine == stateMachine.slowMotion)
        {


            

            if (myHitInput < 0.5)
            {
                ///Transition to Speed Up\\\
                myStateMachine = stateMachine.speedUp;
                
            }
        }

        //Speed Up\\
        else if (myStateMachine == stateMachine.speedUp)
        {

            Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, slowdownFactor, 1f);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            current_velocity = 0;



            if (Time.timeScale == 1)
            {
                

                //Transition Back to Normal Speed\\
                myStateMachine = stateMachine.normalSpeed;
                //player.GetComponent<Rigidbody>().velocity.magnitude 

            }

        }

    }






    //public void SlowMotion()
    //{
    //    Time.timeScale -= (1f / slowdownLength) * Time.unscaledDeltaTime;
    //    Time.timeScale = Mathf.Clamp(Time.timeScale, slowdownFactor, 1f);
    //    Time.fixedDeltaTime = Time.timeScale * 0.02f;
    //    if (Time.timeScale == slowdownFactor)
    //    {
    //        current_velocity = standard_velocity * 10f;

    //    }
    //    else
    //    {
    //        current_velocity = standard_velocity;
    //    }

    //    player.GetComponent<KatamariMovement>().flyStrength = current_velocity;


    //}


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
        //Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
        //Time.timeScale = Mathf.Clamp(Time.timeScale, slowdownFactor, 1f);
        //Time.fixedDeltaTime = Time.timeScale * 0.02f;

        //if (Time.timeScale != 1)
        //{
        //    velocity_magnitude = player.GetComponent<Rigidbody>().velocity.magnitude;
        //}

        //if (Time.timeScale == 1)
        //{
        //    current_velocity = standard_velocity;


        //}

        player.GetComponent<KatamariMovement>().flyStrength = current_velocity;
    }



}