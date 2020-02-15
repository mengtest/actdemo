// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*********************************************************************************************************
 * 着色器 ：水面渲染
 * 注解 : 水面深度效果 + 天空反射 + 漫反射 + 高光 + 实时平面反射
 **********************************************************************************************************/
Shader "Snail/WaterPro" {
	Properties {
		_DeepTex("深度纹理", 2D) = "white" {}	
		_horizonColor ("天光颜色", COLOR)  = ( .172 , .463 , .435 , 0)
		_ColorControl ("反射颜色控制 (RGB) fresnel (A) ", 2D) = "" { }	
		_WaveScale ("波纹缩放", Range (0.002,3)) = .07
		_BumpMap ("水面法线 ", 2D) = "" { }
		_ReflIntensity ("反射强度", Range (0,1)) = 1	
		_ReflLightness ("反射亮度", Range (1,4)) = 1	
		_ReflDistort ("反射扭曲值", Range (0,1.5)) = 0.44
		_ReflectionTex ("反射纹理，调试用", 2D) = "" {}
		_ShadowIntensity ("阴影强度", Range (0,1)) = 1
		_WaveDirection("水流方向", Vector) = (0, 0.05, 0, 0.02)
		_Alpha("透明度", Range (0.1,1)) = 1 							
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			Name "BASE"
			Cull Back
			
			CGPROGRAM
			#pragma only_renderers d3d9 d3d11 d3d11_9x opengl gles gles3 metal
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma multi_compile _ SHADOW_LIGHT_ON
			#pragma multi_compile _ REFLECTION_ON

			#include "UnityCG.cginc"

			sampler2D _DeepTex;
			float4 _DeepTex_ST;

			float _Alpha;

			uniform float4 _horizonColor;
			uniform float _WaveScale;
			uniform float _WaveOffset;

			sampler2D _BumpMap;
			sampler2D _ColorControl;

			// 反射
			uniform float _ReflDistort;
			sampler2D _ReflectionTex;
			half _ReflIntensity;
			half _ReflLightness;

			// 高光参数 ------------------------------------------------------------------------
			half3 _SunLightDir;										// 太阳光方向
			float _WaterSpecRange;									// 高光范围
			float _WaterSpecStrength;								// 高光强度

			float _WaterDiffRange;									// 高光范围

			// 阴影
			uniform sampler2D _x_shadowMap;
			uniform float4x4 _x_lightVP;
			fixed _ShadowIntensity;

			// 水流方向
			float4 _WaveDirection;

			struct appdata {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : POSITION;
				half2 uv[8] : TEXCOORD0;
				UNITY_FOG_COORDS(9)
			};

			/** 计算屏幕坐标 */
			float4 ComputeScreenPosition (float4 pos) {
				float4 o = pos * 0.5f;
				o.xy = float2(o.x, o.y*_ProjectionParams.x) + o.w;
				o.zw = pos.zw;
				return o;
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);

				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv[0] = TRANSFORM_TEX(v.texcoord, _DeepTex);

				float4 temp = v.texcoord.xyxy * _WaveScale + _WaveDirection * _WaveOffset;
				o.uv[1] = temp.xy * float2(.4, .45);
				o.uv[2] = temp.wz;


				half4 viewDir;
				viewDir.xzy = normalize(ObjSpaceViewDir(v.vertex));

				o.uv[3].xy = viewDir.xz;
				o.uv[4].x = viewDir.y;


				half4 ref;
				ref = ComputeScreenPosition(o.pos);

				o.uv[5].xy = ref.xy;
				o.uv[6].xy = ref.zw;

				#ifdef SHADOW_LIGHT_ON
				o.uv[7] = (mul(_x_lightVP, mul(unity_ObjectToWorld, v.vertex)) + 1) * 0.5;
				#endif

				UNITY_TRANSFER_FOG(o,o.pos);

				return o;
			}

			float4 frag (v2f i) : COLOR
			{
				
				// 最终颜色
				half4 col = 1;

				// 两层水波叠加
				half3 bump1 = UnpackNormal(tex2D( _BumpMap, i.uv[1] )).rgb;
				half3 bump2 = UnpackNormal(tex2D( _BumpMap, i.uv[2] )).rgb;
				half3 normal = normalize(bump1 + bump2) ;			// 调整波浪叠加法线值
				
				half3 viewDir;
				viewDir.xz = i.uv[3].xy;
				viewDir.y = i.uv[4].x;

				half fresnel = dot( viewDir, normal);

				half4 ref;
				ref.xy = i.uv[5].xy;
				ref.zw = i.uv[6].xy;

				// 计算天空反射 ------------------------------------------------------------------------
				half4 uv1 = ref * 2;
				uv1.xy += normal * 5;
				half4 refl = tex2Dproj( _ColorControl, UNITY_PROJ_COORD(uv1));
				refl.a = 1;

				// 采样深度 ----------------------------------------------------------------------------
 				half depth = tex2D(_DeepTex, i.uv[0]).r;

				col.rgb = lerp( _horizonColor.rgb, refl.rgb, _horizonColor.a);	

				// 计算高光 -----------------------------------------------------------------------------
				half3 h = normalize(_SunLightDir + viewDir);
				half diff = max(0, dot (normal, _SunLightDir));

				diff = pow (diff,  1.2);

				float nh = max (0, dot (normal, h));
				float spec = pow (nh, _WaterSpecRange * 50);

				// 叠加深度透明, 柔化边缘 ----------------------------------------------------------------
				col.a = depth * _Alpha;

				half lightCol = (diff * 0.3 + spec * _WaterSpecStrength);
				col.rgb += lightCol;													// 水面高光颜色

				// 反射
				#ifdef REFLECTION_ON
				half3 refl_bump = (bump1 + bump2) * 0.5;
				float4 refl_uv = ref;
				refl_uv.xy -= refl_bump * _ReflDistort;
				half4 refl_c = tex2Dproj( _ReflectionTex, UNITY_PROJ_COORD(refl_uv) );
				refl_c = min(refl_c * _ReflLightness, 1);
				col.rgb *= refl_c.rgb + (1 - refl_c.rgb) * _ReflIntensity;
				#endif

				// 阴影
				#ifdef SHADOW_LIGHT_ON
				fixed shadow = min(1, tex2D(_x_shadowMap, i.uv[7]).r + (1 - _ShadowIntensity));
				col.rgb *= shadow;
				#endif

				UNITY_APPLY_FOG(i.fogCoord, col);

				return  col;
			}

			

			ENDCG			
		}
	}
	
	Fallback "VertexLit"
}
