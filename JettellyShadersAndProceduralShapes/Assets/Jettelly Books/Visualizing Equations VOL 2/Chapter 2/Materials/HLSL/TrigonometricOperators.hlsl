// 180° or a semicircle.
#define PI 3.14159265358
// 360° or a full circle.
#define TWO_PI 6.28318530718

float sinusoidal (float x, float a, float b, float c)
{
    // Clamp the sine wave to a range between 0.0 and 1.0
    const float u = 0.5;
    // Return the sine wave with amplitude, frequency and phase.
    return a * sin((x + c) * b) * u + u;
}

float sine_wave (float2 uv, float a, float b, float c)
{
    // Get the Y coordinate.
    float fx = uv.y;
    // Get the X coordinate.
    float x = uv.x;
    // Set the sine wave with amplitude, frequency and phase.
    float f = sinusoidal(x, a, b, c);
    fx-= f;
    // if fx is greater than 0.0, return black, else return white.
    return (fx > 0) ? 0.0 : 1.0;
}

float tan_wave (float2 uv, float a, float b, float c)
{
    // Get the Y coordinate.
    float fx = uv.y;
    // Get the X coordinate.
    float x = uv.x;
    // Set the pivot X position to 0.5.
    const float px = 0.5;
    // Set the pivot Y position as a wave in the center of the Quad.
    float py = sinusoidal(px, a, b, c);
    // Set the wave slope.
    float m = a * tan(PI / 4.0 * cos((px + c) * b)) * (b / 2.0);
    // Set the function mx + b.
    float f = m * (x - px) + py;
    fx -= f;
    // if fx is greater than 0.0, return white, else return black.
    return (fx > 0) ? 1.0 : 0.0;
}

float normalized_time (float s)
{
    // Return the fractional part of time to generate a loop.
    // Get the time in seconds.
    return frac(_Time.y / s);
}

void trigonometric_operators_float (in float2 uv, in float a, in float b, in float c, out float Sin, out float Tan, out float Py)
{
    // Add normalized time.
    // Multiply it by a full circle, and divide by the frequency to get a perfect loop.
    float t = normalized_time(c) * (TWO_PI / b);    
    
    Sin = sine_wave(uv, a, b, t);
    Tan = tan_wave(uv, a, b, t);
    Py = sinusoidal(0.5, a, b, t);
}