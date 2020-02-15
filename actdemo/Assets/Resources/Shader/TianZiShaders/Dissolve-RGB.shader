// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effect/Marsk RGB"
{
	Properties
	{
		_Outline ("Outline Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Main Texture", 2D) = "white" {}
  		_SliceGuide ("Slice Guide (RGB)", 2D) = "white" {}
		_SliceAmount ("Slice Amount", Range(0.0, 1.0)) = 0.5
	}

	Category
	{
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="OpaqueDissolveRGB" }
		SubShader
		{
			LOD 300
			Cull Off
			Lighting Off
			ZWrite On
			Fog { Mode Off }
			
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
	
				#include "UnityCG.cginc" 
	
				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed4 _Outline;
				
				sampler2D _SliceGuide;
				float4 _SliceGuide_ST;
				float _SliceAmount;
				
				struct appdata_t
				{
					half4 vertex : POSITION;
					half2 texcoord0 : TEXCOORD0;
					half2 texcoord1 : TEXCOORD1;
				};
	
				struct v2f
				{
					half4 vertex : POSITION;
					half2 texcoord0 : TEXCOORD0;
					half2 texcoord1 : TEXCOORD1;
				};
	
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord0 = TRANSFORM_TEX(v.texcoord0, _MainTex);
					o.texcoord1 = TRANSFORM_TEX(v.texcoord1, _SliceGuide);
					
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					clip( tex2D(_SliceGuide, i.texcoord1).rgb - _SliceAmount );
					
					fixed4 diff = tex2D(_MainTex, i.texcoord0);
					fixed4 final = diff * _Outline * 2.0f;
					
					return final;
				}
				ENDCG 
			}
		}
		
//		SubShader
//		{
//			LOD 200
//			
//			Blend SrcAlpha One
//			Cull Back
//			Lighting Off
//			ZWrite On
//			Fog { Mode Off }
//			
//			Pass
//			{
//				CGPROGRAM
//				#pragma vertex vert
//				#pragma fragment frag
//				#pragma fragmentoption ARB_precision_hint_fastest
//	
//				#include "UnityCG.cginc" 
//	
//				sampler2D _MainTex;
//				float4 _MainTex_ST;
//				fixed4 _TintColor;
//				
//				sampler2D _SliceGuide;
//				float4 _SliceGuide_ST;
//				float _SliceAmount;
//				
//				struct appdata_t
//				{
//					half4 vertex : POSITION;
//					half2 texcoord0 : TEXCOORD0;
//					half2 texcoord1 : TEXCOORD1;
//				};
//	
//				struct v2f
//				{
//					half4 vertex : POSITION;
//					half2 texcoord0 : TEXCOORD0;
//					half2 texcoord1 : TEXCOORD1;
//				};
//	
//				v2f vert (appdata_t v)
//				{
//					v2f o;
//					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
//					o.texcoord0 = TRANSFORM_TEX(v.texcoord0, _MainTex);
//					o.texcoord1 = TRANSFORM_TEX(v.texcoord1, _SliceGuide);
//					
//					return o;
//				}
//				
//				fixed4 frag (v2f i) : COLOR
//				{					
//					fixed4 sliceTex = tex2D(_SliceGuide, i.texcoord1);
//					if( (sliceTex.r - _SliceAmount) < 0.0f || (sliceTex.g - _SliceAmount) < 0.0f || (sliceTex.b - _SliceAmount) < 0.0f)
//					{
//						return fixed4(0.0f, 0.0f, 0.0f, 0.0f);
//					}
//					
//					fixed4 diff = tex2D(_MainTex, i.texcoord0);
//					fixed4 final = diff * _TintColor * 2.0f;
//					
//					return final;
//				}
//				ENDCG 
//			}
//		}
	Fallback off
	}
}
