using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcess : MonoBehaviour
{
    private Material _material;
    public Shader _shader;
    
    // Start is called before the first frame update
    void Start()
    {
        _material = new Material(_shader);
        //Shader.SetGlobalFloat("Effect amount", (1 - Time.timeScale));
        //_material.SetFloat("_EffectAmount", (1 - Time.timeScale));
    }

    private void Update()
    {
        var effectAmount = Mathf.Clamp((1 - Time.timeScale),0,.2f);
        _material.SetFloat("_EffectAmount", effectAmount);
        //Shader.SetGlobalFloat("_EffectAmount", (1 - Time.timeScale));
        

        

        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        Graphics.Blit(source, destination, _material);

    }


}
