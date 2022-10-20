using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class doNotDestroyMusic : MonoBehaviour
{

    GameObject[] musicObj;
    int levelNumber;
 

    private void Awake()
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

    private void OnLevelWasLoaded(int level)
    {

        print(level);
        if ((level != 2 && level != 1 && level != 0) && level != levelNumber)
        {
            Destroy(this.gameObject);
        }
    }

}
