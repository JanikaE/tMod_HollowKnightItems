sampler uImage0 : register(s0);

float4 Text(float2 coords : TEXCOORD0) : COLOR0{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;

    if (color.r > 0.3)
        return float4(255, 255, 255, 1);
    else
        return float4(0, 0, 0, 1);
}

technique Technique1 {
    pass Test {
        PixelShader = compile ps_2_0 Text();
    }
}