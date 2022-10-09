using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizeAngle : MonoBehaviour
{
    public KleanerMovement km;
    public float positionOffset = 1;
    

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = km.gameObject.transform.position;

        Quaternion hitAngle = km.heading * Quaternion.Euler(-1 * km.hitXAngle, 0, 0);

        transform.position += hitAngle * (positionOffset * Vector3.forward);
        transform.rotation = hitAngle * Quaternion.Euler(90, 0, 90);
    }
}
