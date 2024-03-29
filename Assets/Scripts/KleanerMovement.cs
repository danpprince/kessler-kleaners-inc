// Prefer Microsoft C# coding style conventions in this file:
// https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public static class FlyingConstants
{
    public const float SlowDownDebounceSec = 0.75f;
    public const float StopVelocityMagnitude = 0.5f;
}

public class KleanerMovement : MonoBehaviour
{
    public float movementSpeed = 1;
    private float horizontalRotationSpeed = 3;
    private float verticalRotationSpeed = 3;
    [System.NonSerialized]
    public Quaternion heading;

    private float strikeStrength = 5000;
    private float flyStrength = 2000;

    [System.NonSerialized]
    public float hitXAngle = 45;

    public int stuckObjectCountLimit = 200;

    public bool isColorCodingColliders = false;

    //For Sound Control\\
    private AudioSource collisionAudioSource;

    public ResourceManager resourceManager;

    private Rigidbody rb;
    private float horizontalInput, verticalInput, hitInput, stopInput;
    private bool isHitInputActive;

    private Queue<GameObject> stuckObjects;

    private Color colliderObjectColor = new Color(1.0f, 0.25f, 0.95f, 1.0f);
    private Color nonColliderObjectColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);

    public GameObject powerBar;

    private bool isPowerBarIncreasing = true;
    private float powerBarValue = 0;
    public float time_modifier;
    private float angle_timer = 0;

    public enum StateMachine {
        normalSpeed, slowDown, slowMotion, speedUp, golfMode, toGolfMode,
        resetPositionStart, resetPositionWait
    };
    [System.NonSerialized]
    public StateMachine movementState;

    // time scaling
    public float slowdownFactor = 0.1f;
    public float slowdownLength = 1f;

    //for Collision Stuff\\
    [System.NonSerialized]
    public float timeOnGround = 0;
    public float timeToStop = 5;
    public Vector3 originalPosition;
    public Quaternion originalRotation;

    //for determing where to point the "heading"
    public GameObject _camera;
    public GameObject arrow;

    private bool isFlyMovementDeadTime = false;

    public GameObject hitParticlesObject;
    private ParticleSystem hitParticles;
    public GameObject stopParticlesObject;
    private ParticleSystem stopParticles;
    public GameObject flyParticlesObject;
    private ParticleSystem flyParticles;

    private Vector3 lastGolfPosition;
    private float resetPositionStartTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Vector3 initialRotation = transform.rotation.eulerAngles;
        heading = Quaternion.Euler(0, transform.rotation.y, 0);

        originalPosition = transform.position;
        originalRotation = transform.rotation.normalized;

        stuckObjects = new Queue<GameObject>();

        collisionAudioSource = GetComponent<AudioSource>();

        movementState = StateMachine.toGolfMode;

        // If the kleaner rigidbody falls asleep, it will disable the OnCollisionStay
        // callback. Setting the sleep threshold to zero makes sure this callback
        // continues to be called when the kleaner is stationary on a collider.
        rb.sleepThreshold = 0;

        hitParticles = hitParticlesObject.GetComponent<ParticleSystem>();
        stopParticles = stopParticlesObject.GetComponent<ParticleSystem>();
        flyParticles = flyParticlesObject.GetComponent<ParticleSystem>();

        lastGolfPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.isPaused || LoadLevelOnCollision.isAtEndOfLevel)
        {
            return;
        }

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        hitInput = Input.GetAxis("Jump");
        isHitInputActive = hitInput > 0.5;
        stopInput = Input.GetAxis("Stop");

        // increment the vertical angle in chunks
        angle_timer += Time.unscaledDeltaTime;
        if (movementState == StateMachine.golfMode)
        {
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
        }
        else if (movementState == StateMachine.slowMotion)
        {
            hitXAngle += verticalInput * 0.33f;
        }
    }

    private void FixedUpdate()
    {
        if (PauseMenu.isPaused || LoadLevelOnCollision.isAtEndOfLevel)
        {
            return;
        }

        float yRotation;
        if (movementState == StateMachine.slowMotion)
        {
            yRotation = horizontalInput * horizontalRotationSpeed * 0.33f;
        } else 
        {
             yRotation = horizontalInput * horizontalRotationSpeed * Time.timeScale;
        }
        Vector3 rotation = new Vector3(0, yRotation, 0);
        transform.Rotate(rotation, Space.World);

        if (movementState == StateMachine.golfMode || movementState == StateMachine.slowMotion || movementState == StateMachine.slowDown)
        {
            heading *= Quaternion.Euler(0, yRotation, 0);
        }

        if (
            (
                movementState == StateMachine.normalSpeed ||
                movementState == StateMachine.speedUp ||
                movementState == StateMachine.slowMotion ||
                movementState == StateMachine.speedUp
            ) && !isFlyMovementDeadTime
        )
        {
            float accelerateFuelUsed = resourceManager.UseFuel(hitInput);
            float stopFuelUsed = resourceManager.UseFuel(stopInput * 0.5f);

            ForceMode forceMode = ForceMode.Force;
            rb.AddForce(CalculateHitVector(accelerateFuelUsed), forceMode);
            // Roll the kleaner in the direction it is being hit
            rb.AddTorque(CalculateRollVector(accelerateFuelUsed), forceMode);

            // Slow down based on input
            if (stopFuelUsed > 0)
            {
                rb.velocity *= 0.95f;
                rb.angularVelocity = 0.95f * rb.angularVelocity;
                rb.useGravity = false;

                stopParticles.Play();
            }
            else
            {
                rb.useGravity = true;
                stopParticles.Stop();
            }
        }

        UpdateTimeStateMachine();
    }

    public Vector3 CalculateHitVector(float accelerateFuelUsed)
    {
        float strength;
        

        if (IsGolfHitMode())
        {
            strength = strikeStrength * powerBarValue;
        }
        else
        {
            strength = flyStrength/Time.timeScale;
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
        // Audio source may be null during trajectory simulation
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
        GameObject colliderObject = collider.gameObject;
        if (colliderObject.tag == "Stickable")
        {
            StickToKleaner(colliderObject);

            AudioSource audioSource = colliderObject.GetComponent<AudioSource>();
            if (!(audioSource is null))
            {
                audioSource.pitch = Random.Range(0.7f, 1.3f);
                audioSource.Play();
            }
        }
    }

    void StickToKleaner(GameObject colliderObject)
    {
        float towardsKleanerAmount = 0.1f;
        float jitterAmount = 0.0f;

        Vector3 colliderPosition = colliderObject.transform.position;

        colliderObject.transform.SetParent(transform, worldPositionStays: true);

        Vector3 towardsKleaner = colliderPosition - transform.position;
        colliderPosition -= towardsKleaner * towardsKleanerAmount;

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
        if (powerBarValue <= 1 && isPowerBarIncreasing)
        {
            powerBarValue += 0.01f * (time_modifier / 0.02f);
        }

        if (powerBarValue >= 1)
        {
            isPowerBarIncreasing = false;
        }

        if (powerBarValue >= 0 && isPowerBarIncreasing == false)
        {
            powerBarValue -= 0.01f * (time_modifier / 0.02f);
        }

        if (powerBarValue <= 0)
        {
            isPowerBarIncreasing = true;
        }

        powerBar.GetComponent<RawImage>().material.SetFloat("_Power", powerBarValue);
    }

    // Returns True if in golf hit mode
    public bool IsGolfHitMode()
    {
        return movementState == StateMachine.golfMode;
    }

    public void UpdateTimeStateMachine()
    {
        switch (movementState) {
            case StateMachine.resetPositionStart:
                resetPositionStartTime = Time.time;
                movementState = StateMachine.resetPositionWait;

                // Place the kleaner above where it was to prevent it from clipping through the floor
                float verticalOffset = 5f;
                rb.position = lastGolfPosition + new Vector3(0f, verticalOffset, 0f);
                rb.velocity = new Vector3(0f, 0f, 0f);
                rb.angularVelocity = new Vector3(0f, 0f, 0f);

                Time.timeScale = 1.0f;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;

                arrow.SetActive(false);

                flyParticles.Stop();

                break;

            case StateMachine.resetPositionWait:
                // Allow some time for the camera to move behind the kleaner
                float waitTimeSec = 2.0f;

                rb.constraints =
                    RigidbodyConstraints.FreezePositionX
                    | RigidbodyConstraints.FreezePositionZ
                    | RigidbodyConstraints.FreezeRotation;

                if (Time.time - resetPositionStartTime >= waitTimeSec)
                {
                    movementState = StateMachine.toGolfMode;
                }

                break;

            case StateMachine.toGolfMode:
                movementState = StateMachine.golfMode;

                // Set up UI
                powerBarValue = 0;
                powerBar.SetActive(true);
                isPowerBarIncreasing = true;
                this.GetComponent<LineRenderer>().enabled = true;
                arrow.SetActive(true);
                PointArrowAwayFromKleaner();
                hitXAngle = 45;

                // Set up physics
                rb.constraints = RigidbodyConstraints.FreezePosition;
                rb.freezeRotation = false;
                Time.timeScale = 1.0f;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;

                break;

            case StateMachine.golfMode:
                UpdatePowerBar();

                if (isHitInputActive)
                {
                    bool isHitSuccessful = resourceManager.tryToHit();
                    if (isHitSuccessful)
                    {
                        lastGolfPosition = transform.position;

                        rb.constraints = RigidbodyConstraints.None;

                        // Make sure force is calculated in golf state
                        ForceMode forceMode = ForceMode.Impulse;
                        rb.AddForce(CalculateHitVector(1), forceMode);
                        rb.AddTorque(CalculateRollVector(1), forceMode);

                        movementState = StateMachine.normalSpeed;

                        powerBar.SetActive(false);
                        arrow.SetActive(false);
                        this.GetComponent<LineRenderer>().enabled = false;

                        hitParticles.Play();
                    }
                }
                break;

            case StateMachine.normalSpeed:
                isFlyMovementDeadTime =
                    resourceManager.GetTimeSinceLastHit() <= FlyingConstants.SlowDownDebounceSec;
                if (!isFlyMovementDeadTime && isHitInputActive && resourceManager.GetFuelRemaining() > 0)
                {
                    movementState = StateMachine.slowDown;
                }

                if (timeOnGround >= timeToStop && rb.velocity.magnitude <= FlyingConstants.StopVelocityMagnitude)
                {
                    movementState = StateMachine.toGolfMode;
                    arrow.SetActive(false);
                }
                break;

            case StateMachine.slowDown:
                DecreaseTimeScale();
                
                if (Time.timeScale == slowdownFactor)
                {
                    movementState = StateMachine.slowMotion;

                    PointArrowAwayFromKleaner();
                    arrow.SetActive(true);
                }

                if (!isHitInputActive || resourceManager.GetFuelRemaining() == 0f)
                {
                    movementState = StateMachine.speedUp;
                    arrow.SetActive(false); //maybe
                    flyParticles.Stop();
                }
                else if (!flyParticles.isPlaying)
                {
                    flyParticles.Play();
                }

                break;

            case StateMachine.slowMotion:
                if (!isHitInputActive || resourceManager.GetFuelRemaining() == 0f)
                {
                    movementState = StateMachine.speedUp;
                    arrow.SetActive(false); // maybe
                    flyParticles.Stop();
                } 
                else if (!flyParticles.isPlaying)
                {
                    flyParticles.Play();
                }

                break;

            case StateMachine.speedUp:
                IncreaseTimeScale();

                // Scale back the velocity from slow motion to prevent unexpected momentum
                rb.velocity *= .98f;

                if (isHitInputActive && resourceManager.GetFuelRemaining() > 0f)
                {
                    movementState = StateMachine.slowDown;
                }

                if (Time.timeScale == 1)
                {
                    movementState = StateMachine.normalSpeed;
                }
                break;

            default:
                print("Unrecognized state reached: " + movementState);
                break;
        }
    }

    public virtual void OnCollisionStay(Collision collision)
    {
        // DrawTrajectory simulation may run this before Start() is called, so the RigidBody
        // component reference may not have happened yet
        if (rb is null) { rb = GetComponent<Rigidbody>(); }

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
    }

    private void OnCollisionExit(Collision collision)
    {
        if (rb is null) { rb = GetComponent<Rigidbody>(); }

        if (collision.gameObject.tag == "green")
        {
            rb.drag = 0f;
            rb.angularDrag = 0f;
            timeOnGround = 0;
        }
    }

    private void PointArrowAwayFromKleaner()
    {
        Quaternion awayHeading = Quaternion.LookRotation(this.transform.position - _camera.transform.position, Vector3.up);

        // Store the Euler X rotation component only in hitXAngle and not in heading
        heading.eulerAngles = new Vector3(0, awayHeading.eulerAngles.y, 0);
        hitXAngle = -1 * awayHeading.eulerAngles.x;
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

    public RigidbodyConstraints GetRigidbodyConstraints()
    {
        return rb.constraints;
    }

    public void MoveToLastGolfPosition()
    {
        movementState = StateMachine.resetPositionStart;
    }
}
