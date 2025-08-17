
void TosRGB_float(in float4 Linear, out float3 RGB, out float A)
{
    Linear = saturate(Linear);
    bool3 cutoff = (Linear.rgb < float3(0.0031308, 0.0031308, 0.0031308));
    float a = 1.0/2.4;
    float3 higher = float3(1.055, 1.055, 1.055) * pow(Linear.rgb, float3(a, a, a)) - float3(0.055, 0.055, 0.055);
    float3 lower = Linear.rgb * float3(12.92, 12.92, 12.92);

    float4 render = float4(lerp(higher, lower, cutoff), Linear.a);
    RGB = render.rgb;
    A = render.a;
}