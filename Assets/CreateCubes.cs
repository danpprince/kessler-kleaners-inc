using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCubes : MonoBehaviour
{
    public GameObject cube;
    public float size = 20;
    public int numCubes = 300;

    // Start is called before the first frame update
    void Start()
    {
        CreateCubeLine();
        CreateCubeCluster();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void CreateCubeLine () {
        for (int i = 0; i < 100; i ++)
        {
            Vector3 position = new Vector3(10, 2, 10 + i);
            Instantiate(cube, position, new Quaternion());
        }
    }

    void CreateCubeCluster () {
        float startX = transform.position.x - size;
        float endX = transform.position.x + size;
        float startY = transform.position.y - size; 
        float endY = transform.position.y + size;
        float startZ = transform.position.z - size;
        float endZ = transform.position.z + size;

        for (int i = 0; i < numCubes; i ++)
        {
            Vector3 position = new Vector3(
                Random.Range(startX, endX),
                Random.Range(startY, endY),
                Random.Range(startZ, endZ)
            );
            Instantiate(cube, position, new Quaternion());
        }
    }
}
