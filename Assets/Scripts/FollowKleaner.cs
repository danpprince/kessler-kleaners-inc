using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowKleaner : MonoBehaviour
{
    public GameObject kleaner;

    public float followDistance;
    public float cameraHeight;
    [System.NonSerialized]
    public Vector3 originalPosition;
    public Quaternion originalRotation;

    private KleanerMovement movement;

    // Start is called before the first frame update
    void Start()
    {
        movement = kleaner.GetComponent<KleanerMovement>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion kleanerHeading = movement.heading;

        float verticalInput = Input.GetAxis("Vertical");
        cameraHeight -= verticalInput * Time.deltaTime;

        transform.position =
            kleaner.transform.position
            - kleanerHeading * new Vector3(0, 0, followDistance)
            + new Vector3(0, cameraHeight, 0);

        transform.LookAt(kleaner.transform);
    }
}
