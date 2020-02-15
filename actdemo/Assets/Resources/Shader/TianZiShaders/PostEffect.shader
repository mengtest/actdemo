/**************************************************************************************************
 * ���� : ����Ч������
 **************************************************************************************************/
Shader "Snail/PostEffect" {
	Properties {
		_MainTex ("Base (RGB)", RECT) = "white" {}
		_RampTex ("Base (RGB)", 2D) = "grayscaleRamp" {}
		_Desat("���Ͷ�", Float) = 0.5
	}

SubShader {
		Pass {
			ZTest Always 
			Cull Off 
			ZWrite Off
			Fog { Mode off }

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _RampTex;
			uniform float4 _RampOffset;
			uniform float _Desat;

			/** ��˹ģ��Ȩ�� */
			static const half curve[7] = { 0.0205, 0.0855, 0.232, 0.324, 0.232, 0.0855, 0.0205 };  

			/** ��˹ģ��Ȩ��(4D����) */
			static const half4 curve4[7] = { half4(0.0205,0.0205,0.0205,0), half4(0.0855,0.0855,0.0855,0), half4(0.232,0.232,0.232,0),
				half4(0.324,0.324,0.324,1), half4(0.232,0.232,0.232,0), half4(0.0855,0.0855,0.0855,0), half4(0.0205,0.0205,0.0205,0) };



			/** ����ɫתΪ�ҽ�ֵ */
			inline fixed GetLuminance( fixed3 c )
			{
				return dot( c, fixed3(0.22, 0.707, 0.071) );
			}

			float4 frag (v2f_img i) : COLOR 
			{
				float4 original = tex2D(_MainTex, i.uv);
				float grayscale = GetLuminance(original.rgb);
				float2 remap = float2 (grayscale, .5);
				float4 output = tex2D(_RampTex, remap);

				float invert = 1 - _Desat;

				// ����ɫ�ʴ���
				output.r = original.r * invert + output.r * _Desat + _RampOffset.r;
				output.g = original.g * invert + output.g * _Desat + _RampOffset.g;
				output.b = original.b * invert + output.b * _Desat + _RampOffset.b;
				output.a = original.a;
				return output;
			}
			ENDCG

		}
	}

	Fallback off

}