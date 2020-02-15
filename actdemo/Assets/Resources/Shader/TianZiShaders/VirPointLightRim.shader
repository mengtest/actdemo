// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/***********************************************************************************************************
 * 着色器 : (模拟虚拟点光源)虚拟点光 + 边缘光 + 线性雾
 * 注解 : 使用于角色或NPC
 ************************************************************************************************************/
Shader "Snail/VirPointLightRim" { 
	Properties {
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}

		_PointLightMap ("点光模拟纹理", CUBE) = "" { Texgen CubeNormal }

		_RimColor ("Rim Color", Color) = (0.8,0.8,0.8,0.6)
		_RimMin ("Rim min", Range(0,1)) = 0.4
		_RimMax ("Rim max", Range(0,1)) = 0.6
		
		_TimeScale ("Time Scale", Float) = 28
	}

	SubShader {
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent" }
		
		Pass {
			Name "BASE"
			Cull Back
			
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct appdata members fog)
#pragma exclude_renderers d3d11 xbox360
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 

			#include "UnityCG.cginc"

			samplerCUBE _PointLightMap;

			sampler2D _MainTex;
			float4 _Color;
			float4 _MainTex_ST;

			// 太阳光参数 ------------------------------------------------------------------------------
			half3 _SunLightDir;										// 太阳光方向

			float4 _RimColor;
			float _RimMin;
			float _RimMax;

			float4 _FogColor;
			float4 _FogParam;
			float4 _FogIntensity;
			
			float _TimeScale;

			struct appdata {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};
			
			struct v2f {
				float4 pos : POSITION;
				//float2 texcoord : TEXCOORD0;
				//float3 fog;
				//float3 rim;
				//float3 pointLight : TEXCOORD1;

				half2 uv[7] : TEXCOORD0;
			};

			v2f vert (appdata v)
			{
				v2f o;
				float4 vertex = v.vertex;
				
				o.pos = UnityObjectToClipPos (vertex);
				o.uv[0] = TRANSFORM_TEX(v.texcoord, _MainTex);

				// 计算点光
				half3 pointLight = mul (UNITY_MATRIX_MV, float4(v.normal,0));
				o.uv[1].xy = pointLight.xy;
				o.uv[2].x = pointLight.z;

				// 计算边缘光强度
				half3 rim = 1.0f - saturate( dot(normalize(ObjSpaceViewDir(v.vertex)), v.normal));
				o.uv[2].y = rim.z;
				o.uv[3].xy = rim.xy;


				// 距离雾
				float4 wsPos = mul (unity_ObjectToWorld, v.vertex);
				float3 camDir = wsPos.xyz - _WorldSpaceCameraPos;
				float fogInt = saturate(length(camDir) * (1 / _FogParam.x) - 1.0) * (100-_FogParam.x);	

				// 高度雾
				float fogVert = max(0.0, (wsPos.y - _FogParam.w) * (1 / _FogParam.z));
				fogVert *= fogVert;
				fogVert = (exp (-fogVert));

				float intensity = (1-exp(-_FogParam.y*fogInt*0.01)) * fogVert;

				// float intensity = (1-exp(-_FogParam.y*fogInt*0.01));
				// o.fog = _FogIntensity.xyz * intensity;

				o.uv[4].x = _FogIntensity.x * intensity;

				return o;
			}

			float4 frag (v2f i) : COLOR
			{
				float4 c = _Color * tex2D(_MainTex, i.uv[0]) * 2 ;

				half3 pointLight;
				pointLight.xy = i.uv[1].xy;
				pointLight.z = i.uv[2].x;

				// 取样点光
				float4 light =  texCUBE(_PointLightMap, pointLight);

				c.rgb = 3.0f * light.rgb * c.rgb;

				// 计算边缘光
				half3 rim;
				rim.xy = i.uv[3].xy;
				rim.z = i.uv[2].y;
				
				
				_RimMin = _RimMin * abs(cos(_Time * _TimeScale));
				
				rim = smoothstep( _RimMin , 1 , rim ) ;
				c.rgb = c + c * rim  * 3 * _RimColor;

				// 计算雾效
				half fog = i.uv[4].x;
				c.rgb = lerp (c.rgb, _FogColor.rgb, fog);

				return  c;
			}
			ENDCG
		}
		

	} 

	Fallback "VertexLit"
}
