using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Vector3.back * Time.deltaTime * 100f);
        this.transform.Rotate(Vector3.left * Time.deltaTime * 50f);
    }
}
