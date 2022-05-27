// Prefer Microsoft C# coding style conventions in this file:
// https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class KatamariMovement : MonoBehaviour
{
    public float movementSpeed = 1;
    public float rotationSpeed = 1;
    private Quaternion heading;
    public float strikeStrength = 100;
    public float flyStrength = 10;
    private float hitXAngle = 45;
    private float hitXAngleSpeed = 10;

    public int stuckObjectCountLimit = 200;

    public bool isColorCodingColliders = false;

    //For Sound Control\\
    public AudioMixer slowMixer;
    private AudioSource collisionAudioSource;

    public ResourceManager resourceManager;

    private Rigidbody rb;
    private float horizontalInput, verticalInput, hitInput, stopInput;
    private bool isHitInputActive;

    private Queue<GameObject> stuckObjects;

    private Color colliderObjectColor = new Color(1.0f, 0.25f, 0.95f, 1.0f);
    private Color nonColliderObjectColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);

    public GameObject powerBar;

    private bool go_up;
    private float power;
    public float time_modifier;
    private float angle_timer = 0;

    // state machine stuff\\
    private enum StateMachine { normalSpeed, slowDown, slowMotion, speedUp, golfMode, toGolfMode };
    private StateMachine movementState;
    private bool strokeDone = false;

    // time scaling
    public float slowdownFactor = 0.1f;
    public float slowdownLength = 1f;

    // for determing how much and attack/release time of time scaling\\
    ForceMode forceMode;

    //for Collision Stuff\\
    private float timeOnGround = 0;
    public float timeToStop = 5;

    //for determing where to point the "heading"
    public GameObject _camera;
    public GameObject arrow;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // for the powerbar
        power = 0;
        go_up = true;

        Vector3 initialRotation = transform.rotation.eulerAngles;
        heading = Quaternion.Euler(0, transform.rotation.y, 0);

        stuckObjects = new Queue<GameObject>();

        collisionAudioSource = GetComponent<AudioSource>();

        movementState = StateMachine.toGolfMode;
        forceMode = ForceMode.Impulse;

        // If the katamari rigidbody falls asleep, it will disable the OnCollisionStay
        // callback. Setting the sleep threshold to zero makes sure this callback
        // continues to be called when the katamari is stationary on a collider.
        rb.sleepThreshold = 0;
    }

    // Update is called once per frame
    void Update()
    {

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        hitInput = Input.GetAxis("Jump");
        isHitInputActive = hitInput > 0.5;
        stopInput = Input.GetAxis("Stop");
        float forceToGolfModeInput = Input.GetAxis("Fire3");

        if (forceToGolfModeInput > 0.5)
        {
            movementState = StateMachine.golfMode;
        }

        // increment the vertical angle in chunks
        angle_timer += Time.unscaledDeltaTime;
        if (verticalInput > 0.5 && angle_timer >= 0.25)
        {
            hitXAngle += 20;
            angle_timer = 0;
        }

        if (verticalInput < -0.5 && angle_timer >= 0.25)
        {
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
        if (movementState == StateMachine.golfMode || movementState == StateMachine.slowMotion || movementState == StateMachine.slowDown)
        {
            heading *= Quaternion.Euler(0, yRotation, 0);
        }

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
        else
        {
            rb.useGravity = true;
        }

        UpdateTimeStateMachine();
    }

    public Vector3 CalculateHitVector(float accelerateFuelUsed)
    {
        float strength;

        if (IsGolfHitMode())
        {
            strength = strikeStrength * power;
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

    void UpdatePowerBar()
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
        return movementState == StateMachine.golfMode;
    }

    public void UpdateTimeStateMachine()
    {
        bool isFlyMovementDeadTime = resourceManager.GetTimeSinceLastHit() >= 0.75f;

        switch (movementState) {
            case StateMachine.golfMode:
                UpdatePowerBar();

                //Transition to Normal Speed\\
                if (isHitInputActive)
                {
                    rb.constraints = RigidbodyConstraints.None;
                    movementState = StateMachine.normalSpeed;
                    powerBar.SetActive(false);
                    arrow.SetActive(false);
                    resourceManager.tryToHit();
                    forceMode = ForceMode.Force;

                }
                break;

            case StateMachine.toGolfMode:

                Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Clamp(Time.timeScale, slowdownFactor, 1f);
                Time.fixedDeltaTime = Time.timeScale * 0.02f;

                //transition to golf mode
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
                    _pointArrowAwayFromKatamari();
                    hitXAngle = 45;
                    arrow.SetActive(true);
                    movementState = StateMachine.golfMode;
                }
                break;

            case StateMachine.normalSpeed:
                // Transition to Slow Down\\
                if (isFlyMovementDeadTime && isHitInputActive)
                {
                    movementState = StateMachine.slowDown;
                }
                //Or toGolfMOde\\
                if (strokeDone)
                {
                    movementState = StateMachine.toGolfMode;
                    arrow.SetActive(false);
                }

            case StateMachine.slowDown:
                DecreaseTimeScale();

                //Transition to slowMotion\\
                if (Time.timeScale == slowdownFactor)
                {
                    movementState = StateMachine.slowMotion;

                    _pointArrowAwayFromKatamari();
                    arrow.SetActive(true);

                }
                //Transition to Speed up if player lets go of slow motion movement mid slowdown\\
                if (!isHitInputActive)
                {
                    movementState = StateMachine.speedUp;
                    arrow.SetActive(false); //maybe
                } //Or to toGolfMode\\
                if (strokeDone)
                {
                    movementState = StateMachine.toGolfMode;
                    arrow.SetActive(false); //maybe
                }
                break;

            case StateMachine.slowMotion:

                if (!isHitInputActive)
                {
                    ///Transition to Speed Up\\\
                    movementState = StateMachine.speedUp;
                    arrow.SetActive(false); // maybe
                }
                if (strokeDone)
                {
                    movementState = StateMachine.toGolfMode;
                }
                break;

            case StateMachine.speedUp:
                IncreaseTimeScale();

                // scale back the velocity from slow motion to prevent unexpected momentum
                rb.velocity *= .98f; // maybe a better more dynamic way to do this but this works for now

                //transition to slow down
                if (isHitInputActive)
                {

                    //arrow.SetActive(true);
                    movementState = StateMachine.slowDown;

                }
                if (strokeDone)
                {
                    movementState = StateMachine.toGolfMode;
                }

                if (Time.timeScale == 1)
                {
                    //Transition Back to Normal Speed\\
                    movementState = StateMachine.normalSpeed;
                }
                break;

            case default:
                print("Unrecognized state reached: " + movementState);
        }
    }

    public virtual void OnCollisionStay(Collision collision)
    {
        if (movementState != StateMachine.golfMode && movementState != StateMachine.slowMotion && collision.gameObject.tag == "green")
        {
            timeOnGround += Time.deltaTime;
            rb.drag += 0.005f;
            rb.angularDrag += .2f;
        }
        else if (movementState == StateMachine.golfMode || movementState == StateMachine.slowMotion)
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
        if (collision.gameObject.tag == "green")
        {
            rb.drag = 0f;
            rb.angularDrag = 0f;
            timeOnGround = 0;
        }
    }

    private void PointArrowAwayFromKatamari()
    {
        heading = Quaternion.LookRotation(this.transform.position - _camera.transform.position, Vector3.up);
        hitXAngle = heading.x;
    }

    private void ModifyTimeScale(bool isSpeedingUp)
    {
        if (isSpeedingUp)
        {
            Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
        }
        else
        {
            Time.timeScale -= (1f / slowdownLength) * Time.unscaledDeltaTime;
        }
        Time.timeScale = Mathf.Clamp(Time.timeScale, slowdownFactor, 1f);
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    private void IncreaseTimeScale() { ModifyTimeScale(true); }
    private void DecreaseTimeScale() { ModifyTimeScale(false); }
}
