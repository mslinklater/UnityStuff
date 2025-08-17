float sphere_sd(float3 p, float r)
{    
    return length(p) - r;
}

float capsule_sd(float3 p, float3 p0, float3 p1, float r)
{   
    float h0 = dot(p - p0, p1 - p0) / dot(p1 - p0, p1 - p0);    
    float3 h1 = clamp(h0, 0.0, 1.0);
    float3 v = p0 + h1 * (p1 - p0); 
    return sqrt(dot(p - v, p - v)) - r;
}

float smooth_min (float a, float b, float k)
{
    return 0.5 * ((a + b) - sqrt(pow(a - b, 2.0) + k));
}