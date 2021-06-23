// NextMind's shaders source.

struct Input
{
    float2 uv_MainTex;
    float2 uv_Stimulation;
    float3 worldNormal;
    float3 worldPos;
    float4 screenPos;
};

// Base texture on which the stimulation will be applied.
sampler2D _MainTex;

// Color applied on main texture.
fixed4 _Color;

// Stimulation texture.
sampler2D _Stimulation;

// Blend factor between main and stimulation textures. This variable is automatically set during runtime by the NeuroManager.
fixed _Blend;

// Use as the stimulation texture scale in a triplanar context. Initialliy called density because it is a simple way to deal with "beans" density.
half _Density;

bool _OverlayBlending;

void Unity_Blend_Overlay_float4(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    float4 result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
    float4 result2 = 2.0 * Base * Blend;
    float4 zeroOrOne = step(Base, 0.5);
    Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
    Out = lerp(Base, Out, Opacity);
}

float4 GetProjectedTexture(sampler2D tex, half3 weights, float3 uv)
{
    float4 beansTex = 0;
    if (weights.x > weights.y && weights.x > weights.z)
    {
        beansTex = tex2D(tex, uv.yz);
    }
    else if (weights.y > weights.x && weights.y > weights.z)
    {
        beansTex = tex2D(tex, uv.xz);
    }
    else if (weights.z > weights.x && weights.z > weights.y)
    {
        beansTex = tex2D(tex, uv.xy);
    }

    return beansTex;
}

float4 BlendStimulationTexture(float4 mainTex, float4 beansTex)
{
    float4 textureToBlend = 0;
    if (_OverlayBlending)
    {
        // Apply overlay 
        Unity_Blend_Overlay_float4(beansTex, mainTex, 1, textureToBlend);
    }
    else
    {
        textureToBlend = beansTex;
    }

    // The full blended texture will use the main texture if the StimulationTexture is transparent.
    float4 fullBlendedTexture = lerp(mainTex, textureToBlend, beansTex.a);

    float4 retTex = lerp(mainTex, fullBlendedTexture, _Blend);

#if MAIN_ALPHA
    retTex.w = mainTex.a;
#else
    retTex.w = lerp(mainTex.a, beansTex.a, _Blend);
#endif

    return retTex; 
}

float4 ScreenCoordinatesProjection(Input IN, float4 mainTex)
{
    float2 textureCoordinate = _Density * (IN.screenPos / IN.screenPos.w);

    float aspect = _ScreenParams.x / _ScreenParams.y;
    textureCoordinate.x = textureCoordinate.x * aspect;
   
    float4 beansTex = tex2D(_Stimulation, textureCoordinate);

    return BlendStimulationTexture(mainTex, beansTex);
}

// Returns distance between the camera and the center of the object.
float CameraDistance()
{
    float4 objectOrigin = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
    return length(objectOrigin - _WorldSpaceCameraPos.xyz);
}

float4 StandardProjection(Input IN, float4 mainTex)
{
    float2 f = IN.uv_Stimulation;
    f *= _Density;

#if CONSTANT_SIZE
    // Modulate the density regarding the distance to the camera.
    f /= CameraDistance();
#endif 
    
    float4 beansTex = tex2D(_Stimulation, f);

    return BlendStimulationTexture(mainTex, beansTex);
}

float4 TriplanarProjection(Input IN, float4 mainTex)
{
    // Use absolute value of normal as texture weights.
    float3 localNormal = mul(unity_WorldToObject, IN.worldNormal);
    half3 weights = abs(localNormal);
    // Make sure the weights sum up to 1 (divide by sum of x+y+z).
    weights /= dot(weights, 1.0);

    float3 f = mul(unity_WorldToObject, float4(IN.worldPos, 1));
    
    // Don't let the stimulation texture be affected by the object scale.
    float3 worldScale = float3(
        length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)), // scale x axis
        length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)), // scale y axis
        length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))  // scale z axis
        );
    f *= worldScale * _Density;

#if CONSTANT_SIZE
    // Modulate the density regarding the distance to the camera.
    f /= CameraDistance();
#endif 

    f += float3(0.5, 0.5, 0.5);

    // Read the three texture projections, for x,y,z axes.
    float4 beansTex = GetProjectedTexture(_Stimulation, weights, f);

    return BlendStimulationTexture(mainTex, beansTex);
}



