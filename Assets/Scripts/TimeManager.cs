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
    // second round of implementation\\

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
        player.GetComponent<KatamariMovement>().flyStrength = current_velocity;





        ///NORMAL SPEED\\\
        if (myStateMachine == stateMachine.normalSpeed)
        {

            // Transition to Slow Down\\
            if ( isFlyMovementDeadTime && myHitInput > 0.5)
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
            current_velocity = 0; //replace with flyStrength





            //Transition to slowMotion\\
            if (Time.timeScale == slowdownFactor)
            {
                myStateMachine = stateMachine.slowMotion;
                current_velocity = standard_velocity * 10f;
            }

            else if (myHitInput < 0.5)
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
                

            }

        }

    }

        
    
}