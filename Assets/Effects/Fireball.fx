sampler uImage0 : register(s0);

float Length(float2 Vector) {
    float x = Vector.x;
    float y = Vector.y;
    return sqrt(x * x + y * y);
}

float4 color1 = float4(1, 0.61, 0.62, 1);
float4 color2 = float4(1, 0.34, 0.37, 1);

float4 Fireball(float2 coords : TEXCOORD0) : COLOR0{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;

    float2 pos = float2(0.5, 0.5);
    float offset = Length(coords - pos);

    if (offset < 0.3)
        return color1;
    else if (offset < 0.4) {
        float off = (offset - 0.3) * 10;
        float r = color1.r + (color2.r - color1.r) * off;
        float g = color1.g + (color2.g - color1.g) * off;
        float b = color1.b + (color2.b - color1.b) * off;
        return float4(r, g, b, color.a);
    }
    else
        return float4(color2.rgb, (0.5 - offset) * 10);
}

technique Technique1 {
    pass Test {
        PixelShader = compile ps_2_0 Fireball();
    }
}