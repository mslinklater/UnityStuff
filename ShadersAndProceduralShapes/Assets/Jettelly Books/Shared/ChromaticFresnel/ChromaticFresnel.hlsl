void ChromaticFresnel_float(in float Power, in float Opacity, in float3 Aberration, in float3 NormalWS, in float3 ViewDirection, out float3 RGB)
{
    float fresnel = pow((1.0 - saturate(dot(normalize(NormalWS), normalize(ViewDirection)))), Power);
    half3 color = sin(fresnel * Aberration);
    color *= Opacity;
    RGB = color;
}