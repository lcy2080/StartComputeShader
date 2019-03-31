using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultComputeShader : MonoBehaviour
{
    public ComputeShader computeShader;
    int csKernel;
    Texture2D resultTexture;
    
    public Material targetMaterial;
    void Start()
    {
        resultTexture = new Texture2D(512, 512, TextureFormat.ARGB32, false, true);
        targetMaterial.SetTexture("_MainTex", resultTexture);

        csKernel = computeShader.FindKernel("CSMain");

        RenderTexture tmpRT = RenderTexture.GetTemporary(512, 512, 16, RenderTextureFormat.ARGB32);
        tmpRT.enableRandomWrite = true;
        tmpRT.Create();
        
        computeShader.SetTexture(csKernel, "Result", tmpRT);
        //Execute
        computeShader.Dispatch(csKernel, 512, 512, 1);
        Graphics.CopyTexture(tmpRT, resultTexture);

        tmpRT.Release();
    }
}
