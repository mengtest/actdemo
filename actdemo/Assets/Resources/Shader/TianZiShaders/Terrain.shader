// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

/*******************************************************************************************
 * 着色器 ：地形渲染
 * 注 ： 地形面积太大，着色器最好采用单面渲染
 ********************************************************************************************/

Shader "Snail/Terrain"
{
	Properties {
		_Splat0 ("Layer 1", 2D) = "white" {}
		_Splat1 ("Layer 2", 2D) = "white" {}
		_Splat2 ("Layer 3", 2D) = "white" {}
		//_Splat3 ("Layer 4", 2D) = "white" {}
		
		_SplatNormalMap0 ("SplatNormalMap 1", 2D) = "white" {}
		_SplatNormalMap1 ("SplatNormalMap 2", 2D) = "white" {}
		_SplatNormalMap2 ("SplatNormalMap 3", 2D) = "white" {}
		//_SplatNormalMap3 ("SplatNormalMap 4", 2D) = "white" {}

		_Control ("Control (RGBA)", 2D) = "white" {}

		_WaveScale ("波纹缩放", Range (0.002,0.15)) = .07							// 
		WaveSpeed ("波纹速度 (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)			// 水波速度		
		_DeepTex("深度纹理", 2D) = "white" {}										// 深度纹理

		_BumpMap ("Water BumpMap ", 2D) = "" {}										// 水面法线
		_RefrDistort ("Refr Distort", Range (0,1.5)) = 0.2							// 折算扭曲值
		_WaterHeight ("Water BumpMap ", Float) = 0.5								// 水面高度
		
		_MainTex ("未使用", 2D) = "white" {}
	}

SubShader {

		Tags { "RenderType" = "Opaque" }

		CGPROGRAM
		
		#pragma surface surf Lambert   finalcolor:frag  vertex:vert  
		#include "UnityCG.cginc"

		struct Input {
			float2 uv_Control ;
			float2 uv_Splat0 ;
			float2 uv_Splat1 ;
			float2 uv_Splat2 ;
			float2 uv_Splat3 ;
			float3 fog; 
			float gFogIntensity;
		};
		
		sampler2D _Control;
		sampler2D _Splat0;
		sampler2D _Splat1;
		sampler2D _Splat2;
		sampler2D _Splat3;

		sampler2D _SplatNormalMap0;
		sampler2D _SplatNormalMap1;
		sampler2D _SplatNormalMap2;
		sampler2D _SplatNormalMap3;

		float4 _FogColor;
		float4 _FogParam;
		float4 _FogIntensity;

		void vert (inout appdata_full v, out Input o) {
				UNITY_INITIALIZE_OUTPUT(Input,o);

				float4 wsPos = mul (unity_ObjectToWorld, v.vertex);
				float3 camDir = wsPos.xyz - _WorldSpaceCameraPos;
				float fogInt = saturate(length(camDir) * (1 / _FogParam.x) - 1.0) * (100-_FogParam.x);
				float fogVert = max(0.0, (wsPos.y - _FogParam.w) * (1 / _FogParam.z));
				fogVert *= fogVert; 
				fogVert = (exp (-fogVert));
				float intensity = (1-exp(-_FogParam.y*fogInt*0.01)) * fogVert;
				o.fog = _FogIntensity.xyz * intensity;
				o.gFogIntensity = intensity;
		}

		void frag (Input IN, SurfaceOutput o, inout fixed4 color)
		{
			fixed3 fogColor = _FogColor.rgb;
			#ifdef UNITY_PASS_FORWARDADD
			fogColor = 0;
			#endif
			color.rgb = lerp (color.rgb, fogColor, IN.fog.xyz);
			color.a = IN.gFogIntensity;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 splat_control = tex2D (_Control, IN.uv_Control).rgba;
			
			fixed3 lay1 = tex2D (_Splat0, IN.uv_Splat0);
			fixed3 lay2 = tex2D (_Splat1, IN.uv_Splat1);
			fixed3 lay3 = tex2D (_Splat2, IN.uv_Splat2);
			//fixed3 lay4 = tex2D (_Splat3, IN.uv_Splat4);
			//o.Alpha = splat_control.r + splat_control.g;
			o.Albedo = (lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b );
			
			// 法线采样
			fixed3 mixNormal = fixed3(0, 0, 0);

			fixed3 normal1 = (tex2D (_SplatNormalMap0, IN.uv_Splat0));
			fixed3 normal2 = (tex2D (_SplatNormalMap1, IN.uv_Splat1));
			fixed3 normal3 = (tex2D (_SplatNormalMap2, IN.uv_Splat2));

			// mixNormal = lerp(normal0, normal1, splat_control.r) + lerp(normal0, normal2, splat_control.g) + lerp(normal0, normal3, splat_control.b);

			mixNormal = normal1 *  splat_control.r + normal2 * splat_control.g + normal3 * splat_control.b;


			// 设置地形的法线
			o.Normal = (mixNormal * 2 - 1);
		}
		
		ENDCG


	}
	Fallback "Diffuse"
}


