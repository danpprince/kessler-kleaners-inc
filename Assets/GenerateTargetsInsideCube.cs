using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTargretsInsideCube : MonoBehaviour
{
    public GameObject gameObjectToInstance;
    public int numObjects;

    void Start()
    {
        for (int i = 0; i < numObjects; i++)
        {
            Vector3 point = GetRandomPointInsideCube();
            print("Placing object at " + point);
            GameObject newObject = Instantiate(gameObjectToInstance, point, new Quaternion());
            newObject.name = "Yoo " + i;
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
