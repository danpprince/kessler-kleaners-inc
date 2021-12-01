using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatamariMovement : MonoBehaviour
{
    public float movementSpeed = 1;
    public float rotationSpeed = 1;
    public Quaternion heading;
    public float hitStrength = 10;
    public float hitXAngle = 45;
    public float hitXAngleSpeed = 10;

    public int stuckObjectCountLimit = 200;

    private Rigidbody rb;

    private float horizontalInput, verticalInput, hitInput;

    private Queue<GameObject> stuckObjects;

    private Color colliderObjectColor = new Color(1.0f, 0.25f, 0.95f, 1.0f);
    private Color nonColliderObjectColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Vector3 initialRotation = transform.rotation.eulerAngles;
        heading = Quaternion.Euler(0, transform.rotation.y, 0);

        stuckObjects = new Queue<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        hitInput = Input.GetAxis("Jump");

        hitXAngle += hitXAngleSpeed * verticalInput * Time.deltaTime;
    }

    private void FixedUpdate() {
        float yRotation = horizontalInput * rotationSpeed;
        Vector3 rotation = new Vector3(0, yRotation, 0);
        transform.Rotate(rotation, Space.World);
        heading *= Quaternion.Euler(0, yRotation, 0);

        Quaternion hitAngle = heading * Quaternion.Euler(-1 * hitXAngle, 0, 0);
        Vector3 hitVector = hitInput * hitStrength * (hitAngle * Vector3.forward);
        rb.AddForce(hitVector);

        // Roll the katamari in the direction it is being hit
        rb.AddTorque(100 * hitInput * (heading * Vector3.right), ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision) {
        print("Collided with " + collision.collider.name);

        GameObject colliderObject = collision.gameObject;
        if (colliderObject.name == "Small Cube(Clone)") {
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

        Destroy(colliderObject.GetComponent<Rigidbody>());
        colliderObject.transform.position = colliderPosition;

        OptimizeNewStuckObjects(colliderObject);
        OptimizeOldestStuckObjects();
    }

    void OptimizeNewStuckObjects(GameObject colliderObject)
    {
        bool isDeletingColider = Random.value < 0.25;
        if (isDeletingColider)
        {
            Destroy(colliderObject.GetComponent<Collider>());            
            colliderObject.GetComponent<Renderer>().material.color = nonColliderObjectColor;
        } 
        else 
        {
            // Mark new objects with colliders with a visible color
            colliderObject.GetComponent<Renderer>().material.color = colliderObjectColor;
        }
        stuckObjects.Enqueue(colliderObject);
    }

    void OptimizeOldestStuckObjects()
    {
       if (stuckObjects.Count > stuckObjectCountLimit)
       {
            bool isDeletingColider = Random.value < 0.75;
            if (isDeletingColider)
            {
                // print("Deleting collider from old object");
                GameObject oldestObject = stuckObjects.Dequeue();
                Destroy(oldestObject.GetComponent<Collider>());

                oldestObject.GetComponent<Renderer>().material.color = nonColliderObjectColor;
            }
        }
    }
}


