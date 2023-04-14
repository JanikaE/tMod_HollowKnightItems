sampler uImage0 : register(s0);
float uThreshold;

float4 Normal(float2 coords : TEXCOORD0) : COLOR0{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;
        
    if (color.a < uThreshold)
        return float4(0, 0, 0, 1);
    else
        return float4(1, 1, 1, 1);
}

technique Technique1 {
    pass Normal {
        PixelShader = compile ps_2_0 Normal();
    }
}