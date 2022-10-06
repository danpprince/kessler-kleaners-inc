using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public Button start, settings, quit;


    // Start is called before the first frame update
    void Start()
    {
        start.onClick.AddListener(StartButton);
        start.onClick.AddListener(SettingsButton);
        start.onClick.AddListener(QuitButton);

    }

   void StartButton()
    {

    }

    void SettingsButton()
    {

    }

    void QuitButton() {Application.Quit();}


}
