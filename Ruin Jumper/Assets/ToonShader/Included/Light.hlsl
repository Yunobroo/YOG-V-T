///#infdef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

void GetLight_float(float3 worldPos, out float3 Direction, out float3 Color)
{
#if SHADERGRAPH_PREVIEW
    Direction = float3(0.5,0.5,0.5);
    Color = 1;
#else
    Light main = GetMainLight(0);
    Direction = main.direction;
    Color = main.color;
#endif
}

///#endif