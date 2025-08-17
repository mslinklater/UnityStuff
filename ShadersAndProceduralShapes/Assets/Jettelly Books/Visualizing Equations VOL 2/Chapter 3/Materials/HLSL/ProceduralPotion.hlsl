#include "Assets/Jettelly Books/Visualizing Equations VOL 2/Chapter 3/Materials/HLSL/AntiAliasingStep.hlsl"
#include "Assets/Jettelly Books/Visualizing Equations VOL 2/Chapter 2/Materials/HLSL/PointReflection.hlsl"

// Source: https://www.desmos.com/calculator/qlhfzxtke0

// it refers to the equation: x^2 + (y + 0.1)^2 < k^2
float body (float2 uv, float r, bool predicate)
{   
    float cx = (uv.x * uv.x);
    float cy = (uv.y * uv.y);
    return (predicate) ? step(cx + cy, r) : 1 - anti_aliasing_step(cx + cy, r);
}

// it refers to the equation: x^2 + min(0.5 - |y - 0.5|, 0)^2 < 0.1^2 {y < c} - 0.5 {x^2 + (y + 0.1)^2 > k^2}
float top (float2 uv, float r, bool predicate)
{   
    float cx = (uv.x * uv.x);
    float cy = pow(min(0.5 - abs(uv.y - 0.5), 0), 2);
    return (predicate) ?  step(cx + cy, r) : 1 - anti_aliasing_step(cx + cy, r);
}

// It refers to the equation: min(0.1 - |x|, 0)^4 + (y - c + 0.45)^4 < 0.05^4
float cap (float2 uv, float r, float a, bool predicate)
{    
    float cx = pow(min(0.1 - abs(uv.x), 0), 4);
    float cy = pow(uv.y - a + 0.45, 4);
    return (predicate) ? step(cx + cy, r) : 1 - anti_aliasing_step(cx + cy, r);
}

// It refers to the equation: x^2 + min(u - |y - c + 0.25|, 0)^2 < r^2 {y < c - 0.5}
float cap_inside (float2 uv, float r, float u, float c, bool predicate)
{
    float ax = (uv.x * uv.x);
    float ay = pow(min(u - abs(uv.y - c + 0.25), 0), 2);
    return (predicate) ? step(ax + ay, r) : 1 - anti_aliasing_step(ax + ay, r);
}

// It refers to the equation: y + 0.1 < sin((x + y) * pi * 3) * a {x^2 + (y + 0.1)^2 < k^2 - 0.02} {y + 0.1 > sin((x + t) * pi * 3) * a}
float liquid (float2 uv, float e, float a, float t, bool predicate)
{    
    float w = sin((uv.x + t) * PI * 3.0) * a;
    uv.y -= w;    
    return (predicate) ? step(uv.y, e) : 1 - anti_aliasing_step(uv.y, e);
}

// It refers to the equation: (x - 0.35 * k)^2 + (y - 0.05 * k)^2 < 0.1^2 * k
float shine (float2 uv, float r, float k, bool predicate)
{    
    float c = pow(uv.x - 0.35 * k, 2) + pow(uv.y - 0.05 * k, 2);
    return (predicate) ? step(c, r) : 1 - anti_aliasing_step(c, r);
}

void procedural_potion_float (in float2 uv, in float k, in float r, in float u, in float c, in float a,
    in float3 colorA, in float3 colorB, in float3 colorC, in float size, in float time, in bool predicate, out float4 Out)
{
    uv = (predicate) ? uv_to_pixel(uv, 64) : uv;
    uv -= 0.5;    
    uv *= lerp(2.0, 1.0, size);

    float2 uv_offset = float2(uv.x, uv.y + 0.1);

    float bottle_body = body(uv_offset, k * k, predicate);
    float bottle_body_inside = body(uv_offset, k * k + r - 0.08, predicate);
    float bottle_top = top(uv, 0.1 * 0.1, predicate) * step(uv.y, c - 0.5);
    float bottle_cap = cap(uv, pow(0.05, 4), c, predicate);
    
    bottle_body = saturate(bottle_body + bottle_top);  
    
    float3 bottle_body_color = colorA * bottle_body;
    float3 bottle_cap_color = colorB * bottle_cap;

    float bottle_cap_inside = cap_inside(uv, r * r, u, c, predicate);
    bottle_cap_inside *= step(uv.y, c -0.5);
    
    float bottle_front_wave = liquid(uv_offset, 0.0, a, time * 0.5, predicate);
    bottle_front_wave *= bottle_body_inside;

    float bottle_back_wave = liquid(uv_offset, 0.0, a, time * -0.5, predicate);
    bottle_back_wave *= bottle_body_inside;

    bottle_body_inside += saturate(bottle_body_inside + 0.8);
    float bottle_shine = shine(uv, (0.1 * 0.1) * k, k, predicate);

    float4 shape = 0;
    shape.rgb += bottle_body_color;
    shape.rgb += bottle_cap_color;   
    shape.rgb = lerp(shape.rgb, colorB * 0.5, bottle_cap_inside);
    shape.rgb = lerp(shape.rgb, colorC, bottle_front_wave);
    shape.rgb = lerp(shape.rgb, colorC * 0.5, bottle_back_wave);
    shape.rgb *= bottle_body_inside;
    shape.rgb += bottle_shine;
    shape.a += bottle_body;
    shape.a += bottle_cap;
    
    Out = shape;
}