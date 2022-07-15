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
            StartCoroutine(restartLevel());
        }
    }

    

    private IEnumerator restartLevel()
    {
        km = katamari.GetComponent<KatamariMovement>();
        rb = katamari.GetComponent<Rigidbody>();
        cam.GetComponent<Cinemachine.CinemachineFreeLook>().ForceCameraPosition
            (cam.GetComponent<OriginalPositionRotation>().originalPosition, cam.GetComponent<OriginalPositionRotation>().originalRotation);
        
        yield return StartCoroutine(resetTransforms());
        km.movementState = KatamariMovement.StateMachine.toGolfMode;
        shouldRestart = false;

    }


    private IEnumerator resetTransforms()
    {
        katamari.transform.position = km.originalPosition;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        katamari.transform.rotation = km.originalRotation;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        yield return new WaitForFixedUpdate();


    }




    private void OnCollisionEnter(Collision collision)
    {
        capturedTag = collision.collider.tag;
        shouldRestart = true;
    }
}
