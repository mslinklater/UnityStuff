#define PI 3.14159265358

#include "Assets/Jettelly Books/Visualizing Equations VOL 2/Chapter 1/Materials/HLSL/AlgebraicCircle.hlsl"

float linear_function (float2 uv, float a)
{
    // Y coordinate.
    float fx = uv.y;
    // X coordinate.
    float x = uv.x;
    // Rotate the X coordinate.
    float m = tan(a);
    float f = m * x;
    fx -= f;
    // Make fx negatives when it goes over 90 degrees.
    float h = (a > PI / 2) ? -1 : 1;
    // if fx is greater than 0.0, return white, else return black.   
    return (h * fx > 0.0) ? 0.5 : 0.0;
}

float to_degree (float a)
{
    // From radians to degrees.
    return a * (PI / 180.0);
}

void point_reflection_float (in float2 uv, in float2 p, in float a, out float3 Out)
{
    // Center the coordinates.
    uv -= 0.5;
    // Transform from radians to degrees.
    a = to_degree(a);
    // Green color.
    const float3 color_g = float3(0, 1, 0);
    // Yellow color.
    const float3 color_y = float3(1, 1, 0);
    // Magenta color.
    const float3 color_m = float3(1, 0, 1);
    // Visualize the line.
    float3 l = linear_function(uv, a) * color_g;
    // Calculate the normal of the line.
    float2 n = float2(-sin(a), cos(a));
    // Calculate the rotated point.
    float2 pr = p - 2 * n * (-sin(a) * p.x + cos(a) * p.y);
    // Initialize p0.
    float p0 = 0;
    // Initialize p1.
    float p1 = 0;
    // Visualize the point p0 as a circle.
    algebraic_circle_half(uv, p.x, p.y,p0);
    // Visualize the reflected point p1 as a circle.
    algebraic_circle_half(uv, pr.x, pr.y,p1);
    // Draw a yellow circle.
    float3 c = (1 - float3(p0.xxx)) * color_y;
    // Draw a magenta circle.
    float3 cr = (1 - float3(p1.xxx)) * color_m;
    // Create the render.
    float3 render = l + c + cr;
    Out = render;
}