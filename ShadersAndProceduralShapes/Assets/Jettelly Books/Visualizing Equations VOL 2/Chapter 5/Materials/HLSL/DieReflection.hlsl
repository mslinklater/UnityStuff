void DieReflection_float(in float2 UV, in float3 TangentWS, in float3 BitangentWS, in float3 NormalWS, in float3 ViewDirection, in Texture2D<float4> Reflection, in Texture2D<float4> Factor, in SamplerState SS, in float Offset, out float Out)
{
    float tangent = dot(TangentWS, ViewDirection);
    float bitangent = dot(BitangentWS, ViewDirection);
    float normal = dot(NormalWS, ViewDirection);
    float3 TBN = normalize(float3(tangent, bitangent, normal));

    float2 uv = UV + (TBN * Offset).xy;
    
    float2 reflection_factor = Factor.SampleLevel(SS, uv, 1).rg;    
    reflection_factor = smoothstep(-reflection_factor, reflection_factor + 0.1, 0.2);
    float reflection = Reflection.SampleLevel(SS, (reflection_factor + uv - TBN).xy, 4).r;
    
    Out = reflection;
}