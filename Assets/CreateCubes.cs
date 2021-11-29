using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCubes : MonoBehaviour
{
    public GameObject cube;
    public float sizeX = 20, sizeY = 20, sizeZ = 20;
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
        float startX = transform.position.x - sizeX;
        float endX = transform.position.x + sizeX;
        float startY = transform.position.y - sizeY; 
        float endY = transform.position.y + sizeY;
        float startZ = transform.position.z - sizeZ;
        float endZ = transform.position.z + sizeZ;

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
