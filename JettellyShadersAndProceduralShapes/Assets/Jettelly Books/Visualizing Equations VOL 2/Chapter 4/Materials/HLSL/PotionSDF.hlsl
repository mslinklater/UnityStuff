#include "Assets/Jettelly Books/Visualizing Equations VOL 2/Chapter 3/Materials/HLSL/AntiAliasingStep.hlsl"
#include "Assets/Jettelly Books/Visualizing Equations VOL 2/Chapter 5/Materials/HLSL/Procedural3DShapes.hlsl"

// Source : https://www.desmos.com/calculator/eut3noi65h

float capsule_sd(float2 p, float b, float w)
{
    return length(float2(p.x, p.y - clamp(p.y, 0.0, b))) - w;
}

float circle_sd(float2 p, float r)
{
    return length(p) - r;
}

void potion_sdf_float(in float2 uv, in float r, in float w, float time, in float3 colorA, in float3 colorB, in float3 colorC, in float size, out float4 Out)
{
    uv -= 0.5;    
    uv *= lerp(2.0, 1.0, size);
    
    float body = capsule_sd(uv + float2(0.0, 0.2), 0.6, r);
    body = 1 - anti_aliasing_step(body, r);
    body *= step(uv.y, 0.21);
    float3 body_color = colorA * body;
    
    float body_top = pow(min(((2 * r) - 0.05) - abs(uv.x), 0.0), 4) + pow(uv.y - 0.3, 4);
    body_top = 1 - anti_aliasing_step(body_top, pow(0.1, 4));
    float3 body_top_color = colorA * body_top;
    
    float body_cap = pow(min(((2 * r) - 0.15) - abs(uv.x), 0.0), 4) + pow(0.05 - abs(uv.y - 0.3), 4);
    body_cap = 1 - anti_aliasing_step(body_cap, pow(0.1, 4));
    float3 body_cap_color = colorB * body_cap;

    float t01 = frac(time * 0.54566);
    float t02 = frac(time * 0.35456);
    float t03 = time * 0.24354;

    float cc_x = lerp(0.0, 0.1, 10 * r * 2 - 1);
    float c01 = circle_sd(uv - float2(cc_x, lerp(-0.5, 0.1, t01)), 0.07 * lerp(1, 0, t01));
    float c02 = circle_sd(uv + float2(cc_x, lerp(0.5, -0.1, t02)), 0.05 * lerp(1, 0, t02));   

    float g01 = smooth_min(c01, c02, lerp(0.1, 0.0, 0.9));
    float g02 = uv.y - sin((uv.x + t03) * 10) * 0.05 + lerp(0.2, 0.0, w);

    float body_liquid = smooth_min(g02, g01, lerp(0.1, 0.0, 0.8));
    float liquid_capsule = capsule_sd(uv + float2(0.0, 0.2), 0.6, r);
    body_liquid = 1 - anti_aliasing_step(body_liquid, 0.0);    
    body_liquid *= 1 - anti_aliasing_step(liquid_capsule, r - 0.05);
    float3 liquid_color = colorC * body_liquid;

    float body_shadow = capsule_sd(uv + float2(0.0, 0.2), 0.6, r);
    body_shadow = 1 - anti_aliasing_step(body_shadow, r - 0.05);
    body_shadow *= step(uv.y, 0.4);
    body_shadow *= 0.1;

    float body_sheen = pow(min(lerp(0.1, 0.2, 10 * r * 2 - 1) - abs(uv.x), 0), 2) + pow(uv.y - 0.35, 2);
    body_sheen = 1 - anti_aliasing_step(body_sheen, pow(0.025, 2));
    body_sheen *= 0.5;

    float4 shape = 0;
    shape.rgb += max(body_color, body_top_color);    
    shape.rgb += body_cap_color;
    shape.rgb = lerp(shape.rgb, liquid_color, body_liquid);
    shape.rgb -= body_shadow;
    shape.rgb += body_sheen;
    shape.a += body;
    shape.a += body_top;
    shape.a += body_cap;
    
    Out = shape;
    
}