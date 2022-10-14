using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class backToMenu : MonoBehaviour
{
    [SerializeField] private Button button;           
    void Start()
    {
        button.onClick.AddListener(LoadMenu);
    }

    void LoadMenu()
    {
        SceneManager.LoadScene("Menu Screen");
    }
}
