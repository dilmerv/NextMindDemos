﻿Shader "NextMind/Stimulation_transparent"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Main Texture", 2D) = "white" {}
        _Stimulation("Stimulation", 2D) = "gray" {}
        _Blend("Blend",Range(0,1)) = 1
        _Density("Density", Float) = 1
        _ScreenRatio("Screen Ratio", Float) = 1.777
        [Toggle] _OverlayBlending("Overlay blending", Float) = 0
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent"}

        CGPROGRAM
        #include "StimulationIncludes.cginc"
        #pragma surface surf Standard fullforwardshadows alpha:fade

        #pragma shader_feature STANDARD SCREEN_COORDINATES TRIPLANAR
        #pragma shader_feature CONSTANT_SIZE
        #pragma shader_feature MAIN_ALPHA

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float4 tex;

            float4 baseTex = tex2D(_MainTex, IN.uv_MainTex) * _Color;

#if SCREEN_COORDINATES
            tex = ScreenCoordinatesProjection(IN, baseTex);
#elif TRIPLANAR
            tex = TriplanarProjection(IN, baseTex);
#elif STANDARD
            tex = StandardProjection(IN, baseTex);
#endif

            o.Albedo = tex.xyz;
            o.Alpha = tex.w;
        }

        ENDCG
    }
            
    Fallback "Diffuse"
    CustomEditor "NextMindTransparentShaderGUI"
}