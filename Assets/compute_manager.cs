using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class compute_manager : MonoBehaviour
{
    public float zoom = 1.0f;
    public float centerReal = 0.0f;
    public float centerImaginary = 0.0f;
    public float angle = 0.0f;
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
        computeShader.SetFloat("angle", angle);
        computeShader.SetFloat("centerReal", centerReal);
        computeShader.SetFloat("centerImaginary", centerImaginary);


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

    public float scrollSpeed = 0.1f;
    public float zoomSpeed = 0.1f;
    public float rotationSpeed = 0.1f;

    void Update()
    {
        Vector2 rotation = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

 

        if(Input.GetKey(KeyCode.A)){
            centerReal -= scrollSpeed/zoom*rotation.x;
            centerImaginary -= scrollSpeed/zoom*rotation.y;
        }
        if(Input.GetKey(KeyCode.D)){
            centerReal += scrollSpeed/zoom*rotation.x;
            centerImaginary += scrollSpeed/zoom*rotation.y;
        }
        //Rotatates 'rotation vector' with 90 degrees before applying linear transformation
        //[x,y] * (90 degree rotation) = [-y,x]
        if(Input.GetKey(KeyCode.W)){
            centerReal += -scrollSpeed/zoom*rotation.y;
            centerImaginary += scrollSpeed/zoom*rotation.x;
        }
        if(Input.GetKey(KeyCode.S)){
            centerReal -= -scrollSpeed/zoom*rotation.y;
            centerImaginary -= scrollSpeed/zoom*rotation.x;
        }


        if(Input.GetKey(KeyCode.C)){
            zoom += (1.0f + Mathf.Abs(zoom))*zoomSpeed;
        }
        if(Input.GetKey(KeyCode.X)){
            zoom -= (1.0f + Mathf.Abs(zoom))*zoomSpeed;
        }
        if(Input.GetKey(KeyCode.Q)){
            angle += rotationSpeed;
        }
        if(Input.GetKey(KeyCode.E)){
            angle -= rotationSpeed;
        }
        
    }


}
