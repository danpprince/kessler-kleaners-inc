using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevelOnCollision : MonoBehaviour
{
    [SerializeField]
    string strTag;

    [SerializeField]
    string strSceneName;

    public GameObject cam;
    public Image scoreBox;
    private float t = 0;

    private Vector2 originalPosition;
    public  float endOfLevelTime;
    private float timeThusFar = 0;
    private bool isAtEndOfLevel;

    private void Start()
    {
        originalPosition = scoreBox.rectTransform.anchoredPosition;
    }


    // Restart Scene on Collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == strTag)
        {
            cam.GetComponent<PostProcess>().enabled = false;
            isAtEndOfLevel = true;
        }
    }

   void Update()
    {
        EndLevel();
        
    }


    private void EndLevel() {
        if (isAtEndOfLevel){
            DisplayScore();
            timeThusFar += Time.unscaledDeltaTime * 0.001f;
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

    private void DisplayScore()
    {
        t += Time.unscaledDeltaTime*2; 

        t = Mathf.Clamp(t, 0, 1);
        float newX = Mathf.SmoothStep(originalPosition.x,0, t);
        float newY = Mathf.SmoothStep(originalPosition.y,0, t);
        float newScale = Mathf.SmoothStep(1, 2, t);
        //scoreBox.rectTransform.position.Set(newX, newY,0);
        scoreBox.rectTransform.anchoredPosition = new Vector2(newX, newY);
        scoreBox.rectTransform.localScale = new Vector3(newScale, newScale, 1);
        

    }

}
