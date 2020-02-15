/**************************************************************************************************
 * 功能 : 后期效果处理
 **************************************************************************************************/
Shader "Snail/PostEffect" {
	Properties {
		_MainTex ("Base (RGB)", RECT) = "white" {}
		_RampTex ("Base (RGB)", 2D) = "grayscaleRamp" {}
		_Desat("饱和度", Float) = 0.5
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

			/** 高斯模糊权重 */
			static const half curve[7] = { 0.0205, 0.0855, 0.232, 0.324, 0.232, 0.0855, 0.0205 };  

			/** 高斯模糊权重(4D向量) */
			static const half4 curve4[7] = { half4(0.0205,0.0205,0.0205,0), half4(0.0855,0.0855,0.0855,0), half4(0.232,0.232,0.232,0),
				half4(0.324,0.324,0.324,1), half4(0.232,0.232,0.232,0), half4(0.0855,0.0855,0.0855,0), half4(0.0205,0.0205,0.0205,0) };



			/** 将颜色转为灰阶值 */
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

				// 调整色彩纯度
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