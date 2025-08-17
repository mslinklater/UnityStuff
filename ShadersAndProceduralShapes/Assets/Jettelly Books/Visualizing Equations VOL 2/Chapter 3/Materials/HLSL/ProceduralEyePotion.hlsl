#include "Assets/Jettelly Books/Visualizing Equations VOL 2/Chapter 3/Materials/HLSL/AntiAliasingStep.hlsl"
#include "Assets/Jettelly Books/Visualizing Equations VOL 2/Chapter 2/Materials/HLSL/PointReflection.hlsl"

// Source: https://www.desmos.com/calculator/k9nhgdz6nk

// it refers to the equation: (x^8 * c) + min(0.2 - |y - 0.1|, 0)^8 < 0.3^8 {y < 0.1}
float body_bottom (float2 uv, float c, float r, bool predicate)
{
    const int ex = 8;
    float cx = pow(uv.x * c, ex);
    float cy = pow(min(0.2 - abs(uv.y - 0.1), 0), ex);      
    return (predicate) ? step(cx + cy, pow(r, ex)) : 1 - anti_aliasing_step(cx + cy, pow(r, ex));
}

// it refers to the equation: (x * c)^2 + ((y - 0.1) * 2)^2 < 0.3^2 {y > 0.1}
float body_top (float2 uv, float c, float r, bool predicate)
{
    float cx = (uv.x * c) * (uv.x * c);
    float cy = ((uv.y - 0.1) * 2) * ((uv.y - 0.1) * 2);
    return (predicate) ? step(cx + cy, r * r) : 1 - anti_aliasing_step(cx + cy, r * r);
}

// it refers to the equation: x^2 + min(0.1 - |y - 0.3|, 0)^2 < 0.15^2 {y > (x * c)^2 + ((y - 0.1) * 2)^2 > 0.3^2} {y < 0.35}
float top (float2 uv, float r, bool predicate)
{
    float cx = uv.x * uv.x;
    float cy = min(0.1 - abs(uv.y - 0.3), 0) * min(0.1 - abs(uv.y - 0.3), 0);
    return (predicate) ? step(cx + cy, r * r) : 1 - anti_aliasing_step(cx + cy, r * r);
}

// it refers to the equation: min(0.15 - |x|, 0)^2 + (y - 0.4)^2 < 0.05^2
float cap (float2 uv, float r, bool predicate)
{
    float cx = min(0.15 - abs(uv.x), 0) * min(0.15 - abs(uv.x), 0);
    float cy = (uv.y - 0.4) * (uv.y - 0.4);
    return (predicate) ? step(cx + cy, r * r) : 1 - anti_aliasing_step(cx + cy, r * r);
}

// it refers to the equation: min(0.01 - |x|, 0)^4 + (y - 0.3)^4 < 0.1^4 {y < 0.35}
float cap_inside (float2 uv, float r, bool predicate)
{
    const int ex = 4;
    float cx = pow(min(0.01 - abs(uv.x), 0), ex);
    float cy = pow(uv.y - 0.3, ex);
    return (predicate) ? step(cx + cy, pow(r, ex)) : 1 - anti_aliasing_step(cx + cy, pow(r, ex));
}

// it refers to the equation: y < sin((x - t) * pi * 3) * k {(x * c)^8 + min(0.2 - |y - 0.1|, 0)^8 < 0.25^8}
float liquid (float2 uv, float t, float k, bool predicate)
{
    float w = sin((uv.x - t) * PI * 3) * k;
    uv.y -= w;
    return (predicate) ? step(uv.y, 0) : 1 - anti_aliasing_step(uv.y, 0);
}

// it refers to the equation: x^2 + (y - a)^2 < 0.15^2
float eye (float2 uv, float a, bool predicate)
{  
    float cx = (uv.x * uv.x);
    float cy = ((uv.y - a) * (uv.y - a));
    float cr = 0.15 * 0.15;
    return (predicate) ? step(cx + cy, cr) : 1 - anti_aliasing_step(cx + cy, cr);
}

// it refers to the equation: ((x - 0.01) - sin(t) * 0.05)^2 + (((y - a) - 0.01) - cos(t) * 0.05)^2 < 0.07^2
float iris (float2 uv, float t, float a, float r, bool predicate)
{    
    float cx = ((uv.x - 0.01) - sin(t) * 0.05) * ((uv.x - 0.01) - sin(t) * 0.05);
    float cy = (((uv.y - a) - 0.01) - cos(t) * 0.05) * (((uv.y - a) - 0.01) - cos(t) * 0.05);    
    return (predicate) ? step(cx + cy, r * r) : 1 - anti_aliasing_step(cx + cy, r * r);
}

// it refers to the equation: ((x * c) - 0.24)^2 + min(0.05 - |y - 0.07|, 0)^2 < 0.035^2
float shine (float2 uv, float c, float r, bool predicate)
{    
    float cx = pow(((uv.x * c) - 0.24), 2);
    float cy = pow(min(0.05 - abs(uv.y - 0.07), 0), 2);
    return (predicate) ? step(cx + cy, r * r) : 1 - anti_aliasing_step(cx + cy, r * r);
}

void procedural_eye_potion_float (in float2 uv, in float k, in float c, in float s, in float3 colorA, in float3 colorB,
    in float3 colorC, in float size, in float time, in bool predicate, out float4 Out)
{
    uv = (predicate) ? uv_to_pixel(uv, 64) : uv;
    uv -= 0.5;    
    uv *= lerp(2.0, 1.0, size);
    
    time *= 0.35;
    float a = (cos(time * PI * 3) * k/2 - 0.1);

    float bottle_body_bottom = body_bottom(uv, c, 0.3, predicate);
    bottle_body_bottom *= step(uv.y, 0.1);

    float bottle_body_top = body_top(uv, c, 0.3, predicate);
    bottle_body_top *= 1 - step(uv.y, 0.1);

    bottle_body_bottom += bottle_body_top;

    float bottle_top = top(uv, 0.15, predicate);
    bottle_top *= 1- bottle_body_bottom;
    bottle_top *= step(uv.y, 0.35);
    
    bottle_body_bottom += bottle_top;

    float bottle_body_bottom_inside = body_bottom(uv, c, 0.25, predicate);
    bottle_body_bottom_inside *= step(uv.y, 0.08);
    float bottle_body_top_inside = body_top(uv + float2(0, 0.02), c, 0.25, predicate);
    bottle_body_top_inside *= 1 - step(uv.y, 0.08);
    bottle_body_bottom_inside += bottle_body_top_inside;
    
    
    float bottle_cap = cap(uv, 0.05, predicate);  
    float bottle_cap_inside = cap_inside(uv, 0.1, predicate);
    bottle_cap_inside *= step(uv.y, 0.35);
    
    float bottle_liquid_front = liquid(uv, time, k, predicate);
    bottle_liquid_front *= bottle_body_bottom_inside;

    float bottle_liquid_back = liquid(uv, -time, k, predicate);
    bottle_liquid_back *= bottle_body_bottom_inside;

    bottle_body_bottom_inside = saturate(bottle_body_bottom_inside + 0.5);

    float bottle_eye = eye(uv, a, predicate);
    bottle_eye -= bottle_liquid_back * 0.5;
    bottle_eye = saturate(bottle_eye);

    float bottle_iris = iris(uv, time, a, 0.07, predicate);
    bottle_iris -= bottle_liquid_back * 0.5;
    bottle_iris = saturate(bottle_iris);

    float bottle_pupil = iris(uv, time, a, 0.05 * s, predicate);
    bottle_pupil -= bottle_liquid_back * 0.5;
    bottle_pupil = saturate(bottle_pupil);

    float bottle_shine = shine(uv, c, 0.035, predicate);

    float3 bottle_body_color = colorA * bottle_body_bottom;
    float3 bottle_cap_color = colorB * bottle_cap;    
    
    float4 shape = 0;
    shape.rgb += bottle_body_color;
    shape.rgb += bottle_cap_color;
    shape.rgb = lerp(shape.rgb, colorB * 0.5, bottle_cap_inside);
    shape.rgb = lerp(shape.rgb, colorC, bottle_liquid_front);
    shape.rgb = lerp(shape.rgb, colorC * 0.5, bottle_liquid_back);
    shape.rgb += bottle_eye;
    shape.rgb = lerp(shape.rgb, colorC, bottle_iris);
    shape.rgb = lerp(shape.rgb, 0, bottle_pupil);
    shape.rgb *= bottle_body_bottom_inside;
    shape.rgb += bottle_shine;
    shape.a += bottle_body_bottom;
    shape.a += bottle_cap;
    
    Out = shape;
}