Shader "Custom/NormalToHeight"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_HeightMap("Heightmap", 2D) = "gray" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _HeightMap;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float val = tex2D(_HeightMap, IN.uv_MainTex);
			float UVOffset = 1.0 / 1024.0;
			float valU = tex2D(_HeightMap, IN.uv_MainTex + float2(UVOffset, 0));
			float valV = tex2D(_HeightMap, IN.uv_MainTex + float2(0, UVOffset));
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			float diffU = (valU - val) * 50.0;
			float diffV = (valV - val) * 50.0;
			float sufNormalDirection = sqrt(1.0 - saturate(diffU * diffU + diffV * diffV));
			//o.Normal = float3(diffU, sufNormalDirection, diffV);
			o.Normal = float3(diffU, diffV, 1);
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
