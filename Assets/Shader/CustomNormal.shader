Shader "Custom/CustomNormal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Normal ("Normal", 2D) = "gray" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _Normal;

        struct Input
        {
            float2 uv_MainTex;
        };

        inline fixed3 CustomUnpackNormal(fixed4 packednormal)
        {
            fixed3 normal;
            normal.xy = packednormal.xy * 2.0 - 1;
            normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
            return normal;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Normal = CustomUnpackNormal(tex2D(_Normal, IN.uv_MainTex));
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
