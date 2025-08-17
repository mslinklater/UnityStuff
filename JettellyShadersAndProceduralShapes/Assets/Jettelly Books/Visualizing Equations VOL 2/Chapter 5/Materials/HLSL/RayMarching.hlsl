#include "Assets/Jettelly Books/Visualizing Equations VOL 2/Chapter 5/Materials/HLSL/Procedural3DShapes.hlsl"

#define MAX_ITERATIONS 256      
#define MAX_DISTANCE 100.0  
#define SURFACE_DISTANCE 0.001
#define IOR 1.88

inline float map(float3 p)
{
    const float sphere_radius = 0.3;
    float sphere = sphere_sd(p, sphere_radius);

    const float capsule_radius = 0.1;
    const float k = 0.001;
    const float3 capsule_p0 = float3(0.0, 0.35, 0.0);
    const float3 capsule_p1 = float3(0.0, -0.35, 0.0);
    float capsule = capsule_sd(p, capsule_p0, capsule_p1, capsule_radius);
    
    return smooth_min(sphere, capsule, k);
}

inline float render(float3 rayOrigin, float3 rayDirection, float side)  
{
    float ray_distance = 0.0;                 
    for ( int i = 0; i < MAX_ITERATIONS; i++)
    {
        float3 p = rayOrigin + ray_distance * rayDirection;    
        float scene_distance = map(p) * side;         
        ray_distance += scene_distance;
        if(scene_distance < SURFACE_DISTANCE || ray_distance > MAX_DISTANCE)
        {
            break;                 
        }
    }    
    return ray_distance;
}


// inline float3 get_normal(float3 p)
// {
//     float2 e = float2(0.00001, 0);
//     float3 n = map(p) - float3(map(p - e.xyy), map(p - e.yxy), map(p - e.yyx));
//     return normalize(n);
// }


inline float3 get_normal(float3 p)
{
    float h = 1e-5;
    float x = (map(p + float3(h, 0, 0)) - map(p)) / h;
    float y = (map(p + float3(0, h, 0)) - map(p)) / h;
    float z = (map(p + float3(0, 0, h)) - map(p)) / h;
    
    return normalize(float3(x, y, z));
}


void ray_marching_float(in float3 rayOrigin, in float3 rayDirection, in float3 lightDirection, out float3 RGB)
{   
    rayDirection = normalize(rayDirection);    
    float distance = render(rayOrigin, rayDirection, 1.0);    
    half3 color;       

    if (distance < MAX_DISTANCE)
    {
        float3 p = rayOrigin + distance * rayDirection;
        float3 normal = get_normal(p);
        float l = max(0.0, dot(normal, -lightDirection)) + 0.1;
        l = saturate(l);
        color = l;
    }
    else
    {        
        discard;
    }   
    
    RGB = color;
}

















