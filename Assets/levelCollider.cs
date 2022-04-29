using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelCollider : MonoBehaviour
{
    public Rigidbody rb;
    public KatamariMovement k_move;

    // Update is called once per frame
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "green")
        {
            rb.drag += 0.001f;
            rb.angularDrag = Mathf.Clamp(rb.drag,0f,1f);

            rb.angularDrag += .1f;
            rb.angularDrag = Mathf.Clamp(rb.angularDrag,0f,40f);
            


        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "green")
        {
            rb.drag = 0f;
            rb.angularDrag = 0f;
        }
    }
}
