#include "Assets/Jettelly Books/Visualizing Equations VOL 2/Chapter 3/Materials/HLSL/AntiAliasingStep.hlsl"

void pentagon_sd_float(in float2 uv, in float r, out float4 Out)
{    
    const float3 k = float3(0.809016994, 0.587785252, 0.726542528);
    uv -= 0.5;
    uv.y = -uv.y;
    uv.x = abs(uv.x);
    uv -= 2.0 * min(dot(float2(-k.x,k.y),uv),0.0) * float2(-k.x,k.y);
    uv -= 2.0 * min(dot(float2( k.x,k.y),uv),0.0) * float2( k.x,k.y);
    uv -= float2(clamp(uv.x,-r * k.z, r * k.z),r);
    
    Out = length(uv) * sign(uv.y);
}