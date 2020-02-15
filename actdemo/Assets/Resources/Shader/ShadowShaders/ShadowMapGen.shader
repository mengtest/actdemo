// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Snail/ShadowMapGen" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader 
	{
		ZTest Off
	    ZWrite Off
		Tags { "RenderType" = "ShadowMapCullBack" }
		Pass 
		{
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers d3d11 d3d11_9x xbox360 ps3 flash

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform fixed _shadowIntensity;
			uniform fixed4 _shadowColor;

			struct VertIn
			{
				float4 vertex : POSITION;
			};

			struct FragIn
			{
				float4 pos : POSITION;
			};

			FragIn vert(VertIn i)
			{
				FragIn o;
				o.pos = UnityObjectToClipPos (i.vertex);
				return o;
			}

			fixed4 frag(FragIn i) : COLOR 
			{
				return _shadowColor;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
