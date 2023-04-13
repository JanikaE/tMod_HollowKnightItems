sampler uImage0 : register(s0);
float2 uImageSize;

float4 Text(float2 coords : TEXCOORD0) : COLOR0{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;

    // 获取每个像素的正确大小
    float dx = 1 / uImageSize.x;
    float dy = 1 / uImageSize.y;
    bool flag = false;
    // 对周围8个点进行判定，多了编译会不通过
    for (int i = -2; i <= 2; i += 2) {
        for (int j = -2; j <= 2; j += 2) {
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
    pass Test {
        PixelShader = compile ps_2_0 Text();
    }
}