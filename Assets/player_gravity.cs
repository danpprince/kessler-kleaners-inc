using System.Collections;
using System.Collections.Generic;
using UnityEngine;


 

public class player_gravity : MonoBehaviour
{


    public Rigidbody rb;
    public bool turn_on_gravity;
    public GameObject sphere;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (turn_on_gravity)
        {
            Vector3 currentPosition = GetComponent<Transform>().position;
            Vector3 gravityPosition = sphere.GetComponent<Transform>().position;
            Vector3 gravityForce = (gravityPosition - currentPosition) * 100;
            rb.AddForce(gravityForce);
        }
    }

    private void OnTriggerEnter(Collider gravity_well)
    {
        turn_on_gravity = true;

    }


    private void OnTriggerExit(Collider gravity_well)
    {
        turn_on_gravity = false;

    }

}
