#ifndef __SNAIL_XRAY__
#define __SNAIL_XRAY__

#include "UnityPBSLighting.cginc"

sampler2D _MainTex;

struct Input {
	float2 uv_MainTex;
	float xrayFactor;
};

fixed4 _Color;

void SnailXRayVert (inout appdata_full v, out Input data) 
{
	UNITY_INITIALIZE_OUTPUT(Input, data);

	float3 viewDir = ObjSpaceViewDir(v.vertex);
	viewDir = normalize(viewDir);
	float3 normal = normalize(v.normal);
	float xrayFactor = dot(normal, viewDir);
	xrayFactor = 1 - max(xrayFactor, 0) + 0.15;
	data.xrayFactor = xrayFactor;
}

void SnailXRaySurf (Input IN, inout SurfaceOutputStandard o) 
{
	fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb * IN.xrayFactor;
	o.Alpha = IN.xrayFactor;
}

inline void LightingSnailXRay_GI (
	SurfaceOutputStandard s,
	UnityGIInput data,
	inout UnityGI gi)
{
}

inline half4 LightingSnailXRay (SurfaceOutputStandard s, half3 viewDir, UnityGI gi)
{
	return half4(s.Albedo, s.Alpha);
}

#endif // __SNAIL_XRAY__
