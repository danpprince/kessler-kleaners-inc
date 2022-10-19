using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class nextSceneHowToPlay : MonoBehaviour
{

    public Button next;


     private void Start()
    {
        next.onClick.AddListener(loadControlsScene);
    }

    private void loadControlsScene()
    {
        SceneManager.LoadScene("HowToPlayControls");
    }


}



