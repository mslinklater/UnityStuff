#define RADIUS 0.03

void algebraic_circle_half(in half2 uv, in float u, in float b, out float Out)
{
    const half r = RADIUS * RADIUS;
    // X coordinate.
    half x = uv.x - u;
    // Y coordinate.
    half y = uv.y - b;
    // Circle equation.
    half circle = x * x + y * y;    // pow(x, 2) + pow(y, 2);
    // circle color.    
    Out = (circle <= r) ? 0.0 : 1.0;  
}