using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevelOnCollision : MonoBehaviour
{
    [SerializeField]
    string strTag;

    // Restart Scene on Collision
    

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Collider>().tag == strTag)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
