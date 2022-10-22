using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
public class doNotDestroyMusic : MonoBehaviour
{
    public AudioMixer masterSet;
    GameObject[] musicObj;
    int levelNumber;

     public void Awake()
    {
        musicObj = GameObject.FindGameObjectsWithTag("GameMusic");
        levelNumber = SceneManager.GetActiveScene().buildIndex;
        if (musicObj.Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
        
    }

    public void Start()
    {
        masterSet.SetFloat("MasterVolume", -20f);
    }

    private void OnLevelWasLoaded(int level)
    {
        
        print(level);
        if ((level != 2 && level != 1 && level != 0) && level != levelNumber)
        {
            Destroy(this.gameObject);
        }
    }

}
