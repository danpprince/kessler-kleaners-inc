using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtLevel : MonoBehaviour
{
    public GameObject level;


    // Update is called once per frame
    void Update()
    {
        transform.LookAt(level.transform.position);
        transform.Translate(Vector3.right * Time.deltaTime);
    }
}
