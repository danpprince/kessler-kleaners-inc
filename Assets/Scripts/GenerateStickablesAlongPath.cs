using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class GenerateStickablesAlongPath : MonoBehaviour
{
    public List<GameObject> gameObjectsToInstance;
    public int numObjects;
    public PathCreator pathCreator;
    public float radius;
    public ResourceManager resourceManager;

    void Start()
    {
        // Traverse the path and place stickables along it within a set radius
        for (int i = 0; i < numObjects; i++)
        {
            float fractionAlongLine = i / ((float) numObjects);

            Vector3 pointOnPath = pathCreator.path.GetPointAtTime(fractionAlongLine);
            Vector3 pathNormal  = pathCreator.path.GetNormalAtDistance(fractionAlongLine);

            float distanceFromPath = Random.Range(0, radius);
            float normalRotationAngle = Random.Range(0, 360);

            Vector3 stickablePosition = 
                pointOnPath + distanceFromPath 
                * (Quaternion.Euler(0, 0, normalRotationAngle) * pathNormal);

            Quaternion stickableRotation = Quaternion.Euler(
                Random.Range(-180, 180),
                Random.Range(-180, 180),
                Random.Range(-180, 180)
            );

            GameObject objToInstance = gameObjectsToInstance[
                Random.Range(0, gameObjectsToInstance.Count)
            ];
            Instantiate(objToInstance, stickablePosition, stickableRotation);
            resourceManager.AddToTotalMass(1);
        }

        Destroy(gameObject);
    }
}
