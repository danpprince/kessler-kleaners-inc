using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevelOnCollision : MonoBehaviour
{
    [SerializeField]
    string strTag;
    public GameObject katamari;
    private bool shouldRestart = false;
    private string capturedTag = "";
    private KatamariMovement km;
    private Rigidbody rb;
    public GameObject cam;
    

    // Restart Scene on Collision
    private void Update()
    {
        

        if (capturedTag == strTag && shouldRestart==true)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    

   




    private void OnCollisionEnter(Collision collision)
    {
        capturedTag = collision.collider.tag;
        shouldRestart = true;
    }
}
