// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

/*************************************************************************************************************
 * 功能 ： 光照贴图下漫反射着色器 - 模拟点光源
 ***************************************************************************************************************/

Shader "Snail/Diffuse-PointLight-Cutout" 
{
	Properties {
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	
	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		
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
			#include "AutoLight.cginc"
 
			struct v2f { 
				float2 uv_MainTex : TEXCOORD0;
				float4 pos : SV_POSITION;
				float2 lmap : TEXCOORD1;
				float4 illumination;
				SHADOW_COORDS(3) //float4 _ShadowCoord;
			};
 
			float4 _MainTex_ST;
			sampler2D _MainTex;

			float _Cutoff;

			// 点光属性
			float4 _PointLightPos;				
			float _PointLightRangeMin;
			float _PointLightRangeMax;
			float _PointLightIntensity;
			float4 _PointLightColor;

			// 无光模式下定义光照贴图
			// float4 unity_LightmapST;
			// sampler2D unity_Lightmap;

			v2f vert (appdata_full v)
			{
				v2f o;
				o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.pos = UnityObjectToClipPos (v.vertex);
				o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

				float4 wsPos = mul (unity_ObjectToWorld, v.vertex);
				o.illumination = wsPos;

				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}
 
			float4 frag (v2f i) : COLOR
			{
				half4 c = tex2D(_MainTex, i.uv_MainTex);

				clip (c.a - _Cutoff);

				float atten = LIGHT_ATTENUATION(i);

				// 加上阴影信息
				c.rgb -= (1-atten) * 0.25;

				// 拾取光照贴图信息
				fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				c.rgb *= lm;

				float dis = distance(_PointLightPos.xyz, i.illumination.xyz) ;
				float fogAndLight = smoothstep(_PointLightRangeMin, _PointLightRangeMax, dis);

				float indensity = max(0f, (0.5f - fogAndLight));
				c.rgb = c.rgb + c.rgb * indensity * _PointLightIntensity * _PointLightColor.rgb;

				return c;
			}
			ENDCG
		}
	}
 
}
