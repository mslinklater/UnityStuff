// Using "lineal" as name because "linear" is a reserved word in HLSL.
float lineal (half2 uv, half m, half b, half u)
{
    // Y coordinate.
    half fx = uv.y;
    // X coordinate.
    half x = uv.x;
    // The linear function mx + b extended.
    half f = m * (x - u) + b;
    fx -= f;
    return fx;
}

void linear_function_half(in half2 uv, in half m, in half b, in half u, out half Out)
{    
    half fx = lineal(uv, m, b, u);
    // if fx is greater than 0.0, return white, else return black.        
    Out = (fx > 0.0) ? 1.0 : 0.0;
}