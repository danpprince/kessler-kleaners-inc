using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orbit : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject planet;
    public float speed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        OrbitAround();
    }

    void OrbitAround()
    {
        transform.RotateAround(planet.transform.position, Vector3.forward, speed * Time.deltaTime);
    }


}


