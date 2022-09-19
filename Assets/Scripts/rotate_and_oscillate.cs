using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate_and_oscillate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        oscillate();
        spin();
    }

    void oscillate()
    {

        float scalar = Mathf.Clamp(Mathf.Sin(Time.time), 0.5f, 1f);
        scalar = Mathf.Abs(Mathf.Sin(Time.time));
        this.transform.localScale = new Vector3(scalar, scalar, scalar);
    }

    void spin()
    {
        this.transform.Rotate(Vector3.forward * Time.deltaTime * 100f);

    }




}
