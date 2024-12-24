Shader "Custom/DoubleSided"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Off   // Disable back-face culling
            ZWrite On
            ZTest LEqual
            ColorMaterial AmbientAndDiffuse
            Lighting On
            SetTexture [_MainTex] { combine texture }
        }
    }
}
