using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTargetsInsideCube : MonoBehaviour
{
    public List<GameObject> gameObjectsToInstance;
    public int numObjects;

    void Start()
    {
        for (int i = 0; i < numObjects; i++)
        {
            Vector3 point = GetRandomPointInsideCube();
            Quaternion rotation = Quaternion.Euler(
                Random.Range(-180, 180),
                Random.Range(-180, 180),
                Random.Range(-180, 180)
            );

            GameObject objToInstance = gameObjectsToInstance[
                Random.Range(0, gameObjectsToInstance.Count)
            ];
            Instantiate(objToInstance, point, rotation);
        }
        Destroy(gameObject);
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
