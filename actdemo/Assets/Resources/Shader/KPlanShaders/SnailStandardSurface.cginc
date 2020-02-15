#ifndef UNITY_STANDARD_SURFACE_INCLUDED
#define UNITY_STANDARD_SURFACE_INCLUDED

//-------------------------------------------------------------------------------------
#include "UnityPBSLighting.cginc"
#include "UnityStandardInput.cginc"

half3 _OffsetLightDir;

//-------------------------------------------------------------------------------------
// Shared PBS surface setup

// Define input struct unless it's already defined
#ifndef Input
	struct Input
	{
		float4	texcoord;

	#ifdef _PARALLAXMAP
		half3	viewDir;
	#endif
	};
#endif

void SnailStandardSurfaceVertex (inout appdata_full v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input, o);
	
	// Setup UVs to the format expected by Standard input functions.
	o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
	o.texcoord.zw = TRANSFORM_TEX(((_UVSec == 0) ? v.texcoord : v.texcoord1), _DetailAlbedoMap);
}


//-------------------------------------------------------------------------------------
// Metallic workflow

void SnailStandardSurface (Input IN, inout SurfaceOutputStandard o) {
#ifdef _PARALLAXMAP
	IN.texcoord = Parallax(IN.texcoord, IN.viewDir);
#endif
	
	o.Alpha = Alpha(IN.texcoord.xy);
#if defined(_ALPHATEST_ON)
	clip(o.Alpha - _Cutoff);
#endif

	o.Albedo = Albedo(IN.texcoord.xyzw);	

#ifdef _NORMALMAP
	o.Normal = NormalInTangentSpace(IN.texcoord.xyzw);
#endif

	half2 metallicGloss = MetallicGloss(IN.texcoord.xy);
	o.Metallic = metallicGloss.x;
	o.Smoothness = metallicGloss.y;

#ifdef _OCCLUSION_MAP
	o.Occlusion = Occlusion(IN.texcoord.xy);
#endif

#ifdef _EMISSION
	o.Emission = Emission(IN.texcoord.xy);
#endif
}

inline void SnailStandardSurfaceFinal (Input IN, SurfaceOutputStandard o, inout half4 color)
{
#if defined(_ALPHABLEND_ON) || defined(_ALPHAPREMULTIPLY_ON)
	color.a = Alpha(IN.texcoord.xy);
	color.rgb *= min(1, color.a * 3.0/*Make the color not too be darkness*/);
#else
	UNITY_OPAQUE_ALPHA(color.a);
#endif
}

inline void LightingSnailStandard_GI (
	SurfaceOutputStandard s,
	UnityGIInput data,
	inout UnityGI gi)
{
	UNITY_GI(gi, s, data);
}

inline half4 LightingSnailStandard (SurfaceOutputStandard s, half3 viewDir, UnityGI gi)
{
	s.Normal = normalize(s.Normal);

	half oneMinusReflectivity;
	half3 specColor;
	s.Albedo = DiffuseAndSpecularFromMetallic (s.Albedo, s.Metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

	// shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
	// this is necessary to handle transparency in physically correct way - only diffuse component gets affected by alpha
	half outputAlpha;
	s.Albedo = PreMultiplyAlpha (s.Albedo, s.Alpha, oneMinusReflectivity, /*out*/ outputAlpha);

	UnityLight light = gi.light;
	#ifdef _OFFSET_LIGHT_DIR
	half3 lightDir = light.dir;
	lightDir += _OffsetLightDir;
	light.dir = normalize(lightDir);
	#endif
	half4 c = UNITY_BRDF_PBS (s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, light, gi.indirect);
	c.rgb += UNITY_BRDF_GI (s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);
	c.a = outputAlpha;
	return c;
}

//-------------------------------------------------------------------------------------
// Specular workflow

void SnailStandardSurfaceFakeSpecular (Input IN, inout SurfaceOutputStandardSpecular o) {
#ifdef _PARALLAXMAP
	IN.texcoord = Parallax(IN.texcoord, IN.viewDir);
#endif
	
	o.Alpha = Alpha(IN.texcoord.xy);
#if defined(_ALPHATEST_ON)
	clip(o.Alpha - _Cutoff);
#endif

	o.Albedo = Albedo(IN.texcoord.xyzw);	

#ifdef _NORMALMAP
	o.Normal = NormalInTangentSpace(IN.texcoord.xyzw);
#endif

	half4 specGloss = half4(_SpecColor.rgb, _Glossiness);
	half4 specTexColor = tex2D(_SpecGlossMap, IN.texcoord.xy);
	specGloss *= specTexColor;
	o.Specular = specGloss.rgb;
	o.Smoothness = specGloss.a;

#ifdef _OCCLUSION_MAP
	o.Occlusion = Occlusion(IN.texcoord.xy);
#endif

#ifdef _EMISSION
	o.Emission = Emission(IN.texcoord.xy);
#endif
}

void SnailStandardSurfaceSpecular (Input IN, inout SurfaceOutputStandardSpecular o) {
#ifdef _PARALLAXMAP
	IN.texcoord = Parallax(IN.texcoord, IN.viewDir);
#endif
	
	o.Alpha = Alpha(IN.texcoord.xy);
#if defined(_ALPHATEST_ON)
	clip(o.Alpha - _Cutoff);
#endif

	o.Albedo = Albedo(IN.texcoord.xyzw);	

#ifdef _NORMALMAP
	o.Normal = NormalInTangentSpace(IN.texcoord.xyzw);
#endif

	half4 specGloss = SpecularGloss(IN.texcoord.xy);
	o.Specular = specGloss.rgb;
	o.Smoothness = specGloss.a;

#ifdef _OCCLUSION_MAP
	o.Occlusion = Occlusion(IN.texcoord.xy);
#endif

#ifdef _EMISSION
	o.Emission = Emission(IN.texcoord.xy);
#endif
}

inline void SnailStandardSurfaceSpecularFinal (Input IN, SurfaceOutputStandardSpecular o, inout half4 color)
{	
#if defined(_ALPHABLEND_ON) || defined(_ALPHAPREMULTIPLY_ON)
	color.a = o.Alpha;
	color.rgb *= min(1, color.a * 3.0/*Make the color not too be darkness*/);
#else
	UNITY_OPAQUE_ALPHA(color.a);
#endif
}

inline void LightingSnailStandardSpecular_GI (
	SurfaceOutputStandardSpecular s,
	UnityGIInput data,
	inout UnityGI gi)
{
	UNITY_GI(gi, s, data);
}

inline half4 LightingSnailStandardSpecular (SurfaceOutputStandardSpecular s, half3 viewDir, UnityGI gi)
{
	s.Normal = normalize(s.Normal);

	// energy conservation
	half oneMinusReflectivity;
	s.Albedo = EnergyConservationBetweenDiffuseAndSpecular (s.Albedo, s.Specular, /*out*/ oneMinusReflectivity);

	// shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
	// this is necessary to handle transparency in physically correct way - only diffuse component gets affected by alpha
	half outputAlpha;
	s.Albedo = PreMultiplyAlpha (s.Albedo, s.Alpha, oneMinusReflectivity, /*out*/ outputAlpha);

	UnityLight light = gi.light;
	#ifdef _OFFSET_LIGHT_DIR
	half3 lightDir = light.dir;
	lightDir += _OffsetLightDir;
	light.dir = normalize(lightDir);
	#endif
	half4 c = UNITY_BRDF_PBS (s.Albedo, s.Specular, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, light, gi.indirect);
	c.rgb += UNITY_BRDF_GI (s.Albedo, s.Specular, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);
	c.a = outputAlpha;
	return c;
}

#endif // UNITY_STANDARD_SURFACE_INCLUDED
