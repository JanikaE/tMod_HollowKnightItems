sampler uImage0 : register(s0);
float2 uImageSize;

float4 Text(float2 coords : TEXCOORD0) : COLOR0{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;

    // ��ȡÿ�����ص���ȷ��С
    float dx = 1 / uImageSize.x;
    float dy = 1 / uImageSize.y;
    bool flag = false;
    // ����Χ8��������ж������˱���᲻ͨ��
    for (int i = -2; i <= 2; i += 2) {
        for (int j = -2; j <= 2; j += 2) {
            float4 c = tex2D(uImage0, coords + float2(dx * i, dy * j));
            // ����κ�һ������û����ɫ
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