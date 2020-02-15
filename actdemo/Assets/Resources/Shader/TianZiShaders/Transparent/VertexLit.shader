// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*******************************************************************************************
 * 功能 ： 漫反射
 * 注： 支持点光、平行光、聚光、补光、高度距离雾、阴影
 ********************************************************************************************/
Shader "Snail/Transparent/VertexLit"
{
	Properties {
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	}
	
	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Alphatest Greater 0
		Blend SrcAlpha OneMinusSrcAlpha  
		ZWrite On
		LOD 300
 
		Pass {	
			Name "ShadowPass"
			Tags {"LightMode" = "ForwardBase"}
			Cull Back	
			CGPROGRAM 
			// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members lightDir)
			#pragma exclude_renderers d3d11 xbox360
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
 
			struct v2f { 
				half2 uv_MainTex : TEXCOORD0;
				half4 pos : SV_POSITION;
				half2 lmap : TEXCOORD1;
				half4 color;
			};
 
			half4 _MainTex_ST;
			sampler2D _MainTex;

			v2f vert (appdata_full v)
			{
				v2f o;
				o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.pos = UnityObjectToClipPos (v.vertex);
				o.color = v.color;
				return o;
			}
 
			half4 frag (v2f i) : COLOR
			{
				half4 c = tex2D(_MainTex, i.uv_MainTex) * i.color;
			
				return c;
			}
			ENDCG
		}
	}
 
}







































 /**
Shader "Snail/Transparent/Cutout/Diffuse" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 200
	Cull Off
 

	
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardBase" }
		ColorMask RGB

		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f_surf members fog)
#pragma exclude_renderers d3d11 xbox360
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma multi_compile_fwdbase
		#include "HLSLSupport.cginc"
		#include "UnityShaderVariables.cginc"
		#define UNITY_PASS_FORWARDBASE
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"
 
		#define INTERNAL_DATA
		#define WorldReflectionVector(data,normal) data.worldRefl
		#define WorldNormalVector(data,normal) normal
		#line 1
		#line 65
 
		sampler2D _MainTex;
		fixed4 _Color;


		half4 _FogColor;
		half4 _FogIntensity;
		half4 _FogParam;
 
		struct Input {
			half2 uv_MainTex;
		};
 
		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}

		#ifdef LIGHTMAP_OFF
		struct v2f_surf {
		  half4 pos : SV_POSITION;
		  half2 pack0 : TEXCOORD0;
		  fixed3 normal : TEXCOORD1;
		  fixed3 vlight : TEXCOORD2;
		  half3 fog;
		  LIGHTING_COORDS(3,4)
		};
		#endif

		#ifndef LIGHTMAP_OFF
		struct v2f_surf {
		  half4 pos : SV_POSITION;
		  half2 pack0 : TEXCOORD0;
		  half2 lmap : TEXCOORD1;
		  half3 fog;
		  LIGHTING_COORDS(2,3)
		};
		#endif

		#ifndef LIGHTMAP_OFF
		half4 unity_LightmapST;
		#endif
		half4 _MainTex_ST;


		v2f_surf vert_surf (appdata_full v) {
		  v2f_surf o;
		  o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		  o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
		  #ifndef LIGHTMAP_OFF
		  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
		  #endif
		  half3 worldN = mul((half3x3)_Object2World, SCALED_NORMAL);
		  #ifdef LIGHTMAP_OFF
		  o.normal = worldN;
		  #endif
		  #ifdef LIGHTMAP_OFF
		  half3 shlight = ShadeSH9 (half4(worldN,1.0));
		  o.vlight = shlight;
		  #ifdef VERTEXLIGHT_ON
		  half3 worldPos = mul(_Object2World, v.vertex).xyz;
		  o.vlight += Shade4PointLights (
			unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
			unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
			unity_4LightAtten0, worldPos, worldN );
		  #endif // VERTEXLIGHT_ON
		  #endif // LIGHTMAP_OFF
		  TRANSFER_VERTEX_TO_FRAGMENT(o);

		  half4 wsPos = mul (_Object2World, v.vertex);
			half3 camDir = wsPos.xyz - _WorldSpaceCameraPos;
			half fogInt = saturate(length(camDir) * (1 / _FogParam.x) - 1.0) * (100-_FogParam.x);	
			half fogVert = max(0.0, (wsPos.y - _FogParam.w) * (1 / _FogParam.z));
			fogVert *= fogVert; 
			fogVert = (exp (-fogVert));
			half intensity = (1-exp(-_FogParam.y*fogInt*0.01)) * fogVert;
			o.fog = _FogIntensity.xyz * intensity;


		  return o;
		}

		#ifndef LIGHTMAP_OFF
		sampler2D unity_Lightmap;
		#ifndef DIRLIGHTMAP_OFF
		sampler2D unity_LightmapInd;
		#endif
		#endif
		fixed _Cutoff;

		fixed4 frag_surf (v2f_surf IN) : COLOR {
		  Input surfIN;
		  surfIN.uv_MainTex = IN.pack0.xy;
		  #ifdef UNITY_COMPILER_HLSL
		  SurfaceOutput o = (SurfaceOutput)0;
		  #else
		  SurfaceOutput o;
		  #endif
		  o.Albedo = 0.0;
		  o.Emission = 0.0;
		  o.Specular = 0.0;
		  o.Alpha = 0.0;
		  o.Gloss = 0.0;
		  #ifdef LIGHTMAP_OFF
		  o.Normal = IN.normal;
		  #endif
		  surf (surfIN, o);
		  clip (o.Alpha - _Cutoff);
		  fixed atten = LIGHT_ATTENUATION(IN);
		  fixed4 c = 0;

		  #ifdef LIGHTMAP_OFF
		  c = LightingLambert (o, _WorldSpaceLightPos0.xyz, atten);
		  #endif // LIGHTMAP_OFF

		  #ifdef LIGHTMAP_OFF
		  c.rgb += o.Albedo * IN.vlight;
		  #endif // LIGHTMAP_OFF

		  #ifndef LIGHTMAP_OFF
		  #ifdef DIRLIGHTMAP_OFF
		  fixed4 lmtex = tex2D(unity_Lightmap, IN.lmap.xy);
		  fixed3 lm = DecodeLightmap (lmtex);
		  #else
		  fixed4 lmtex = tex2D(unity_Lightmap, IN.lmap.xy);
		  fixed4 lmIndTex = tex2D(unity_LightmapInd, IN.lmap.xy);
		  half3 lm = LightingLambert_DirLightmap(o, lmtex, lmIndTex, 0).rgb;
		  #endif

		  #ifdef SHADOWS_SCREEN
		  #if defined(SHADER_API_GLES) && defined(SHADER_API_MOBILE)
		  c.rgb += o.Albedo * min(lm, atten*2);
		  #else
		  //c.rgb += o.Albedo * max(min(lm,(atten*2)*lmtex.rgb), lm*atten);
		  fixed3 lmDens = max(min(lm,(atten*2)*lmtex.rgb), lm*atten);
			c.rgb = lerp (o.Albedo, o.Albedo * lmDens, 0.8);
		  #endif
		  #else // SHADOWS_SCREEN
		  c.rgb += o.Albedo * lm;
		  #endif // SHADOWS_SCREEN
		  c.a = o.Alpha;
		#endif // LIGHTMAP_OFF
		  c.a = o.Alpha;

		  c.rgb = lerp (c.rgb, _FogColor.rgb, IN.fog.xyz);
		  return c;
	}
 
	ENDCG 
	}

	}
	Fallback "Transparent/Cutout/VertexLit"
}
**/
