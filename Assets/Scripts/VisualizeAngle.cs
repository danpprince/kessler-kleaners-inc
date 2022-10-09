using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizeAngle : MonoBehaviour
{
    public KatamariMovement km;
    public float positionOffset = 1;
    
    void Update()
    {
        transform.position = km.gameObject.transform.position;

        Quaternion hitAngle = km.heading * Quaternion.Euler(-1 * km.hitXAngle, 0, 0);

        transform.position += hitAngle * (positionOffset * Vector3.forward);
        transform.rotation = hitAngle * Quaternion.Euler(90, 0, 90);
    }
}
