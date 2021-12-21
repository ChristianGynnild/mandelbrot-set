using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class compute_manager : MonoBehaviour
{
    public double zoom = 1.0;
    public double centerReal = 0.0;
    public double centerImaginary = 0.0;
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

        computeShader.SetFloat("zoom", (float)zoom);

        computeShader.SetFloat("centerReal", (float)centerReal);
        computeShader.SetFloat("centerImaginary", (float)centerImaginary);


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

    public double scrollSpeed = 0.1;
    public double zoomSpeed = 0.1;

    void Update()
    {
        if(Input.GetKey(KeyCode.A)){
            centerReal -= scrollSpeed/zoom;
        }
        if(Input.GetKey(KeyCode.D)){
            centerReal += scrollSpeed/zoom;
        }
        if(Input.GetKey(KeyCode.W)){
            centerImaginary += scrollSpeed/zoom;
        }
        if(Input.GetKey(KeyCode.S)){
            centerImaginary -= scrollSpeed/zoom;
        }


        if(Input.GetKey(KeyCode.C)){
            zoom += (1.0 + (double)Mathf.Abs((float)zoom))*zoomSpeed;
        }
        if(Input.GetKey(KeyCode.X)){
            zoom -= (1.0 + (double)Mathf.Abs((float)zoom))*zoomSpeed;
        }
        
    }


}
