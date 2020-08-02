float Value : register(C0);
float IsEnabled : register(C6);
float3 DisableColor : register(C7);

float4 main(float2 uv : TEXCOORD) : COLOR
{
	if (IsEnabled != 0.0f)
	{
		float h = uv.x;
		float s = 1 - uv.y;

        // https://qiita.com/keim_at_si/items/c2d1afd6443f3040e900
        float3 color = ((clamp(abs(frac(h + float3(0, 2, 1) / 3) * 6 - 3) - 1, 0, 1) - 1) * s + 1) * Value;

		return float4(color, 1);
	}
	else
	{
		return float4(DisableColor, 1);
	}
}

