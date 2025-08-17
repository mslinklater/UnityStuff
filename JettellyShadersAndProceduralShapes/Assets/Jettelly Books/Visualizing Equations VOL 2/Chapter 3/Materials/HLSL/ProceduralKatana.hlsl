#include "Assets/Jettelly Books/Visualizing Equations VOL 2/Chapter 3/Materials/HLSL/AntiAliasingStep.hlsl"

// Source: https://www.desmos.com/calculator/rbbnq3cyh8

float edge (float2 uv, float s, float q, float r, float g, bool predicate)
{
    float cx = pow(min(s - abs((uv.x + 0.05) + s/2), 0), 2);
    float cy = pow(min(q - abs(uv.y), 0), 2);
    float c = (predicate) ? step(cx + cy, r * r) : 1 - anti_aliasing_step(cx + cy, r * r);    
    c *= (predicate) ? (uv.x + 0.05) + s/2 > 0: anti_aliasing_step((uv.x + 0.05) + s/2, 0);
    c *= (predicate) ? uv.y + 0.06 > 0:  anti_aliasing_step(uv.y + 0.06, 0);
    c *= (predicate) ? pow(uv.x + 0.05 + s/2, 2) + pow(uv.y + 0.07, 2) > pow(0.05, 2) * g * s :
    anti_aliasing_step(pow(uv.x + 0.05 + s/2, 2) + pow(uv.y + 0.07, 2), pow(0.05, 2) * g * s);    
    return c;
}

float grip (float2 uv, bool predicate)
{
    float cx = pow(min(0.0 - abs(uv.x), 0), 4);
    float cy = pow(min(0.1 - abs(uv.y + 0.25), 0), 4);
    float c = (predicate) ? step(cx + cy, pow(0.04, 4)) : 1 - anti_aliasing_step(cx + cy, pow(0.04, 4));
    c *= (predicate) ? uv.y < -0.14 : 1 - anti_aliasing_step(uv.y, -0.14);
    c *= (predicate) ? pow(uv.x, 2) + pow((uv.y + 0.4), 2) > pow(0.05, 2): anti_aliasing_step(pow(uv.x, 2) + pow((uv.y + 0.4), 2), pow(0.05, 2));
    return c;
}

float guard (float2 uv, float s, bool predicate)
{
    float cx = pow(min(0.05 + s/2 - abs(uv.x), 0), 4);
    float cy = pow(min(0.00 - abs(uv.y + 0.1), 2), 4);
    return (predicate) ? step(cx + cy, pow(0.04, 4)) : 1 - anti_aliasing_step(cx + cy, pow(0.04, 4));
}

float pommel (float2 uv, float r, bool predicate)
{
    float cx = uv.x * uv.x;
    float cy = pow(uv.y + 0.4, 2);
    return (predicate) ? step(cx + cy, r * r) : 1 - anti_aliasing_step(cx + cy, r * r);
}

float cube (float2 uv, float g, bool predicate)
{
    float cx = abs(uv.x);
    float cy = abs(g + uv.y);
    return (predicate) ? step(cx + cy, 0.03) : 1 - anti_aliasing_step(cx + cy, 0.03);
}

void procedural_katana_float (in float2 uv,
    in float s,
    in float q,
    in float size,
    in float3 colorA,
    in float3 colorB,
    in float3 colorC,
    in bool predicate,
    out float4 Out)
{   
    uv = (predicate) ? uv_to_pixel(uv, 128) : uv;   
    uv -= 0.5;   
    uv *= lerp(2.0, 1.0, size);
    
    float katana_edge = edge(uv, s, q, 0.1, 5, predicate);   
    float katana_edge_internal = edge(uv, s - 0.04, q - 0.04, 0.1, 10, predicate) * 0.9;
    float katana_edge_render = katana_edge + (1 - katana_edge_internal) * katana_edge;
    float3 katana_edge_color = colorA * katana_edge_render;   
    
    float katana_grip = grip(uv, predicate);
    float3 katana_grip_color = colorB * katana_grip;
    
    float katana_guard = guard(uv, s, predicate);
    float3 katana_guard_color = colorC * katana_guard;
    
    float katana_pommel = pommel(uv, 0.05, predicate);
    float3 katana_pommel_color = colorC * katana_pommel;

    float katana_cube_top = cube(uv, 0.2, predicate);
    float katana_cube_bottom = cube(uv, 0.3, predicate);
    katana_cube_top += katana_cube_bottom;
    float3 katana_cubes = colorC * katana_cube_top;

    float katana_shine = (predicate) ? (uv > 0) * 0.1: anti_aliasing_step(uv.x, 0) * 0.1;

    float4 shape = 0;
    shape.rgb += katana_edge_color;
    shape.rgb += katana_grip_color;
    shape.rgb += katana_guard_color;
    shape.rgb += katana_pommel_color;
    shape.rgb += katana_cubes;
    shape.rgb += katana_shine;
    shape.a += katana_edge;
    shape.a += katana_grip;
    shape.a += katana_guard;
    shape.a += katana_pommel;
    
    Out = shape;
}