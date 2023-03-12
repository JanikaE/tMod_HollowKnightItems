sampler uImage0 : register(s0);
float4 uColorCenter;
float4 uColorEdge;

float Length(float2 Vector) {
    float x = Vector.x;
    float y = Vector.y;
    return sqrt(x * x + y * y);
}

float4 Fireball(float2 coords : TEXCOORD0) : COLOR0{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;


    float2 pos = float2(0.5, 0.5);
    float offset = Length(coords - pos);

    if (offset < 0.3)
        return uColorCenter;
    else if (offset < 0.4) {
        float off = (offset - 0.3) * 10;
        float r = uColorCenter.r + (uColorEdge.r - uColorCenter.r) * off;
        float g = uColorCenter.g + (uColorEdge.g - uColorCenter.g) * off;
        float b = uColorCenter.b + (uColorEdge.b - uColorCenter.b) * off;
        return float4(r, g, b, color.a);
    }
    else
        return float4(uColorEdge.rgb, (0.5 - offset) * 10);
}

technique Technique1 {
    pass Test {
        PixelShader = compile ps_2_0 Fireball();
    }
}