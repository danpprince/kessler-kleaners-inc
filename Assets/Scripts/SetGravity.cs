using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGravity : MonoBehaviour
{
    public Vector3 gravity;

    void Start()
    {
        Physics.gravity = gravity;
    }
}
