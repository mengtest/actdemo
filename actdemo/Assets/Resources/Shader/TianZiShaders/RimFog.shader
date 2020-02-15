// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/***********************************************************************************************************
 * 着色器 : 边缘光 + 线性雾
 * 注解 : 使用于角色或NPC
 ************************************************************************************************************/
Shader "Snail/RimFog" {
	Properties {
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}

		_RimColor ("Rim Color", Color) = (0.8,0.8,0.8,0.6)
		_RimMin ("Rim min", Range(0,1)) = 0.4
		_RimMax ("Rim max", Range(0,1)) = 0.6
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		
	

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

			sampler2D _MainTex;
			float4 _Color;
			float4 _MainTex_ST;

			float4 _RimColor;
			float _RimMin;
			float _RimMax;

			float4 _FogColor;
			float4 _FogParam;
			float4 _FogIntensity;

			struct appdata {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};
			
			struct v2f {
				float4 pos : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 fog;
				//float3 rim;
			};

			v2f vert (appdata v)
			{
				v2f o;
				float4 vertex = v.vertex;
				//vertex.z = 0;
				o.pos = UnityObjectToClipPos (vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				//o.rim = 1.0f - saturate( dot(normalize(ObjSpaceViewDir(v.vertex)), v.normal));

				// startDistance, globalDensity, heightScale, height

				// 角色只进行距离雾计算, 避免性能开支

				// 距离雾
				float4 wsPos = mul (unity_ObjectToWorld, v.vertex);
				float3 camDir = wsPos.xyz - _WorldSpaceCameraPos;
				float fogInt = saturate(length(camDir) * (1 / _FogParam.x) - 1.0) * (100-_FogParam.x);	

				// 高度雾
				float fogVert = max(0.0, (wsPos.y - _FogParam.w) * (1 / _FogParam.z));
				fogVert *= fogVert;
				fogVert = (exp (-fogVert));

				float intensity = (1-exp(-_FogParam.y*fogInt*0.01)) * fogVert;

				//float intensity = (1-exp(-_FogParam.y*fogInt*0.01));
				o.fog = _FogIntensity.xyz * intensity;
				return o;
			}

			float4 frag (v2f i) : COLOR
			{
				float4 c = _Color * tex2D(_MainTex, i.texcoord) * 2;

				// i.rim = smoothstep(_RimMin, _RimMax, i.rim);
				// c.rgb = c + c * i.rim * 3 * _RimColor;

				c.rgb = lerp (c.rgb, _FogColor.rgb, i.fog.xyz);

				//c = 0.1;
				return  c;
			}
			ENDCG			
		}
	} 


	
	Fallback "VertexLit"
}
