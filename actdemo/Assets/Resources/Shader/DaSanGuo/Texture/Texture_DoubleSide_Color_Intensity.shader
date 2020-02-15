Shader "DaSanGuo/Texture/Texture_DoubleSide_Color_Intensity" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
		_Intensity ("Intensity", Range (0, 10)) = 1
	}
	SubShader 
	{
		Pass 
		{
			Cull Off

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma exclude_renderers d3d11 d3d11_9x xbox360 ps3 flash

			#include "UnityCG.cginc"

			fixed4 _Color;
			uniform sampler2D _MainTex;
			uniform fixed _Intensity;

			fixed4 frag(v2f_img i) : COLOR 
			{
				fixed4 c = tex2D(_MainTex, i.uv) * _Color;
				c.rgb *= _Intensity;
				c.a = 1;
				return c;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
