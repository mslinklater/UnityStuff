#include "Assets/Jettelly Books/Visualizing Equations VOL 2/Chapter 5/Materials/HLSL/Procedural3DShapes.hlsl"
#include "Assets/Jettelly Books/Shared/SimpleNoise/SimpleNoise.hlsl"
#include "Assets/Jettelly Books/Shared//PolarCoordinates/PolarCoordinates.hlsl"

#define MAX_ITERATIONS 256      
#define MAX_DISTANCE 100.0  
#define SURFACE_DISTANCE 0.001
#define IOR 2

inline float map(float3 p, float r)
{    
    float sphere = sphere_sd(p, r);    
    return sphere;
}

inline float render(float3 origin, float3 direction, float radius)
{
    float ray_distance = 0.0;                
    for ( int i = 0; i < MAX_ITERATIONS; i++)
    {
        float3 p = origin + ray_distance * direction;  
        float scene_distance = map(p, radius);         
        ray_distance += scene_distance;
        if(scene_distance < SURFACE_DISTANCE || ray_distance > MAX_DISTANCE)
        {
            break;                 
        }
    }    
    return ray_distance;
}


inline float3 get_normal(float3 p, float r)
{
    float2 e = float2(0.00001, 0);    // epsilon 1e-2
    float3 n = map(p, r)- float3(map(p - e.xyy, r), map(p - e.yxy, r), map(p - e.yyx, r));
    return normalize(n);
}

void RayMarching_float(in half Radius, in Texture2D<float4> MainTexture, in SamplerState SS, in float3 NormalWS, in float3 rayOrigin, in float3 rayDirection,
    in float3 lightDirection, in float3 ColorInput, out float3 RGB)
{   
    rayDirection = normalize(rayDirection);    
    float distance = render(rayOrigin, rayDirection, Radius);    
    half3 color = half3(0, 0, 0);       

    if (distance < MAX_DISTANCE)
    {
        float3 p = rayOrigin + distance * rayDirection;
        float3 normal = get_normal(p, Radius);        
        float3 r = refract(rayDirection, normal + NormalWS, IOR); 

        float2 uv_polar;
        Unity_PolarCoordinates_float(p.xy * 0.8 + 0.5, 0.5, 0.1, 1, uv_polar);
        uv_polar += float2(-_Time.y * 0.1, 0.0);
        float noise;
        Unity_SimpleNoise_float(uv_polar, 100, noise);
        float c = length(p.xy);
        c = smoothstep(c, c - 0.2, 0.04);
        noise *= c;
        noise *= 1;
        noise = (1 - noise);        
        color.rgb = SAMPLE_TEXTURE2D(MainTexture, SS, 0.1 * NormalWS.xy + (noise * p.xy + 0.5));
        

        if (1 - length(r) > 0)
        {
            color = ColorInput;
        }
    }
    else
    {
        color = ColorInput;
    }
    
    RGB = color;
}

















