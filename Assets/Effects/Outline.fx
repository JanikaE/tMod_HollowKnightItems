sampler uImage0 : register(s0);
float2 uImageSize;

float4 Outline(float2 coords : TEXCOORD0) : COLOR0{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;

    // 获取每个像素的正确大小
    float dx = 1 / uImageSize.x;
    float dy = 1 / uImageSize.y;
    bool flag = false;
    // 对周围8个点进行判定，多了编译会不通过
    for (int i = -1; i <= 1; i++) {
        for (int j = -1; j <= 1; j++) {
            float4 c = tex2D(uImage0, coords + float2(dx * i, dy * j));
            // 如果任何一个像素没有颜色
            if (!any(c)) {
                flag = true;
            }
        }
    }
    if (flag)
        return float4(0, 0, 0, 1);
    else
        return float4(1, 1, 1, 1);
}

technique Technique1 {
    pass Outline {
        PixelShader = compile ps_2_0 Outline();
    }
}