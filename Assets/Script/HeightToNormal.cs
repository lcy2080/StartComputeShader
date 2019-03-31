using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightToNormal : MonoBehaviour
{
    public ComputeShader computeShader;
    int csKernel;
    
    public RenderTexture HeightRT;
    RenderTexture NormalRT;

    public Material TargetMaterial;

    public ComputeBuffer argsBuffer;

    void Start()
    {
        NormalRT = new RenderTexture(HeightRT.width, HeightRT.height, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
        {
            enableRandomWrite = true
        };
        NormalRT.Create();

        csKernel = computeShader.FindKernel("CSHeightToNormal");
        computeShader.SetTexture(csKernel, "HeightRT", HeightRT);
        computeShader.SetTexture(csKernel, "NormalRT", NormalRT);

        TargetMaterial.SetTexture("_Normal", NormalRT);

        texture = new Texture2D(1024, 1024, TextureFormat.ARGB32, false, true);
        targetArray = new Color[1024 * 1024];

#region Setup Indirect Dispatch Arguments
        
        argsBuffer = new ComputeBuffer(6, sizeof(int), ComputeBufferType.IndirectArguments);
        int[] args = new int[6];
        
        args[0] = 1024 / 32;
        args[1] = 1024 / 32;
        args[2] = 1;

        args[3] = 1024 / 64;
        args[4] = 1024 / 64;
        args[5] = 1;

        argsBuffer.SetData(args);

#endregion //Setup Indirect Dispatch Arguments

    }

    // Update is called once per frame
    void Update()
    {
        ChangeHeightToNormal();
    }

    public void ChangeHeightToNormal()
    {
        computeShader.Dispatch(csKernel, 131072, 1, 1);
        //computeShader.DispatchIndirect(csKernel, argsBuffer, 0);
    }

#region CPU Change Height to Normal

    Texture2D texture;
    Color[] array;
    Color[] targetArray;

    public void CPUChangeHeightToNormal()
    {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = HeightRT;
        texture.ReadPixels(new Rect(0, 0, 1024, 1024), 0, 0);
        texture.Apply();
        //Garbage 발생
        array = texture.GetPixels();

        for (int i = 0; i < array.Length; i++)
        {
            Vector2Int index = new Vector2Int(i % 1024, i / 1024);

            int right = index.y * 1024 + Mathf.Min(index.x + 1, 1023);
            int left = index.y * 1024 + Mathf.Max(index.x - 1, 0);
            int up = Mathf.Min(index.y + 1, 1023) * 1024 + index.x;
            int down = Mathf.Max(index.y - 1, 0) * 1024 + index.x;

            targetArray[i].r = 0.5f + (array[right].r - array[left].r) * 10.0f;
            targetArray[i].g = 0.5f + (array[up].r - array[down].r) * 10.0f;
            targetArray[i].b = 1.0f;
            targetArray[i].a = 1.0f;
        }

        texture.SetPixels(targetArray);
        texture.Apply();

        Graphics.Blit(texture, NormalRT);
        RenderTexture.active = prev;
    }
#endregion //CPU Change Height to Normal
}
