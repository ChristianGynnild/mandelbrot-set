using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class compute_manager : MonoBehaviour
{
    public int maxIterations = 500;
    
    public Vector4 hue = new Vector4(0.17f, 1.784f, 0.335f, 1.0f);

    public Vector2 complexCenter;
    public float angle;
    public float zoom;
    public Vector2 morph;

    Vector2 smoothComplexCenter;
    float smoothAngle;
    float smoothZoom;
    Vector2 smoothMorph;

    Vector2 complexCenter0;
    float angle0;
    float zoom0;
    Vector2 morph0;
    
    public float transitionSpeed;
    public float zoomTransitionSpeed;

    public ComputeShader computeShader;
    public RenderTexture renderTexture;
    
    int currenResolutionWidth, currenResolutionHeight;


    private void resetVariables(){
        complexCenter = complexCenter0;
        angle = angle0;
        zoom = zoom0;
        morph = morph0;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        smoothComplexCenter = complexCenter;
        smoothAngle = angle;
        smoothZoom = zoom;
        smoothMorph = morph;


        complexCenter0 = complexCenter;
        angle0 = angle;
        zoom0 = zoom;
        morph0 = morph;
    }


    
    private void OnRenderImage(RenderTexture src, RenderTexture dest){
        int kernelHandle = computeShader.FindKernel("CSMain");

        float deltaTimeTransitionSpeed = Mathf.Min(Time.deltaTime*transitionSpeed, 1.0f);

        smoothAngle = Mathf.Lerp(smoothAngle, angle, deltaTimeTransitionSpeed);
        smoothComplexCenter = new Vector2(Mathf.Lerp(smoothComplexCenter.x, complexCenter.x, deltaTimeTransitionSpeed), Mathf.Lerp(smoothComplexCenter.y, complexCenter.y, deltaTimeTransitionSpeed));
        smoothZoom = Mathf.Lerp(smoothZoom, zoom, Mathf.Pow(Mathf.Min(Time.deltaTime*zoomTransitionSpeed, 1.0f), 0.25f));
        smoothMorph = new Vector2(Mathf.Lerp(smoothMorph.x, morph.x, deltaTimeTransitionSpeed), Mathf.Lerp(smoothMorph.y, morph.y, deltaTimeTransitionSpeed));

        computeShader.SetInt("screenWidth", Screen.width);
        computeShader.SetInt("screenHeight", Screen.height);
        computeShader.SetInt("maxIterations", maxIterations);

        computeShader.SetVector("hue", hue);
        computeShader.SetFloat("zoom", smoothZoom);
        computeShader.SetFloat("angle", smoothAngle);
        computeShader.SetFloat("centerReal", smoothComplexCenter.x);
        computeShader.SetFloat("centerImaginary", smoothComplexCenter.y);

        computeShader.SetFloat("morphX", smoothMorph.x);
        computeShader.SetFloat("morphY", smoothMorph.y);



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

    public float scrollSpeed;
    public float zoomSpeed;
    public float rotationSpeed;
    public float morphSpeed;

    void Update()
    {
        Vector2 rotation = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
 
        if(Input.GetKey(KeyCode.A)){
            complexCenter -= scrollSpeed/zoom*rotation*Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.D)){
            complexCenter += scrollSpeed/zoom*rotation*Time.deltaTime;
        }
        //Rotatates 'rotation vector' with 90 degrees before applying linear transformation
        //[x,y] * (90 degree rotation) = [-y,x]
        if(Input.GetKey(KeyCode.W)){
            complexCenter += scrollSpeed/zoom*(new Vector2(-rotation.y, rotation.x))*Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.S)){
            complexCenter -= scrollSpeed/zoom*(new Vector2(-rotation.y, rotation.x))*Time.deltaTime;
        }


        if(Input.GetKey(KeyCode.LeftArrow)){
            morph += morphSpeed/zoom*rotation*Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.RightArrow)){
            morph -= morphSpeed/zoom*rotation*Time.deltaTime;
        }
        //Rotatates 'rotation vector' with 90 degrees before applying linear transformation
        //[x,y] * (90 degree rotation) = [-y,x]
        if(Input.GetKey(KeyCode.UpArrow)){
            morph -= morphSpeed/zoom*(new Vector2(-rotation.y, rotation.x))*Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.DownArrow)){
            morph += morphSpeed/zoom*(new Vector2(-rotation.y, rotation.x))*Time.deltaTime;
        }


        if(Input.GetKey(KeyCode.C)){
            zoom += (1.0f + Mathf.Abs(zoom))*zoomSpeed*Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.X)){
            zoom -= (1.0f + Mathf.Abs(zoom))*zoomSpeed*Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.Q)){
            angle += rotationSpeed*Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.E)){
            angle -= rotationSpeed*Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.R)){
            resetVariables();
        }
        
    }


}
