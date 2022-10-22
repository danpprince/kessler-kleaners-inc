using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public Button start, settings, quit;
    public float menuJoystickThreshold = 0.99f;
    private float verticalInput, acceptButton;
    private bool usingMouse = true;
    public GameObject levelLoader;
   
    


    // Start is called before the first frame update
    void Start()
    {   

        start.onClick.AddListener(StartButton);
        settings.onClick.AddListener(SettingsButton);
        quit.onClick.AddListener(QuitButton);
        
        

    }

    private void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
        acceptButton = Input.GetAxis("Fire3");
        //this makes it so that if player uses mouse and nothign is selected, reselect start game
        if (EventSystem.current.currentSelectedGameObject == null && (verticalInput >= menuJoystickThreshold || verticalInput <= -menuJoystickThreshold))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            EventSystem.current.SetSelectedGameObject(start.gameObject);
            usingMouse = false;

        }

        if (Input.GetAxis("Mouse X") > 0.01f || Input.GetAxis("Mouse Y") > 0.01f)
        {
            usingMouse = true;
        }
            
        if (usingMouse)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            EventSystem.current.SetSelectedGameObject(null);
        }
       

    }

    void StartButton()
    {
        levelLoader.GetComponent<LevelLoader>().LoadNextLevel();
    }

    void SettingsButton()
    {
        SceneManager.LoadScene("HowToPlay");
    }

    void QuitButton() {
        //Debug.Log("The game is quitting");
        Application.Quit();
    }


}
