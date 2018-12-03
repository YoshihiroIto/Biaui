float Value : register(C0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float x = uv.x;

    float3 color;

#if 0
    if (x < 1.0/6)
    {
        color.r = 1;
        color.g = lerp(0, 1, x * 6);
        color.b = 0;
    }
    else if (x < 2.0/6)
    {
        color.r = lerp(1, 0, (x - 1.0/6) * 6);
        color.g = 1;
        color.b = 0;
    }
    else if (x < 3.0/6)
    {
        color.r = 0;
        color.g = 1;
        color.b = lerp(0, 1, (x - 2.0/6) * 6);
    }
    else if (x < 4.0/6)
    {
        color.r = 0;
        color.g = lerp(1, 0, (x - 3.0/6) * 6);
        color.b = 1;
    }
    else if (x < 5.0/6)
    {
        color.r = lerp(0, 1, (x - 4.0/6) * 6);
        color.g = 0;
        color.b = 1;
    }
    else
    {
        color.r = 1;
        color.g = 0;
        color.b = lerp(1, 0, (x - 5.0/6) * 6);
    }
#else
    if (x < 3.0/6)
    {
        if (x < 1.0/6)
        {
            color.r = 1;
            color.g = lerp(0, 1, x * 6);
            color.b = 0;
        }
        else if (x < 2.0/6)
        {
            color.r = lerp(1, 0, (x - 1.0/6) * 6);
            color.g = 1;
            color.b = 0;
        }
        else
        {
            color.r = 0;
            color.g = 1;
            color.b = lerp(0, 1, (x - 2.0/6) * 6);
        }
    }
    else
    {
        if (x < 4.0/6)
        {
            color.r = 0;
            color.g = lerp(1, 0, (x - 3.0/6) * 6);
            color.b = 1;
        }
        else if (x < 5.0/6)
        {
            color.r = lerp(0, 1, (x - 4.0/6) * 6);
            color.g = 0;
            color.b = 1;
        }
        else
        {
            color.r = 1;
            color.g = 0;
            color.b = lerp(1, 0, (x - 5.0/6) * 6);
        }
    }
#endif

    float3 c = lerp(color, float3(1, 1, 1), uv.y);

    return float4(c * Value, 1);
}

