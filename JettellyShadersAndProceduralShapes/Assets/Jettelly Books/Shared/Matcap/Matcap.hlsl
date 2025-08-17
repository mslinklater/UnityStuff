void Matcap_float(in Texture2D<float4> MainTex, in SamplerState SS, in float3 NormalVS, in float Opacity, out float3 RGB)
{
    const half c = 0.5;    
    float2 uv = ((NormalVS * c) + c).xy;
    float3 texture_RGB = SAMPLE_TEXTURE2D(MainTex, SS, uv).rgb;
    texture_RGB.rgb *= Opacity;
    RGB = texture_RGB;
}
