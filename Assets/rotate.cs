using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{

    void Update()
    {
        this.transform.Rotate(-transform.forward*.5f);
    }
}
