using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class compute_manager : MonoBehaviour
{
    public float zoom = 1.0f;
    public float scrollX = (float)Screen.width/2.0f;
    public float scrollY = (float)Screen.height/2.0f;
    public int maxIterations = 500;
    public ComputeShader computeShader;
    public RenderTexture renderTexture;
    
    int currenResolutionWidth, currenResolutionHeight;

    
    // Start is called before the first frame update
    void Start()
    {

    }


    
    private void OnRenderImage(RenderTexture src, RenderTexture dest){
        int kernelHandle = computeShader.FindKernel("CSMain");

        computeShader.SetInt("screenWidth", Screen.width);
        computeShader.SetInt("screenHeight", Screen.height);
        computeShader.SetInt("maxIterations", maxIterations);

        computeShader.SetFloat("zoom", zoom);

        computeShader.SetFloat("scrollX", scrollX);
        computeShader.SetFloat("scrollY", scrollY);


        if (renderTexture == null || currenResolutionWidth != Screen.width || currenResolutionHeight != Screen.height){
            currenResolutionWidth = Screen.width;
            currenResolutionHeight = Screen.height;

            renderTexture = new RenderTexture(Screen.width,Screen.height,24);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create(); 
        }


        computeShader.SetTexture(0,"Result", renderTexture);
        computeShader.Dispatch(0, renderTexture.width/8, renderTexture.height/8, 1);
        
        Graphics.Blit(renderTexture, dest);
    }
    // Update is called once per frame
    void Update()
    {
        
    }


}
