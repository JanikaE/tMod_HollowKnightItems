sampler uImage0 : register(s0);
float2 uImageSize;

float4 Outline(float2 coords : TEXCOORD0) : COLOR0{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;

    // ��ȡÿ�����ص���ȷ��С
    float dx = 1 / uImageSize.x;
    float dy = 1 / uImageSize.y;
    bool flag = false;
    // ����Χ8��������ж������˱���᲻ͨ��
    for (int i = -1; i <= 1; i++) {
        for (int j = -1; j <= 1; j++) {
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
    pass Outline {
        PixelShader = compile ps_2_0 Outline();
    }
}