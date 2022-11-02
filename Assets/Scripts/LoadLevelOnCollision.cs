using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevelOnCollision : MonoBehaviour
{
    
    private string strTag = "Player";
    private string strTagStickable = "Stickable";

    [SerializeField]
    string strSceneName;

    public GameObject cam;

    public float endOfLevelTime;

    // Restart Scene on Collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == strTag || collision.collider.tag == strTagStickable)
        {
            cam.GetComponent<PostProcess>().enabled = false;
            ResourceManager.SetGoalHasBeenReached();
            print("Collision Detected for End of Level");
        }
    }
}
