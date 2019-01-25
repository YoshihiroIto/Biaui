float Value : register(C0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float h = uv.x;
    float s = uv.y;

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

    float3 c = lerp(color, float3(1, 1, 1), s);

    return float4(c * Value, 1);
}

