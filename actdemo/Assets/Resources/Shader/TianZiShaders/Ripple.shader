// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Snail/Ripple" {
	Properties {
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_WaveScale ("涟漪缩放", Vector) = (1,1,1,1)
		_Color ("Main Color", Color) = (.5,.5,.5,1)
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha One
		ZWrite Off
		Pass {
			Name "BASE"
			Cull Back
			CGPROGRAM
		#pragma exclude_renderers d3d11 xbox360
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 

			#include "UnityCG.cginc"

			uniform float4 _WaveScale;

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;

			struct appdata {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};
			
			struct v2f {
				float4 pos : POSITION;
				half2 uv[4] : TEXCOORD0;
			};

			v2f vert (appdata v)
			{
				v2f o;

				float4 vertex = v.vertex;

				o.pos = UnityObjectToClipPos (vertex ) ;
				half2 baseUV = TRANSFORM_TEX(v.texcoord, _MainTex);

				o.uv[0] = baseUV * _WaveScale.x - (0.5 - (1 / _WaveScale.x) * 0.5) * _WaveScale.x;

				//o.uv[0] =  baseUV * _WaveScale.x   - 0.75;	//(baseUV * _WaveScale.x - 1) * 0.5 ;

				o.uv[1] =  baseUV * _WaveScale.y * 0.5;
				o.uv[2] =  baseUV * _WaveScale.z * 0.5;
				o.uv[3] =  baseUV * _WaveScale.w * 0.5;
				return o;
			}

			float4 frag (v2f i) : COLOR
			{
				half4 lay1 = tex2D(_MainTex, i.uv[0]) * _Color;
				half4 lay2 = tex2D(_MainTex, i.uv[1]);
				half4 lay3 = tex2D(_MainTex, i.uv[2]);
				half4 lay4 = tex2D(_MainTex, i.uv[3]);
				return  lay1;
			}
			ENDCG			
		}

	}
}
