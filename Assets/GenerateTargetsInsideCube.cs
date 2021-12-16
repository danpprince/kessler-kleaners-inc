using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTargetsInsideCube : MonoBehaviour
{
    public GameObject gameObjectToInstance;
    public int numObjects;
    public List<GameObject> cubs;
    public Vector3 cubby_position;
    public Vector3 direction_of_travel;
    public bool gravity;
    public float Gravity_Strength;

    void Start()
    {
       cubs =  new List<GameObject>();

        for (int i = 0; i < numObjects; i++)
        {
            Vector3 point = GetRandomPointInsideCube();
            GameObject newObject = Instantiate(gameObjectToInstance, point, new Quaternion());
            cubs.Add(newObject);
        }

        //Destroy(gameObject);
    }

    private void FixedUpdate()
    {

        if (gravity == true)
        {

            for (int i = 0; i < cubs.Count; i++)
            {
                GameObject cubby = cubs[i];
                cubby_position = cubby.transform.position;
                direction_of_travel = gameObject.transform.position - cubby_position;
                cubby.GetComponent<Rigidbody>().AddForce(direction_of_travel * Gravity_Strength);
            }

        }
    }

    public Vector3 GetRandomPointInsideCube()
    {
        // Extents are all 1 because this object is assumed to be a scaled cube
        Vector3 extents = new Vector3(1, 1, 1) / 2f;
        Vector3 point = new Vector3(
            Random.Range(-extents.x, extents.x),
            Random.Range(-extents.y, extents.y),
            Random.Range(-extents.z, extents.z)
        );

        return transform.TransformPoint(point);
    }
}
