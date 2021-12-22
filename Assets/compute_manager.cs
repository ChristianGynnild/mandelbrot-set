using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class compute_manager : MonoBehaviour
{
    public int maxIterations = 500;
    
    public Vector2 morph = new Vector2(0,0);
    public Vector4 hue = new Vector4(0.17f, 1.784f, 0.335f, 1.0f);

    public Vector2 complexCenter = new Vector2(0.0f, 0.0f);
    public float angle = 0.0f;
    public float zoom = 1.0f;

    Vector2 smoothComplexCenter = new Vector2(0.0f, 0.0f);
    float smoothAngle = 0.0f;
    float smoothZoom = 1.0f;

    

    public ComputeShader computeShader;
    public RenderTexture renderTexture;
    
    int currenResolutionWidth, currenResolutionHeight;



    
    // Start is called before the first frame update
    void Start()
    {

    }


    
    private void OnRenderImage(RenderTexture src, RenderTexture dest){
        int kernelHandle = computeShader.FindKernel("CSMain");

        smoothAngle = Mathf.Lerp(smoothAngle, angle, 0.03f);
        smoothComplexCenter = new Vector2(Mathf.Lerp(smoothComplexCenter.x, complexCenter.x, 0.03f), Mathf.Lerp(smoothComplexCenter.y, complexCenter.y, 0.03f));
        smoothZoom = Mathf.Lerp(smoothZoom, zoom, 0.03f);

        computeShader.SetInt("screenWidth", Screen.width);
        computeShader.SetInt("screenHeight", Screen.height);
        computeShader.SetInt("maxIterations", maxIterations);

        computeShader.SetVector("hue", hue);
        computeShader.SetFloat("zoom", smoothZoom);
        computeShader.SetFloat("angle", smoothAngle);
        computeShader.SetFloat("centerReal", smoothComplexCenter.x);
        computeShader.SetFloat("centerImaginary", smoothComplexCenter.y);

        computeShader.SetFloat("morphX", morph.x);
        computeShader.SetFloat("morphY", morph.y);



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
            complexCenter -= scrollSpeed/zoom*rotation;
        }
        if(Input.GetKey(KeyCode.D)){
            complexCenter += scrollSpeed/zoom*rotation;
        }
        //Rotatates 'rotation vector' with 90 degrees before applying linear transformation
        //[x,y] * (90 degree rotation) = [-y,x]
        if(Input.GetKey(KeyCode.W)){
            complexCenter += scrollSpeed/zoom*(new Vector2(-rotation.y, rotation.x));
        }
        if(Input.GetKey(KeyCode.S)){
            complexCenter -= scrollSpeed/zoom*(new Vector2(-rotation.y, rotation.x));
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
