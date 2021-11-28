using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCubes : MonoBehaviour
{
    public GameObject cube;

    // Start is called before the first frame update
    void Start()
    {
        CreateCubeLine();
        CreateCubePile();
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

    void CreateCubePile () {
        int numCubes = 300;
        float startX = 10, endX = 15;
        float startY = 10, endY = 20;
        float startZ = 10, endZ = 15;
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
