void ColorLerp_float(in float t, in float3 ColorA, in float3 ColorB, in float3 ColorC, out float3 RGB)
{       
    half3 color;
    
    if (t <= 0.5)
    {
        float t_scaled = t * 2.0;
        color = lerp(ColorA.rgb, ColorB.rgb, t_scaled);
    }
    else
    {
        float t_scaled = (t - 0.5) * 2.0;
        color = lerp(ColorB.rgb, ColorC.rgb, t_scaled);
    }

    
    RGB = color;
}