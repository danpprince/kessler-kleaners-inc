using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevelOnCollision : MonoBehaviour
{
    [SerializeField]
    string strTag;

    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == strTag)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
