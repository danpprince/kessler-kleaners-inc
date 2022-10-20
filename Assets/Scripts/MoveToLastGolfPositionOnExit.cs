using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToLastGolfPositionOnExit : MonoBehaviour
{
    [SerializeField]
    string strTag;

    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == strTag)
        {
            KleanerMovement km = collider.GetComponent<KleanerMovement>();

            if (km != null)
            {
                km.MoveToLastGolfPosition();
            }
        }
    }
}
