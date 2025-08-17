#include "Assets/Jettelly Books/Visualizing Equations VOL 2/Chapter 1/Materials/HLSL/QuadraticFunction.hlsl"
#include "Assets/Jettelly Books/Visualizing Equations VOL 2/Chapter 3/Materials/HLSL/AntiAliasingStep.hlsl"

// Source: https://www.desmos.com/calculator/h6nqpguydl

// it refers to the equation: min(u - |x|, 0)^w + (y)^w < r^w
float dialog_box(float2 uv, float u, float w, float m)
{         
    float ax = pow(min(u - abs(uv.x), m), w);
    float ay = pow(uv.y, w);    
    return length(ax + ay);
}

// It refers to the equation: x^2 + y^2 < (r * c)^2
half dialog_middle_circle(in half2 uv)
{      
    half ax = uv.x * uv.x;
    half ay = uv.y *uv.y;    
    return length(ax + ay);
}

// It refers to the equation: min(-r, |x| - (r + b) * r) < y < -r {u > x > -u}
half dialog_arrow(in half2 uv, in half b, in half r)
{   
    half fx = min(-r, abs(uv.x) - (r + b) * r);
    half mx = saturate((abs(uv.x) - r) *- 1.0);
    half my = (uv.y - r) *- 1.0;
    
    uv.y -= fx;
    uv.y *= mx;
    uv.y *= my;
    
    return uv.y;
}

void procedural_dialog_box_float (in float2 uv, in float u, in float r, in float c, in float b, in int w,
    in float size, in float3 colorA, in float3 colorB, in float3 colorC, in bool predicate, out float4 Out)
{
    // Transform the UV coordinates to Pixel Art.       
    uv = (predicate) ? uv_to_pixel(uv, 64) : uv;
    // Center the procedural object.
    uv -= 0.5;
    // Increase or decrease its size.
    uv *= lerp(2.0, 1.0, size);   
    
    float box = dialog_box(uv, u, w, 0.0);  
    float box_radius = pow(r, w);                
    box = (predicate) ? 1 - step(box_radius, box) : 1.0 - anti_aliasing_step(box, box_radius);        

    float circles = dialog_box(uv, u, 2, 0.5);
    float circles_radius = pow(r * c, 2);
    circles = (predicate) ? step(circles_radius, circles) : anti_aliasing_step(circles, circles_radius);      
    
    float middle_circle = dialog_middle_circle(uv);
    float middle_circle_radius = pow(r * c, 2);
    middle_circle = (predicate) ? step(middle_circle_radius,middle_circle) : anti_aliasing_step(middle_circle, middle_circle_radius);   

    float arrow = dialog_arrow(uv, b, r);
    arrow = (predicate) ? step(0.001, arrow) : anti_aliasing_step(arrow, 0.001);

    float shine = quadratic(uv,-0.5, 0.3, 0.0, 0.0);
    shine = (predicate) ? step(shine, 0) : 1.0 - anti_aliasing_step(shine, 0);   
    
    float body = saturate(box + arrow);
    float body_complete = (body * circles) * middle_circle;
    body_complete = saturate(body_complete);
    shine *= body_complete;
    
    float4 shape = 0;
    shape.rgb = lerp(colorB, colorA, body_complete);
    shape.rgb = lerp(shape.rgb, colorC, shine);
    shape.a = body;
    
    Out = shape;
}