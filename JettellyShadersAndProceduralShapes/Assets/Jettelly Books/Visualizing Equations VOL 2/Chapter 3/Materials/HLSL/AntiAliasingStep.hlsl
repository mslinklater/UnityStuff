float3 anti_aliasing_step (float3 gradient, float edge)
{
    // Determine how much a value changes over a pixel.
    float3 rate_of_change = max(abs(ddx(gradient)), abs(ddy(gradient)));
    // Determine the edges.
    float3 lower_edge = edge - 0.5 * rate_of_change;
    float3 upper_edge = edge + 0.5 * rate_of_change;
    // Perform an inverse linear interpolation.
    float3 stepped = (gradient - lower_edge) / (upper_edge - lower_edge);
    stepped = saturate(stepped);
    
    return stepped;
}

float2 uv_to_pixel (float2 uv, float pixelDensity)
{    
    return floor(uv * pixelDensity) / pixelDensity;
}