using static System.Math;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    public float maxRotationSpeed;
    private bool isRotating;
    private Vector3 rotationVector;
        
    void Start()
    {
        isRotating = true;
        rotationVector = new Vector3(
            Random.Range(0.0f, maxRotationSpeed),
            Random.Range(0.0f, maxRotationSpeed),
            Random.Range(0.0f, maxRotationSpeed)
        );
    }

    void FixedUpdate()
    {
        if (isRotating)
        {
<<<<<<< HEAD
            transform.Rotate(rotationVector*Time.fixedDeltaTime);
=======
            transform.Rotate(rotationVector * Time.fixedDeltaTime);
>>>>>>> origin/master
        }
    }

    public void StopRotating()
    {
        isRotating = false;
    }
}
