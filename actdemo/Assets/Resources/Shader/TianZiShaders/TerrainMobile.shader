// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

/*******************************************************************************************
 * 着色器 ：移动设备上的地形渲染
 * 注 ： 地形面积太大，着色器最好采用单面渲染
 ********************************************************************************************/

Shader "Snail/TerrainMobile" 
{ 
	Properties {
		_Splat0 ("Layer1 (RGB)", 2D) = "white" {}
		_Splat1 ("Layer2 (RGB)", 2D) = "white" {}
		_Splat2 ("Layer3 (RGB)", 2D) = "white" {}
		_Splat3 ("Layer4 (RGB)", 2D) = "white" {}
		_Control ("Control (RGBA)", 2D) = "white" {}
		_MainTex ("未使用", 2D) = "white" {}
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		
		LOD 300
		
		Pass {
			Name "ShadowPass"
			Tags {"LightMode" = "ForwardBase"}
			Cull Back	
			CGPROGRAM 
			// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members lightDir)
			#pragma exclude_renderers d3d11 xbox360
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
 
			struct v2f { 
				float4 pos : SV_POSITION;

				float4 uv1 : TEXCOORD0;						// 
				float4 uv2 : TEXCOORD1;
				float4 uv3 : TEXCOORD2;
				// float3 fog;
				
				SHADOW_COORDS(3) //float4 _ShadowCoord;
			};
 
			float4 _MainTex_ST;
			sampler2D _MainTex;
			
			sampler2D _Splat0 ;
			sampler2D _Splat1 ;
			sampler2D _Splat2 ;
			sampler2D _Splat3 ;
			sampler2D _Control;
			float4 _Splat0_ST;
			float4 _Splat1_ST;
			float4 _Splat2_ST;
			float4 _Splat3_ST;
			float4 _Control_ST;
			
			/**
			float4 _FogColor;
			float4 _FogIntensity;
			float4 _FogParam;
			**/

			// float4 unity_LightmapST;
			// sampler2D unity_Lightmap;

			v2f vert (appdata_full v)
			{
				v2f o;
				o.uv1.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv1.zw = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				
				o.uv2.xy = TRANSFORM_TEX (v.texcoord, _Splat0);
				o.uv2.zw = TRANSFORM_TEX (v.texcoord, _Splat1);
				
				o.uv3.xy = TRANSFORM_TEX (v.texcoord, _Splat2);
				o.uv3.zw = TRANSFORM_TEX (v.texcoord, _Splat3);
				
				o.pos = UnityObjectToClipPos (v.vertex);

				/**
				float4 wsPos = mul (_Object2World, v.vertex);
				float3 camDir = wsPos.xyz - _WorldSpaceCameraPos;
				float fogInt = saturate(length(camDir) * (1 / _FogParam.x) - 1.0) * (100-_FogParam.x);	
				float fogVert = max(0.0, (wsPos.y - _FogParam.w) * (1 / _FogParam.z));
				fogVert *= fogVert; 
				fogVert = (exp (-fogVert));
				float intensity = (1-exp(-_FogParam.y*fogInt*0.01)) * fogVert;
				o.fog = _FogIntensity.xyz * intensity;
				**/
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				

				return o;
			}
			
			float4 frag (v2f i) : COLOR
			{
				float atten = LIGHT_ATTENUATION(i);

				float4 Mask = tex2D( _Control, i.uv1.xy);
				float3 lay1 = tex2D( _Splat0, i.uv2.xy );
				float3 lay2 = tex2D( _Splat1, i.uv2.zw );
				float3 lay3 = tex2D( _Splat2, i.uv3.xy );
				float3 lay4 = tex2D( _Splat3, i.uv3.zw );
				
				float4 c = float4(1, 1, 1, 1);
				c.xyz = (lay1.xyz * Mask.r + lay2.xyz * Mask.g + lay3.xyz * Mask.b + lay4.xyz * Mask.a);
				
				
				c.rgb -= (1-atten ) * 0.25;

				fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv1.zw));
				c.rgb *= lm ;

				// c.rgb = lerp (c.rgb, _FogColor.rgb, i.fog.xyz);

				return c;
			}
			ENDCG
		}
	}
 
}





