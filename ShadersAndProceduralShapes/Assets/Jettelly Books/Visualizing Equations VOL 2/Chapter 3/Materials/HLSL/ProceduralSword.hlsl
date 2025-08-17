#include "Assets/Jettelly Books/Visualizing Equations VOL 2/Chapter 3/Materials/HLSL/AntiAliasingStep.hlsl"
#include "Assets/Jettelly Books/Visualizing Equations VOL 2/Chapter 5/Materials/HLSL/Procedural3DShapes.hlsl"

// Source: https://www.desmos.com/calculator/ubctkw43ya

// It refers to the equation: x^2 + (y + 0.245)^2 < 0.0809^2
float pommel (float2 uv)
{   
    float cx = (uv.x * uv.x);
    float cy = pow(uv.y + 0.245, 2);
    return length(cx + cy);
}

// It refers to the equation: min(u - |x|, 0)^8 + (y + 0.05)^8 < 0.05^8
float guard (float2 uv, float u)
{    
    float cx = pow(min(u - abs(uv.x), 0), 8);
    float cy = pow(uv.y + 0.05, 8);
    return length(cx + cy);
}

// It refers to the equation:x^2 + min(0.115 - |y + 0.13|, 0.06)^2 < 0.0809^2 {-0.1 > y}
float grip (float2 uv)
{    
    float cx = (uv.x * uv.x);
    float cy = pow(min(0.115 - abs(uv.y + 0.13), 0), 2);
    return length(cx + cy);
}

// It refers to the equation:y < max(0, smooth_min(z, s, l)) {0.5 < y}
float edge (float2 uv, float o, float m, float l, float c, float g)
{
    float z = m * abs(uv.x) + c;
    float s = o * (abs(uv.x) - g);
    
    float w = max(-0.01, smooth_min(z, s, l));
    uv.y -= w;    
    
    return uv.y;
}

void procedural_sword_float (in float2 uv, in float u, in float o, in float g, in float m, in float c,
    in bool predicate, in float size, in float3 colorA, in float3 colorB, in float3 colorC, out float4 Out)
{
    // Transform the UV coordinates to Pixel Art.       
    uv = (predicate) ? uv_to_pixel(uv, 64) : uv;
    // Center the procedural object.
    uv -= 0.5;
    // Increase or decrease its size.
    uv *= lerp(2.0, 1.0, size);    
   
    float sword_pommel = pommel(uv);
    float sword_pommel_radius = 0.0809 * 0.0809;
    sword_pommel = (predicate) ? 1 - step(sword_pommel_radius, sword_pommel) : 1 - anti_aliasing_step(sword_pommel, sword_pommel_radius);
    float3 sword_pommel_color = colorA * sword_pommel;
   
    float sword_guard = guard(uv, u);
    float sword_guard_radius = pow(0.05, 8);
    sword_guard = (predicate) ? 1 - step(sword_guard_radius, sword_guard) : 1 - anti_aliasing_step(sword_guard, sword_guard_radius);
    float3 sword_guard_color = colorA * sword_guard;
    
    float sword_grip = grip(uv);
    float sword_grip_radius = 0.06 * 0.06;
    sword_grip = (predicate) ? 1 - step(sword_grip_radius, sword_grip) : 1 - anti_aliasing_step(sword_grip, sword_grip_radius);
    
    sword_grip *= 1 - anti_aliasing_step(uv.y, -0.1);
    sword_grip *= 1 - sword_pommel;
    float3 sword_grip_color = colorB * sword_grip;
    
    float sword_edge = edge(uv, o, m, 0.01, c, g);
    sword_edge = (predicate) ?  1 - step(0.0, sword_edge) : 1 - anti_aliasing_step(sword_edge, 0);
    
    sword_edge *= (predicate) ?  (uv.y > -0.001) ? 1 : 0 : anti_aliasing_step(uv.y, 0);
    float3 sword_edge_color = colorC * sword_edge;

    float shine = (predicate) ? step(uv.x, 0) : 1 - anti_aliasing_step(uv.x, 0);
    shine = saturate(shine) * 0.25;

    float4 shape = 0;
    shape.rgb += sword_pommel_color;
    shape.rgb += sword_guard_color;
    shape.rgb += sword_grip_color;
    shape.rgb += sword_edge_color;    
    shape.a += sword_pommel;
    shape.a += sword_guard;
    shape.a += sword_grip;
    shape.a += sword_edge;

    shine *= shape.a;
    shape.rgb += shine;
    
    Out = shape;
}