using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    public float speedX = 0.5f, speedY = 0.5f;
    private float positionX = 0, positionY = 0;
    private Material material;

    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        positionX += Time.deltaTime * speedX;
        positionY += Time.deltaTime * speedY;

        material.mainTextureOffset = new Vector2(positionX, positionY);
    }
}
