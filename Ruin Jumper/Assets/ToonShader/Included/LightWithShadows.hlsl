///#infdef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

void GetLightWithShadow_float(float3 worldPos, out float3 Direction, out float3 Color, out half DistanceAtten, out half ShadowAtten)
{
#if SHADERGRAPH_PREVIEW
    Direction = float3(0.5, 0.5, 0.5);
    Color = 1;
    DistanceAtten = 1;
    ShadowAtten = 1;
#else
#if SHADOWS_SCREEN
    half4 clipPos = TransformWorldToHClip(worldPos);
    half4 shadowCoord = ComputeScreenPos(clipPos);
#else
    half4 shadowCoord = TransformWorldToShadowCoord(worldPos);
#endif
    Light main = GetMainLight(shadowCoord);
    Direction = main.direction;
    Color = main.color;
    DistanceAtten = main.distanceAttenuation;
    ShadowAtten = main.shadowAttenuation;
#endif
}
///#endif