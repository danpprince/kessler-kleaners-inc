using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class on_enable : MonoBehaviour
{

    public GameObject player;
    // Start is called before the first frame update
    private void OnEnable()
    {
        player.GetComponent<KatamariMovement>().power = 0;
    }
}
