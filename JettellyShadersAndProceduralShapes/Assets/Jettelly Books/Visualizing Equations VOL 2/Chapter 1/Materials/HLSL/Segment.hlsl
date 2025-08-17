half tangent(half2 a, half2 b)
{
    return (b.y - a.y) / (b.x - a.x);
}

void segment_half(in half2 uv, in half2 p0, in half2 p1, out half Out)
{
    // Y coordinate.
    half fx = uv.y;
    // X coordinate.
    half x = uv.x;
    // The tangent.
    half m = tangent(p0, p1);
    // Linear Function in the form mx + b.
    half f = m * (x - p0.x) + p0.y;
    fx -= f;
    // if fx is greater than 0.0, return white, else return black.
    Out = (fx > 0.0) ? 1.0 : 0.0;
}