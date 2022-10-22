using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject howToPlayUI;
    public GameObject controlsUI;

    // Start is called before the first frame update
    void Awake()
    {
        mainMenuUI = GameObject.Find("Main Canvas");
        howToPlayUI = GameObject.Find("HowToPlayCanvas");
        controlsUI = GameObject.Find("ControlsCanvas");

        mainMenuUI.SetActive(true);
        howToPlayUI.SetActive(false);
        controlsUI.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
