float quadratic (half2 uv, float a, float b, float c, float u)
{
    // Y coordinate.
    float fx = uv.y;
    // X coordinate.
    float x = uv.x;   
    float xu = x - u;
    // The quadratic function ax^2 + bx + c extended.
    float f = a * (xu * xu) + b * xu + c;
    fx -= f;
    return fx;
}

void quadratic_function_half(in half2 uv, in half a, in half b, in half c, in half u, out half3 Out)
{
    half fx = quadratic(uv, a, b, c, u);
    // Red color.
    const half3 red = half3(1, 0, 0);
    // Green color.
    const half3 green = half3(0, 1, 0);
    // if fx is greater than 0.0, return white, else return black.  
    Out = (fx > 0.0) ? red : green;
}