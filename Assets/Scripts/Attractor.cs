using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour
{
    public Rigidbody rb;
    public Vector3 force;
    public float forceMagnitude;
    public float Big_G;
    private GameObject player;
     void FixedUpdate()
    {

     
        Attractor[] attractors = FindObjectsOfType<Attractor>();
        foreach (Attractor attractor in attractors)
        {

            if (attractor.gameObject.tag == "player")
            {
                Attract(attractor);
            }
        }
    }




    void Attract (Attractor objToAttract)
    {
        Rigidbody rbToAttract = objToAttract.rb;

        Vector3 direction = rb.position - rbToAttract.position;
        float distance = direction.magnitude;

        forceMagnitude  =   Big_G * (rb.mass * rbToAttract.mass) / Mathf.Pow(distance, 2);

        forceMagnitude = Mathf.Min(forceMagnitude,1000f);
        

        force = direction.normalized * forceMagnitude;

        rbToAttract.AddForce(force);
    }
}
