sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float2 uTargetPosition;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;


float BlendFunction(float from, float to)
{
    return (to <= 0.5) ? 2.22 * from * to : 1 - 2.22 * (1 - from) * (1 - to);
}
float3 BlendColor(float3 from, float3 to)
{
    return float3(BlendFunction(from.r, to.r), BlendFunction(from.g, to.g), BlendFunction(from.b, to.b));
}
float2 InverseLerp(float2 start, float2 end, float2 x)
{
    return saturate((x - start) / (end - start));
}

//Returns noise
float rand2dTo1d(float2 value, float2 dotDir = float2(12.9898, 78.233))
{
    float2 smallValue = sin(value);
    float random = dot(smallValue, dotDir);
    random = frac(sin(random) * 143758.5453);
    return random;
}

float2 rand2dTo2d(float2 value)
{
    return float2(
        rand2dTo1d(value, float2(12.989, 78.233)),
        rand2dTo1d(value, float2(39.346, 11.135))
    );
}



float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    
    float2 framedCoords = (coords * uImageSize0 - uSourceRect.xy) / uSourceRect.zw;
    float2 noiseCoords = (coords * uImageSize0 - uSourceRect.xy) / uImageSize1;
    
    
    float4 color = tex2D(uImage0, coords);
    float4 noiseColor = tex2D(uImage1, float2(noiseCoords.x, sin(3.141 * frac(noiseCoords.y + frac(uTime * 0.51)))) * 0.067);
    
    //Define a baseline water color.
    
    color.rgb = lerp(color.rgb, uColor, noiseColor.rgb);
    return color * color.a;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}