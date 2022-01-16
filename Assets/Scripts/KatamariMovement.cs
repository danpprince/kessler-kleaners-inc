using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KatamariMovement : MonoBehaviour
{
    public float movementSpeed = 1;
    public float rotationSpeed = 1;
    public Quaternion heading;
    public float hitStrength = 10;
    public float hitXAngle = 45;
    public float hitXAngleSpeed = 10;

    public int stuckObjectCountLimit = 200;

    public bool isColorCodingColliders = false;

    public ResourceManager resourceManager;

    private Rigidbody rb;

    private float horizontalInput, verticalInput, hitInput, stopInput;

    private Queue<GameObject> stuckObjects;

    private Color colliderObjectColor = new Color(1.0f, 0.25f, 0.95f, 1.0f);
    private Color nonColliderObjectColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);

    public bool golf_mode = false;
    private Katamari_Input modeInput;
    



    // Start is called before the first frame update
    void Start()
    {
        modeInput = new Katamari_Input();
        modeInput.Mode.Enable();
        modeInput.Mode.Newaction.performed += Newaction_performed; 

        rb = GetComponent<Rigidbody>();

        Vector3 initialRotation = transform.rotation.eulerAngles;
        heading = Quaternion.Euler(0, transform.rotation.y, 0);

        stuckObjects = new Queue<GameObject>();
    }

    private void Newaction_performed(InputAction.CallbackContext obj)
    {
        
        if (golf_mode == false)
        {
            golf_mode = true;
        }

        else
        {
            golf_mode = false;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        hitInput = Input.GetAxis("Jump");
        stopInput = Input.GetAxis("Stop");
        
       
        

        


        
        hitXAngle += hitXAngleSpeed * verticalInput * Time.deltaTime;



        
    }

    private void FixedUpdate() {
        float yRotation = horizontalInput * rotationSpeed;
        Vector3 rotation = new Vector3(0, yRotation, 0);
        transform.Rotate(rotation, Space.World);
        heading *= Quaternion.Euler(0, yRotation, 0);

        float accelerateFuelUsed = resourceManager.UseFuel(hitInput);
        float stopFuelUsed = resourceManager.UseFuel(stopInput);

        rb.AddForce(CalculateHitVector(accelerateFuelUsed));

        // Roll the katamari in the direction it is being hit
        rb.AddTorque(CalculateRollVector(accelerateFuelUsed));

        // Slow down based on input
        if (stopFuelUsed > 0.5)
        {
            rb.velocity = 0.95f * rb.velocity;
            rb.angularVelocity = 0.95f * rb.angularVelocity;
            
            rb.useGravity = false;
        } else
        {
            rb.useGravity = true;
        }
    }

    public Vector3 CalculateHitVector(float accelerateFuelUsed)
    {
        Quaternion hitAngle = heading * Quaternion.Euler(-1 * hitXAngle, 0, 0);
        Vector3 hitVector = accelerateFuelUsed * hitStrength * (hitAngle * Vector3.forward);
        return hitVector;
    }

    public Vector3 CalculateRollVector(float accelerateFuelUsed)
    {
        return 100 * accelerateFuelUsed * (heading * Vector3.right);
    }

    private void OnCollisionEnter(Collision collision) {
        print("Collided with " + collision.collider.name);

        GameObject colliderObject = collision.gameObject;
        if (colliderObject.tag == "Stickable") {
            StickToKatamari(colliderObject);

            AudioSource audioSource = colliderObject.GetComponent<AudioSource>();
            audioSource.pitch = Random.Range(0.7f, 1.3f);
            audioSource.Play();
        }
    }

    void StickToKatamari(GameObject colliderObject) {
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

        Rigidbody rb = colliderObject.GetComponent<Rigidbody>();
        resourceManager.AddMass(rb.mass);
        Destroy(rb);

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

            bool isDeletingColider = Random.value < 0.75;
            if (isDeletingColider)
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

    
   
}


