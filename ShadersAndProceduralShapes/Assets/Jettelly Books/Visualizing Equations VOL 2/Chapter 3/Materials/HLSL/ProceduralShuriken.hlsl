#include "Assets/Jettelly Books/Visualizing Equations VOL 2/Chapter 1/Materials/HLSL/Segment.hlsl"
#include "Assets/Jettelly Books/Visualizing Equations VOL 2/Chapter 3/Materials/HLSL/AntiAliasingStep.hlsl"

// Source: https://www.desmos.com/calculator/8cc82ghuwo

float shuriken (float2 uv, float x0, float radius, bool predicate)
{
    // Y coordinate.
    float y = abs(uv.y);
    // X coordinate.
    float x = abs(uv.x);
    // Point p0. Can be constant.
    float2 p0 = float2(0.0, 0.5);
    // Point p1.
    float2 p1 = float2(x0, x0);   
    // First segment.
    float segment_p0_p1 = y - (tangent(p0, p1) * (x - p0.x) + p0.y);
    segment_p0_p1 = (predicate) ? step(segment_p0_p1, 0.0) : 1 - anti_aliasing_step(segment_p0_p1, 0.0);
    // The normal of the rect.
    float2 n = float2(-1 / sqrt(2), 1 / sqrt(2));
    // Point p2, the reflection of p0.
    float2 p2 = p0 - 2.0 * n * dot(p0, n);
    // Second segment.
    float segment_p2_p1 = y - (tangent(p2, p1) * (x - p2.x) + p2.y);
    segment_p2_p1 = (predicate) ? step(segment_p2_p1, 0.0) : 1 - anti_aliasing_step(segment_p2_p1, 0.0);
    // The radius of the circles.
    float r = radius * radius;
    // The reflected circle.
    float reflected_circle = (x - x0) * (x - x0) + (y - x0) * (y - x0);
    reflected_circle = (predicate) ? 1 - step(reflected_circle, r): anti_aliasing_step(reflected_circle, r);
    // The center circle.
    float circle = (x * x) + (y * y);
    circle = (predicate) ? 1 - step(circle, r) : anti_aliasing_step(circle, r);
    // Add the segments.
    segment_p0_p1 += segment_p2_p1;    
    segment_p0_p1 *= reflected_circle;
    segment_p0_p1 *= circle;    
    
    return saturate(segment_p0_p1);
}

void procedural_shuriken_float (in float2 uv, in float x0, in float radius, in float size, in float3 colorA, in float3 colorB, in bool predicate, out float4 Out)
{    
    // If to pixel art is true, transform to pixel.
    uv = (predicate) ? uv_to_pixel(uv, 64) : uv;    
    // Center the Shuriken in the middle of the Quad.
    uv -= 0.5;
    // Increase or decrease its size.
    uv *= lerp(2.0, 1.0, size);    
    // A light mask.
    const float d0 = 0.9;    
    float sharp = shuriken(uv, x0, radius * d0, predicate);
    float body = shuriken(uv, x0 * d0, radius, predicate);

    float v0 = (sharp * d0) * body;
    float3 l0 = (predicate) ? 1 - step(float3(uv, 1), 0) : anti_aliasing_step(float3(uv, 1), 0);    
    float l0_gray = saturate(dot(l0, float3(0.126, 0.7152, 0.0722)));
    
    float4 shape = 0;
    shape.rgb = lerp(colorA, colorB, v0);
    shape.rgb *= l0_gray;
    shape.a = sharp;
    
    Out = shape;
}