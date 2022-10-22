using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class LoadRandomLevelTerrain : MonoBehaviour
{
    private string LevelPrefabs = "TerrainForMenu";
    private Object[] levels;
    public Camera cam;
    public GameObject place;
    public AudioMixer thignsToMute;
    

    // Start is called before the first frame update
    void Start()
    {
        levels = Resources.LoadAll(LevelPrefabs) as Object[];
        int randomLevel = Random.Range(0, levels.Length);
        GameObject curLevel = (GameObject)GameObject.Instantiate(levels[randomLevel],place.transform);
        thignsToMute.SetFloat("Volume", -40f);
        cam.GetComponent<LookAtLevel>().level = curLevel;
        
    }

    private void OnDisable()
    {
        thignsToMute.SetFloat("Volume", -0f);
    }


}
