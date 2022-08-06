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

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //float2 framedCoords = (coords * uImageSize0 - uSourceRect.xy) / uSourceRect.zw;
    
    float4 color = tex2D(uImage0, coords);
    float2 noiseCoords = (coords * uImageSize0 - uSourceRect.xy) / uImageSize1;
    float4 noise = tex2D(uImage1, float2(0.1 * noiseCoords.x, 0.1 *noiseCoords.y));
    float luminosity = (color.r + color.g + color.b) / 3;
    
    float4 tempColor = (255, 255, 255, 1);
    color.rgb = luminosity * (uColor * noise.rgb + uSecondaryColor * (tempColor.rbg - noise.rgb) );
    return color * sampleColor * color.a;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}