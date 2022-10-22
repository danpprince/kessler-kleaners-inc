using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class displayVersionNumber : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<TextMeshProUGUI>().text = "Version: " + Application.version;
    }

    
}
