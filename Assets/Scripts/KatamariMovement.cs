using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class KatamariMovement : MonoBehaviour
{

    public float movementSpeed = 1;
    public float rotationSpeed = 1;
    public Quaternion heading;
    public float strikeStrength = 100;
    public float flyStrength = 10;
    [System.NonSerialized]
    public float hitXAngle = 45;
    [System.NonSerialized]
    public float hitXAngleSpeed = 10;

    public int stuckObjectCountLimit = 200;

    public bool isColorCodingColliders = false;

    //For Sound Control\\
    public AudioMixer slowMixer;
    private AudioSource collisionAudioSource;

    public ResourceManager resourceManager;

    private Rigidbody rb;
    [System.NonSerialized]
    public float horizontalInput, verticalInput, hitInput, stopInput;

    private Queue<GameObject> stuckObjects;

    private Color colliderObjectColor = new Color(1.0f, 0.25f, 0.95f, 1.0f);
    private Color nonColliderObjectColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
    [System.NonSerialized]
    public bool isGolfHitMode = true;

    public GameObject powerBar;

    private bool go_up;
    [System.NonSerialized]
    public float power;
    public float time_modifier;
    private float angle_timer = 0;

    // state machine stuff\\


    public enum StateMachine { normalSpeed, slowDown, slowMotion, speedUp, golfMode, toGolfMode };
    [System.NonSerialized]
    public StateMachine myStateMachine;
    [System.NonSerialized]
    float standardStrength = 0;
    [System.NonSerialized]
    public bool strokeDone = false;

    // for determing how much and attack/release time of time scaling\\
    [System.NonSerialized]
    public float slowdownFactor = 0.1f;
    [System.NonSerialized]
    public float slowdownLength = 1f;
    ForceMode forceMode;

    //for Collision Stuff\\
    public PhysicMaterial highFriction;
    private float timeOnGround = 0;
    public float timeToStop = 5;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // for the powerbar
        power = 0;
        go_up = true;

        //for switching velocity strength in slow-mo\\
        standardStrength = flyStrength;

        Vector3 initialRotation = transform.rotation.eulerAngles;
        heading = Quaternion.Euler(0, transform.rotation.y, 0);

        stuckObjects = new Queue<GameObject>();

        collisionAudioSource = GetComponent<AudioSource>();
        
        
        myStateMachine = StateMachine.toGolfMode;
        forceMode = ForceMode.Impulse;
        
    }



    // Update is called once per frame
    void Update()
    {

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        hitInput = Input.GetAxis("Jump");
        stopInput = Input.GetAxis("Stop");




        // increment the vertical angle in chunks
        angle_timer += Time.unscaledDeltaTime;
        if (verticalInput > 0.5 && angle_timer >= 0.25) {
            hitXAngle += 20;
            angle_timer = 0;
        }

        if (verticalInput < -0.5 && angle_timer >= 0.25) {
            hitXAngle -= 20;
            angle_timer = 0;
        }

        slowMixer.SetFloat("Pitch", Time.timeScale);
        
    }

    private void FixedUpdate()
    {
        float yRotation = horizontalInput * rotationSpeed;
        Vector3 rotation = new Vector3(0, yRotation, 0);
        transform.Rotate(rotation, Space.World);
        heading *= Quaternion.Euler(0, yRotation, 0);

        float accelerateFuelUsed = resourceManager.UseFuel(hitInput);
        float stopFuelUsed = resourceManager.UseFuel(stopInput);

        rb.AddForce(CalculateHitVector(accelerateFuelUsed), forceMode);

        // Roll the katamari in the direction it is being hit
        rb.AddTorque(CalculateRollVector(accelerateFuelUsed), forceMode);


        // Slow down based on input
        if (stopFuelUsed > 0.5)
        {
            rb.velocity *= 0.95f;
            rb.angularVelocity = 0.95f * rb.angularVelocity;
            rb.useGravity = false;
        }
        else {
            rb.useGravity = true;
        }

        time_state_machine();
    }

    public Vector3 CalculateHitVector(float accelerateFuelUsed)
    {
        float strength;

        if (IsGolfHitMode())
        {
            strength = strikeStrength;
        }
        else
        {
            strength = flyStrength;
        }

        Quaternion hitAngle = heading * Quaternion.Euler(-1 * hitXAngle, 0, 0);
        Vector3 hitVector = accelerateFuelUsed * strength * (hitAngle * Vector3.forward);
        return hitVector;
    }

    public Vector3 CalculateRollVector(float accelerateFuelUsed)
    {
        float strength;

        if (IsGolfHitMode())
        {
            strength = 100000;
        }
        else
        {
            strength = 100;
        }
        return strength * accelerateFuelUsed * (heading * Vector3.right);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Audio source may be null for objects stuck to the katamari
        if (!(collisionAudioSource is null))
        {
            float volume = 0.1f + collision.impulse.magnitude / 500;

            collisionAudioSource.pitch = Random.Range(0.9f, 1.1f);
            collisionAudioSource.volume = volume;
            collisionAudioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        print("Collided with " + collider.name);

        GameObject colliderObject = collider.gameObject;
        if (colliderObject.tag == "Stickable")
        {
            StickToKatamari(colliderObject);

            AudioSource audioSource = colliderObject.GetComponent<AudioSource>();
            if (!(audioSource is null))
            {
                audioSource.pitch = Random.Range(0.7f, 1.3f);
                audioSource.Play();
            }
        }
    }

    void StickToKatamari(GameObject colliderObject)
    {
        float towardsKatamariAmount = 0.0f;
        float jitterAmount = 0.0f;

        Vector3 colliderPosition = colliderObject.transform.position;

        // print("Sticking to katamari");
        colliderObject.transform.SetParent(transform, worldPositionStays: true);
        colliderObject.GetComponent<Collider>().material = highFriction;

        Vector3 towardsKatamari = colliderPosition - transform.position;
        colliderPosition -= towardsKatamari * towardsKatamariAmount;

        // Too many objects stuck on the same plane creates a flat surface
        // that makes it hard to roll. Add some random jitter to the stuck
        // object position to avoid this.
        Vector3 jitter = new Vector3(
            Random.Range(-jitterAmount, jitterAmount),
            Random.Range(-jitterAmount, jitterAmount),
            Random.Range(-jitterAmount, jitterAmount)
        );
        colliderPosition += jitter;

        // Make object "solid" by disabling the trigger
        colliderObject.GetComponent<Collider>().isTrigger = false;

        RandomRotation rr = colliderObject.GetComponent<RandomRotation>();
        if (rr != null)
        {
            rr.StopRotating();
        }

        // TODO: Should objects have other mass?
        resourceManager.AddMass(1);

        colliderObject.transform.position = colliderPosition;

        OptimizeNewStuckObjects(colliderObject);
        OptimizeOldestStuckObjects();
    }

    void OptimizeNewStuckObjects(GameObject colliderObject)
    {
        Color colliderMarkerColor;

        bool isDeletingColider = Random.value < 0.5;
        if (isDeletingColider)
        {
            Destroy(colliderObject.GetComponent<Collider>());
            colliderMarkerColor = nonColliderObjectColor;
        }
        else
        {
            colliderMarkerColor = colliderObjectColor;
        }

        if (isColorCodingColliders)
        {
            // Mark new objects with colliders with a visible color
            colliderObject.GetComponent<Renderer>().material.color = colliderMarkerColor;
        }

        stuckObjects.Enqueue(colliderObject);
    }

    void OptimizeOldestStuckObjects()
    {
        if (stuckObjects.Count > stuckObjectCountLimit)
        {
            GameObject oldestObject = stuckObjects.Dequeue();

            bool isDeletingCollider = Random.value < 0.75;
            if (isDeletingCollider)
            {
                // print("Deleting collider from old object");
                Destroy(oldestObject.GetComponent<Collider>());

                if (isColorCodingColliders)
                {
                    oldestObject.GetComponent<Renderer>().material.color = nonColliderObjectColor;
                }
            }
        }
    }


    void _PowerBar()
    {

        time_modifier = Time.fixedDeltaTime;
        if (power <= 1 && go_up)
        {
            power += 0.01f * (time_modifier / 0.02f);
        }

        if (power >= 1)
        {
            go_up = false;
        }

        if (power >= 0 && go_up == false)
        {
            power -= 0.01f * (time_modifier / 0.02f);
        }

        if (power <= 0)
        {
            go_up = true;
        }


        powerBar.GetComponent<Image>().fillAmount = power;


    }

    // Returns True if in golf hit mode
    public bool IsGolfHitMode()
    {

        if (myStateMachine == StateMachine.golfMode)
        {
            isGolfHitMode = true;
        } else
        {
            isGolfHitMode = false;
        }
        return isGolfHitMode;
    }

    public void time_state_machine() {
        // THINGS TO CHANGE: Replace canhit() with end of stoke condition
       

        bool isFlyMovementDeadTime = resourceManager.GetTimeSinceLastHit() >= 0.75f;
        
        
        //GOLF MODE\\
        if (myStateMachine == StateMachine.golfMode)
        {
            
            _PowerBar();
            strikeStrength = power * 2500; // make this editable

            //Transition to Normal Speed\\
            if (hitInput >0.5){
                rb.constraints = RigidbodyConstraints.None;
                myStateMachine = StateMachine.normalSpeed;
                powerBar.SetActive(false);
                this.GetComponent<LineRenderer>().enabled = false;
                resourceManager.tryToHit();
                forceMode = ForceMode.Force;
                
            }
        }

        if (myStateMachine == StateMachine.toGolfMode) {
            Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, slowdownFactor, 1f);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;

            if (Time.timeScale == 1)
            {
                power = 0;
                powerBar.SetActive(true);
                this.GetComponent<LineRenderer>().enabled = true;
                go_up = true;
                forceMode = ForceMode.Impulse;
                rb.constraints = RigidbodyConstraints.FreezePosition; //currently this make the line renderer not work as there is no possible velocity when this is true!
                rb.freezeRotation = true;
                rb.freezeRotation = false;
                strokeDone = false;
                myStateMachine = StateMachine.golfMode;

            }

        }

        //NORMAL SPEED\\
        if (myStateMachine == StateMachine.normalSpeed)
        {
            // Transition to Slow Down\\
            if (isFlyMovementDeadTime && hitInput > 0.5) {
                myStateMachine = StateMachine.slowDown;
            }
            //Or toGolfMOde\\
            if (strokeDone) {
                myStateMachine = StateMachine.toGolfMode;
            }

            
        } else if (myStateMachine == StateMachine.slowDown) {
            Time.timeScale -= (1f / slowdownLength) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, slowdownFactor, 1f);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            flyStrength =0;

            //Transition to slowMotion\\
            if (Time.timeScale == slowdownFactor) {
                myStateMachine = StateMachine.slowMotion;
                flyStrength = standardStrength * (1 / slowdownFactor);
            }
            //Transition to Speed up if Played lets go of slow motion movement mid slowdown\\
            if (hitInput <= 0.5){
                myStateMachine = StateMachine.speedUp;
            } //Or to toGolfMode\\
            if (strokeDone){
                myStateMachine = StateMachine.toGolfMode;
            }

        } else if (myStateMachine == StateMachine.slowMotion) {
            if (hitInput < 0.5) {
                ///Transition to Speed Up\\\
                myStateMachine = StateMachine.speedUp;
            }
            if (strokeDone){
                myStateMachine = StateMachine.toGolfMode;
            }
            //SPEED UP\\
        } else if (myStateMachine == StateMachine.speedUp) {
            Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, slowdownFactor, 1f);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            flyStrength = 0;

            rb.velocity *= .98f; // maybe a better more dynamic way to do this but this works for now


            if (hitInput >= 0.5) {
                myStateMachine = StateMachine.slowDown;
            }
            if (strokeDone) {
                myStateMachine = StateMachine.toGolfMode;
            }

            if (Time.timeScale == 1) {
                //Transition Back to Normal Speed\\
                myStateMachine = StateMachine.normalSpeed;
                flyStrength = 0;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {

        if (myStateMachine != StateMachine.golfMode && collision.gameObject.tag =="green")
        {
            timeOnGround += Time.deltaTime;

            rb.drag += 0.01f;
            rb.angularDrag += .2f;
            
        } else if (myStateMachine == StateMachine.golfMode)
        {
            rb.drag = 0f;
            rb.angularDrag = 0f;
            
        } 
        
            if (timeOnGround >= timeToStop && rb.velocity.magnitude <= 0.5f)
            {
                strokeDone = true;
            }
        
    }

    private void OnCollisionExit(Collision collision)
    {
        rb.drag = 0f;
        rb.angularDrag = 0f;
        timeOnGround = 0;
    }


}


