float Value : register(C0);
float AspectRatioCorrectionX : register(C1);
float AspectRatioCorrectionY : register(C2);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float2 d = float2(
            (uv.x - 0.5) * AspectRatioCorrectionX,
            (uv.y - 0.5) * AspectRatioCorrectionY);
    float i = dot(d, d);

    if (i >= 0.5*0.5)
        return float4(0,0,0,0);

    float h = (atan2(-d.y, -d.x) + 3.14159265359) / (2.0*3.14159265359);
    float s10 = sqrt(i) * 2;

    float3 color;


#if 0
    if (h < 1.0/6)
    {
        color.r = 1;
        color.g = lerp(0, 1, h * 6);
        color.b = 0;
    }
    else if (h < 2.0/6)
    {
        color.r = lerp(1, 0, (h - 1.0/6) * 6);
        color.g = 1;
        color.b = 0;
    }
    else if (h < 3.0/6)
    {
        color.r = 0;
        color.g = 1;
        color.b = lerp(0, 1, (h - 2.0/6) * 6);
    }
    else if (h < 4.0/6)
    {
        color.r = 0;
        color.g = lerp(1, 0, (h - 3.0/6) * 6);
        color.b = 1;
    }
    else if (h < 5.0/6)
    {
        color.r = lerp(0, 1, (h - 4.0/6) * 6);
        color.g = 0;
        color.b = 1;
    }
    else
    {
        color.r = 1;
        color.g = 0;
        color.b = lerp(1, 0, (h - 5.0/6) * 6);
    }
#else
    if (h < 3.0/6)
    {
        if (h < 1.0/6)
        {
            color.r = 1;
            color.g = lerp(0, 1, h * 6);
            color.b = 0;
        }
        else if (h < 2.0/6)
        {
            color.r = lerp(1, 0, (h - 1.0/6) * 6);
            color.g = 1;
            color.b = 0;
        }
        else
        {
            color.r = 0;
            color.g = 1;
            color.b = lerp(0, 1, (h - 2.0/6) * 6);
        }
    }
    else
    {
        if (h < 4.0/6)
        {
            color.r = 0;
            color.g = lerp(1, 0, (h - 3.0/6) * 6);
            color.b = 1;
        }
        else if (h < 5.0/6)
        {
            color.r = lerp(0, 1, (h - 4.0/6) * 6);
            color.g = 0;
            color.b = 1;
        }
        else
        {
            color.r = 1;
            color.g = 0;
            color.b = lerp(1, 0, (h - 5.0/6) * 6);
        }
    }
#endif

    float3 c = lerp(float3(1, 1, 1), color, s10);

    return float4(c * Value, 1);
}

