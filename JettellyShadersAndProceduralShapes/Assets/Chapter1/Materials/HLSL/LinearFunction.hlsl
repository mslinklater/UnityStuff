void linear_function_half(in half2 uv, in half m, in half b, in half u, out half Out)
{
    half fx = uv.y;
    half x = uv.x;

    half f = m * (x - u) + b;
    fx -= f;

    Out = (fx > 0.0) ? 1.0 : 0.0;
}