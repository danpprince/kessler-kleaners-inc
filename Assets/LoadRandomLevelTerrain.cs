using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadRandomLevelTerrain : MonoBehaviour
{
    private string LevelPrefabs = "TerrainForMenu";
    private Object[] levels;

    // Start is called before the first frame update
    void Start()
    {
        levels = Resources.LoadAll(LevelPrefabs) as Object[];
        int randomLevel = Random.Range(0, levels.Length);
        GameObject.Instantiate(levels[randomLevel]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
