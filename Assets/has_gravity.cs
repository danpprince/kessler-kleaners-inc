using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class has_gravity : MonoBehaviour
{
    public bool grav_bool = true;
    public GameObject gravity_well;
    public GameObject gravity;
    public float mass;
    public Rigidbody rb;

    void Start()
    {

        rb = GetComponent<Rigidbody>();
        mass = rb.mass * GetComponent<Transform>().localScale.x;



        gravity = Instantiate(gravity_well, transform.position, transform.rotation);
        gravity.transform.localScale = (new Vector3 (5* mass,5* mass,5 * mass));
        



        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
