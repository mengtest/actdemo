// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

/*********************************************************************************************************
 * 着色器 ：水面渲染
 * 注解 : 水面深度效果 + 天空反射 + 漫反射 + 高光
 **********************************************************************************************************/
Shader "Snail/Water" {
	Properties {
		_DeepTex("深度纹理", 2D) = "white" {}										// 深度纹理
		_horizonColor ("天光颜色", COLOR)  = ( .172 , .463 , .435 , 0)				// 
		_ColorControl ("反射颜色控制 (RGB) fresnel (A) ", 2D) = "" { }				// 
		_WaveScale ("波纹缩放", Range (0.002,0.15)) = .07							// 
		WaveSpeed ("波纹速度 (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)			// 水波速度		
		_BumpMap ("水面法线 ", 2D) = "" { }											// 
		_ReflDistort ("反射扭曲值", Range (0,1.5)) = 0.44							// 
		_RefrDistort ("折射扭曲值", Range (0,1.5)) = 0.2							// 

		_Alpha("透明度", Float) = 1 							
		_MainTex ("未使用", 2D) = "white" {}
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
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

			sampler2D _DeepTex;
			float4 _DeepTex_ST;

			float _Alpha;

			uniform float4 _horizonColor;
			uniform float4 WaveSpeed;
			uniform float _WaveScale;
			uniform float4 _WaveOffset;

			sampler2D _BumpMap;
			sampler2D _ColorControl;

			uniform float _ReflDistort;
			uniform float _RefrDistort;


			// 高光参数 ------------------------------------------------------------------------
			half3 _SunLightDir;										// 太阳光方向
			float _WaterSpecRange;									// 高光范围
			float _WaterSpecStrength;								// 高光强度

			float _WaterDiffRange;									// 高光范围

			struct appdata {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};
			
			struct v2f {
				float4 pos : POSITION;
				half2 uv[7] : TEXCOORD0;
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
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv[0] = TRANSFORM_TEX(v.texcoord, _DeepTex);

				float4 wsPos = mul (unity_ObjectToWorld, v.vertex);

				float4 temp;
				temp.xyzw = wsPos.xzxz * _WaveScale / 1.0 + _WaveOffset;
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

				// 计算雾效果
				col.a *= 0.8;

				return  col;
			}

			

			ENDCG			
		}
	}
	
	Fallback "VertexLit"
}
