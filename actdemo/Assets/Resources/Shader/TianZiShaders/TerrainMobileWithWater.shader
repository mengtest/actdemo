// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

/*******************************************************************************************
 * 着色器 ：移动设备上的地形渲染
 * 注解 ： 拥有水面的地形
 ********************************************************************************************/
Shader "Snail/TerrainMobileWithWater" {
	Properties {
		_Splat0 ("Layer1 (RGB)", 2D) = "white" {}
		_Splat1 ("Layer2 (RGB)", 2D) = "white" {}
		_Splat2 ("Layer3 (RGB)", 2D) = "white" {}
		_Control ("Control (RGBA)", 2D) = "white" {}								// 地形溅斑纹理

		_WaveScale ("波纹缩放", Range (0.002,0.15)) = .07							// 
		WaveSpeed ("波纹速度 (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)			// 水波速度		

		_DeepTex("深度纹理", 2D) = "white" {}										// 深度纹理

		_BumpMap ("Water BumpMap ", 2D) = "" {}										// 水面法线
		_RefrDistort ("Refr Distort", Range (0,1.5)) = 0.2							// 折算扭曲值
		_WaterHeight ("Water BumpMap ", Float) = 0.5								// 水面高度
		
		_MainTex ("未使用", 2D) = "white" {}
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
			
			Cull Back


			CGPROGRAM
	// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members fog)
	#pragma exclude_renderers d3d11 xbox360
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
			#pragma exclude_renderers xbox360 ps3
			sampler2D _Splat0 ;
			sampler2D _Splat1 ;
			sampler2D _Splat2 ;
			sampler2D _Splat3 ;
			sampler2D _Control;

			sampler2D _DeepTex;

			struct v2f {
				float4  pos : SV_POSITION;
				#ifdef LIGHTMAP_ON
				float2  uv[8] : TEXCOORD0;
				#endif
				#ifdef LIGHTMAP_OFF
				float2  uv[7] : TEXCOORD0;
				#endif

				// float2 bumpuv[2];								// 水波UV值

				// float3 fog;

			};
		
			float4 _FogColor;
			float4 _FogParam;
			float4 _FogIntensity;

			float4 _Splat0_ST;
			float4 _Splat1_ST;
			float4 _Splat2_ST;

			float4 _PointLightPos;					// 
			float _PointLightRangeMin;
			float _PointLightRangeMax;
			float _PointLightIntensity;
			
			float4 _Control_ST;
			#ifdef LIGHTMAP_ON
				fixed4 unity_LightmapST;
				// sampler2D unity_Lightmap;
			#endif

			/** 水面高度 */
			float _WaterHeight;

			/** 水面扭曲法线 */
			sampler2D _BumpMap;

			/** 反射及折算强度 */
			uniform float _ReflDistort;
			uniform float _RefrDistort;

			/** 水波设置 */
			uniform float4 WaveSpeed;
			uniform float _WaveScale;
			uniform float4 _WaveOffset;

			v2f vert (appdata_full v)
			{
				v2f o;

				float4 vertex = v.vertex;
				
				o.pos = UnityObjectToClipPos (vertex);
				o.uv[0] = TRANSFORM_TEX (v.texcoord, _Splat0);
				o.uv[1] = TRANSFORM_TEX (v.texcoord, _Splat1);
				o.uv[2] = TRANSFORM_TEX (v.texcoord, _Splat2);
				
				o.uv[3] = TRANSFORM_TEX (v.texcoord, _Control);
				#ifdef LIGHTMAP_ON
            		o.uv[7] = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				// 计算水波偏移UV
				float4 temp;
				temp.xyzw = vertex.xzxz * _WaveScale / 1.0 + _WaveOffset;
				o.uv[4] = temp.xy * float2(.4, .45);
				o.uv[5] = temp.wz;
		
				float4 wsPos = mul (unity_ObjectToWorld, vertex);
				float3 camDir = wsPos.xyz - _WorldSpaceCameraPos;
				float fogInt = saturate(length(camDir) * (1 / _FogParam.x) - 1.0) * (100-_FogParam.x);	
				float fogVert = max(0.0, (wsPos.y - _FogParam.w) * (1 / _FogParam.z));
				fogVert *= fogVert; 
				fogVert = (exp (-fogVert));
				float intensity = (1-exp(-_FogParam.y*fogInt*0.01)) * fogVert;
				o.uv[6].x = _FogIntensity.x * intensity;

				half dis = distance(_PointLightPos.xyz, wsPos.xyz) ;
				o.uv[6].y = smoothstep(_PointLightRangeMin, _PointLightRangeMax, dis);
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				// 计算深度
				float depth = tex2D(_DeepTex, i.uv[3]).r;
				
				// 两层水波叠加
				half3 bump1 = UnpackNormal(tex2D( _BumpMap, i.uv[4] )).rgb;
				half3 bump2 = UnpackNormal(tex2D( _BumpMap, i.uv[5] )).rgb;
				half3 bump = normalize(bump1 + bump2);			// 调整波浪叠加增值

				// 偏移UV
				float2 offset = bump * _RefrDistort * depth;

				// 对溅斑纹理进行扭曲
				float2 maskUV =  i.uv[3].xy - offset;
				half4 Mask = tex2D( _Control, maskUV).rgba;
				
				// fixed4 Mask = tex2D( _Control, i.uv[3].xy ).rgba;
				fixed3 lay1 = tex2D( _Splat0, i.uv[0].xy - offset);
				fixed3 lay2 = tex2D( _Splat1, i.uv[1].xy - offset);
				fixed3 lay3 = tex2D( _Splat2, i.uv[2].xy - offset);
   				
    			fixed4 c;
				c.xyz = (lay1.xyz * Mask.r + lay2.xyz * Mask.g + lay3.xyz * Mask.b );
				#ifdef LIGHTMAP_ON
           			 c.rgb *= (DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv[7])));
				#endif
				c.w = 0;

				float fog = i.uv[6].x;

				c.rgb = c.rgb + c.rgb * (1 - i.uv[6].y) * _PointLightIntensity;

				c.rgb = lerp (c.rgb, _FogColor.rgb, fog);

				return c;
			}
			ENDCG
		}
	}
} 