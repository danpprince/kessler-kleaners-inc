using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UI_controller : MonoBehaviour
{

    public TMP_Text text_mode;
    public bool mode;
    public GameObject get_mode;
    public GameObject player_gravity;
    public float gravity;


    // Start is called before the first frame update
    void Start()
    {
        
        text_mode.text = "Gold Mode : " + mode.ToString() + "\n Gravitational Force " + get_mode.transform.position.ToString();
        mode = get_mode.GetComponent<KatamariMovement>().golf_mode;
        gravity = player_gravity.GetComponent<Attractor>().forceMagnitude;

    }

    // Update is called once per frame
    void Update()
    {

        gravity = player_gravity.GetComponent<Attractor>().forceMagnitude;
        mode = get_mode.GetComponent<KatamariMovement>().golf_mode;
        text_mode.text = "Golf Mode : " + mode.ToString() + "\n Gravitational Force " + gravity.ToString() + get_mode.transform.position.ToString();
    }
}
