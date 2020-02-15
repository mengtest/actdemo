Shader "SnailStandardXRay" {
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
	
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic", 2D) = "white" {}

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}
	
		_Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02
		_ParallaxMap ("Height Map", 2D) = "black" {}
	
		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}
	
		_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}
	
		_DetailMask("Detail Mask", 2D) = "white" {}
	
		_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
		_DetailNormalMapScale("Scale", Float) = 1.0
		_DetailNormalMap("Normal Map", 2D) = "bump" {}

		[Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0

		_OffsetLightDirEnabled("Offset Light Dir Enabled", Float) = 0.0
		_OffsetLightDir("Offset Light Dir", Vector) = (0, 0, 0, 0)

		// Blending state
		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
		[HideInInspector] _CullMode ("__cullmode", Float) = 2.0
	}

	SubShader {
		Tags { "Queue"="AlphaTest+1" }
		Blend One One
		ZWrite Off
		ZTest Greater
		Cull Back

		CGPROGRAM
		#pragma surface SnailXRaySurf SnailXRay keepalpha vertex:SnailXRayVert exclude_path:deferred exclude_path:prepass noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap noforwardadd 
		#pragma only_renderers d3d9 d3d11 d3d11_9x opengl gles gles3 metal

		#include "SnailXRay.cginc"

		ENDCG

		Tags { "Queue"="AlphaTest+2" }
		Blend [_SrcBlend] [_DstBlend]
		ZWrite [_ZWrite]
		ZTest LEqual
		Cull [_CullMode]

		CGPROGRAM
			#pragma surface SnailStandardSurface SnailStandard fullforwardshadows addshadow keepalpha vertex:SnailStandardSurfaceVertex exclude_path:prepass exclude_path:deferred noforwardadd nodynlightmap nodirlightmap
			#pragma only_renderers d3d9 d3d11 d3d11_9x opengl gles gles3 metal
			#pragma target 3.0

			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION
			#pragma shader_feature _METALLICGLOSSMAP 
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _PARALLAXMAP
			#pragma shader_feature _OCCLUSION_MAP
			#pragma shader_feature _OFFSET_LIGHT_DIR

			#include "SnailStandardSurface.cginc"

		ENDCG
	
	
	}
	CustomEditor "SnailStandardShaderGUI"
	FallBack "VertexLit"
}
