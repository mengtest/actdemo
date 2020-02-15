// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

/*************************************************************************************************************
 * 功能 ： 漫反射 + 遮挡透明 (使用于透明物体) 
 ***************************************************************************************************************/

Shader "Snail/Diffuse-Mask-Cutout" {

	Properties {
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		// _OcclusionMask ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}

	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		Blend SrcAlpha OneMinusSrcAlpha  
		
		Pass {
			Name "BASE"
			
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct appdata members fog)
#pragma exclude_renderers d3d11 xbox360
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _OcclusionMask;
			float _Cutoff;

			float4 _FogColor;
			float4 _FogParam;
			float4 _FogIntensity;

			// 无光模式下定义光照贴图
			#ifndef LIGHTMAP_OFF
			// float4 unity_LightmapST;
			// sampler2D unity_Lightmap;
			#endif

			struct appdata {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				half2 uv[5] : TEXCOORD0;
			};

			/** 计算屏幕坐标 */
			float4 ComputeScreenPosition (float4 pos) {
				float4 o = pos * 0.5f;
				o.xy = float2(o.x, o.y*_ProjectionParams.x) + o.w;
				o.zw = pos.zw;
				return o;
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv[0] = v.texcoord;
		
				// 光照贴图UV
				#ifndef LIGHTMAP_OFF
				o.uv[4] = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
		
				// 高度距离雾
				float4 wsPos = mul (unity_ObjectToWorld, v.vertex);
				float3 camDir = wsPos.xyz - _WorldSpaceCameraPos;
				float fogInt = saturate(length(camDir) * (1 / _FogParam.x) - 1.0) * (100-_FogParam.x);
				float fogVert = max(0.0, (wsPos.y - _FogParam.w) * (1 / _FogParam.z));
				fogVert *= fogVert; 
				fogVert = (exp (-fogVert));
				float intensity = (1-exp(-_FogParam.y*fogInt*0.01)) * fogVert;
				o.uv[1].x = _FogIntensity.x * intensity;

				half4 ref = ComputeScreenPosition(o.pos);

				o.uv[2].xy = ref.xy;
				o.uv[3].xy = ref.wz;
		
				return o;
			}

			float4 frag (v2f i) : COLOR	
			{
				fixed4 c = tex2D (_MainTex, i.uv[0]);
				clip (c.a - _Cutoff);

				fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv[4]));
				c.rgb *= lm;

				half4 uv1; 
				uv1.xy = i.uv[2].xy;
				uv1.wz = i.uv[3].xy;
				half4 mask = tex2Dproj( _OcclusionMask, UNITY_PROJ_COORD(uv1) );

				c.a = (1 - mask.w * 2);
				// c.rgb = mask.xyz;
				c.rgb = lerp (c.rgb, _FogColor.rgb, i.uv[1].x);
				return c;
			}
			ENDCG			
		}
	}
	
	Fallback "VertexLit"
}
