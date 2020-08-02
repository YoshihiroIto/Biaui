float Value : register(C0);
float AspectRatioCorrectionX : register(C1);
float AspectRatioCorrectionY : register(C2);
float3 BorderColor : register(C3);
float IsEnabled : register(C6);
float3 DisableColor : register(C7);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float2 d = float2(
            (uv.x - 0.5) * AspectRatioCorrectionX,
            (uv.y - 0.5) * AspectRatioCorrectionY);
    float i = dot(d, d);

    if (i >= 0.5*0.5)
        return float4(0,0,0,0);

	if (IsEnabled != 0.0f)
	{
		float h = (atan2(-d.y, -d.x) + 3.14159265359) / (2.0*3.14159265359);
		float s10 = sqrt(i) * 2;

        // https://qiita.com/keim_at_si/items/c2d1afd6443f3040e900
        float3 color = ((clamp(abs(frac(h + float3(0, 2, 1) / 3) * 6 - 3) - 1, 0, 1) - 1) * s10 + 1) * Value;

		float r = min(1, (0.5 - sqrt(i)) * 100 * 2);
		color = lerp(BorderColor, color, r);

		return float4(color, 1);
	}
	else
	{
		return float4(DisableColor, 1);
	}
}

