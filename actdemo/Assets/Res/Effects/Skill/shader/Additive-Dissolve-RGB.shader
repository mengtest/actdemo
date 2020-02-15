// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effect/FX-Additive-Dissolve-RGB"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Main Texture", 2D) = "white" {}
  		_SliceGuide ("Slice Guide (A)", 2D) = "white" {}
		_SliceAmount ("Slice Amount", Range(0, 2.0)) = 0.5
		_AlphaAmount ("Alpha Amount", Range(0, 40.0)) = 20
	}

	Category
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="TransparentDissolveRGB" }
		SubShader
		{
			LOD 300
			Blend SrcAlpha One
			ColorMask RGB
			Cull Off
			Lighting Off
			ZWrite Off
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
				fixed4 _TintColor;
				
				sampler2D _SliceGuide;
				float4 _SliceGuide_ST;
				float _SliceAmount;
				float _AlphaAmount;
				
				struct appdata_t
				{
					half4 vertex : POSITION;
					half2 texcoord0 : TEXCOORD0;
					half2 texcoord1 : TEXCOORD1; 
					fixed4 color : COLOR;
				};
	
				struct v2f
				{
					half4 vertex : POSITION;
					half2 texcoord0 : TEXCOORD0;
					half2 texcoord1 : TEXCOORD1;
					fixed4 color : TEXCOORD2;
				};
	
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord0 = TRANSFORM_TEX(v.texcoord0, _MainTex);
					o.texcoord1 = TRANSFORM_TEX(v.texcoord1, _SliceGuide);
					o.color = v.color;
					
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{	
					fixed4 diff = tex2D(_MainTex, i.texcoord0);
					fixed4 final = diff * _TintColor * 2.0f * i.color;
					fixed control = tex2D(_SliceGuide, i.texcoord1).r;
					final.a *= (control - (_SliceAmount - 1)) * _AlphaAmount;

					return final;
				}
				ENDCG 
			}
		}
	Fallback off
	}
}
