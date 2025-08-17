// Source: https://www.desmos.com/calculator/4jhmzqeflq?lang=es

// it refers to the equation: 
float projection(float2 a, float2 b, float2 c)
{
    float2 cb = c - b;
    float2 ab = a - b;

    return dot(ab, cb) / dot(cb, cb);
}

void segment_sd_float(in float2 uv, in float2 p0, in float2 p1, out float4 Out)
{   
    float h = projection(uv, p0, p1);
    h = clamp(h, 0, 1);    
    float2 p2 = p0 + h * (p1 - p0);
    
    Out = distance(p2, uv);
}