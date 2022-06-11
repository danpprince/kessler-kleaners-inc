using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelOnCollision : MonoBehaviour
{
    [SerializeField]
    string strTag;

    [SerializeField]
    string strSceneName;

    public GameObject cam;
    public  float endOfLevelTime;
    private float timeThusFar = 0;
    private bool isAtEndOfLevel;

    // Restart Scene on Collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == strTag)
        {
            cam.GetComponent<PostProcess>().enabled = false;
            isAtEndOfLevel = true;
        }
    }

   void FixedUpdate()
    {
        EndLevel();
    }


    private void EndLevel() {
        if (isAtEndOfLevel){
            timeThusFar += Time.unscaledDeltaTime;
            //Move Score to center
            if (timeThusFar <= endOfLevelTime)
            {
                Time.timeScale -= 0.3f * Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Clamp(Time.timeScale, 0.1f, 1f);
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
            } else
            {
                Time.timeScale = 1;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                SceneManager.LoadScene(strSceneName);
                isAtEndOfLevel = false;
            }
        }
    }
}
