using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    
    [SerializeField]
    KeyCode keyRestart;

    [SerializeField]
    KeyCode keyLoadNextScene;


    private void Awake()
    {
        //list of original buildIndexPair order

        //list of queued buildIndexPair for scene transition utility?
    }

    // Update is called once per frame
    void Update()
    {
        // load next scene in buildIndex
        if (Input.GetKey(keyLoadNextScene))
        {
            LoadNextLevel();
        }

        // restart active scene on key press
        if (Input.GetKey(keyRestart))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }
    /*
    private class SceneNameIndexPairs
    {
        public List<SceneNameIndexPair> buildIndexList;
    }

    [System.Serializable]
    private class SceneNameIndexPair
    {
        public string sceneName;
        public int sceneIndex;
    }

    private void AddSceneNameIndexPair(string name, int index)
    {
        SceneNameIndexPair newPair = new SceneNameIndexPair { sceneName = name, sceneIndex = index };

    }
    */

    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // if next scene index would exceed last scene in build settings
        if (nextSceneIndex > SceneManager.sceneCountInBuildSettings - 1)
        {
            // load first scene instead
            nextSceneIndex = 0;   
        }
        
        StartCoroutine(LoadLevel(nextSceneIndex));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        //Play animation
        transition.SetTrigger("Start");

        //Wait
       
        yield return new WaitForSecondsRealtime(transitionTime);

        //Load scene
        SceneManager.LoadScene(levelIndex);
    }
}
