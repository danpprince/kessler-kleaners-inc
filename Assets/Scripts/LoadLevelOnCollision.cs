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
    public ResourceManager resourceManager;
    private float t = 0;

    private Vector2 originalPosition;
    public  float endOfLevelTime;
    private float timeThusFar;
    public static bool isAtEndOfLevel;

    //for loading level
    public GameObject levelLoader;
    private void Start()
    {
        timeThusFar = 0;
        isAtEndOfLevel = false;
        Time.timeScale = 1;
    }


    // Restart Scene on Collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == strTag || collision.collider.tag == strTagStickable)
        {
            cam.GetComponent<PostProcess>().enabled = false;
            isAtEndOfLevel = true;
            print("Collision Detected for End of Level");
        }
    }

    void Update()
    {
        EndLevel();
    }

    private void EndLevel() {
        if (isAtEndOfLevel){
            timeThusFar += Time.unscaledDeltaTime;
            bool showContinueText;
            if (timeThusFar <= endOfLevelTime)
            {
                Time.timeScale -= 0.3f * Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Clamp(Time.timeScale, 0.1f, 1f);
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                showContinueText = false;
            }
            else
            {
                if (Input.GetButtonDown("Jump"))
                {
                    levelLoader.GetComponent<LevelLoader>().LoadNextLevel();
                }
                showContinueText = true;
            }

            resourceManager.ShowEndOfLevelScreen(showContinueText);
        }
    }
}
