using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UI_controller : MonoBehaviour
{

    public TMP_Text text_mode;
    public bool mode;
    public GameObject get_mode;


    // Start is called before the first frame update
    void Start()
    {
        
        text_mode.text = "Gold Mode : " + mode.ToString();
        mode = get_mode.GetComponent<KatamariMovement>().golf_mode;

    }

    // Update is called once per frame
    void Update()
    {
        mode = get_mode.GetComponent<KatamariMovement>().golf_mode;
        text_mode.text = "Gof Mode : " + mode.ToString();
    }
}
