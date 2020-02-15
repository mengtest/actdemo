// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*******************************************************************************************
 * 着色器 ：移动设备上的地形渲染
 * 注 ： 地形面积太大，着色器最好采用单面渲染
 ********************************************************************************************/
Shader "Snail/Terrain-Splats-Bump-Texture" {
	Properties {
		_Splat0 ("Layer1 (RGB)", 2D) = "white" {}
		_Splat1 ("Layer2 (RGB)", 2D) = "white" {}
		_Splat2 ("Layer3 (RGB)", 2D) = "white" {}
		
		_Control ("Control (RGBA)", 2D) = "white" {}
		_MainTex ("未使用", 2D) = "white" {}
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		Cull Off
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

			struct v2f {
				float4  pos : SV_POSITION;
				float2  uv[4] : TEXCOORD0;
			};


			float4 _Splat0_ST;
			float4 _Splat1_ST;
			float4 _Splat2_ST;
			
			float4 _Control_ST;

			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv[0] = TRANSFORM_TEX (v.texcoord, _Splat0);
				o.uv[1] = TRANSFORM_TEX (v.texcoord, _Splat1);
				o.uv[2] = TRANSFORM_TEX (v.texcoord, _Splat2);
	            o.uv[3] = TRANSFORM_TEX (v.texcoord, _Control);
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				fixed4 Mask = tex2D( _Control, i.uv[3].xy ).rgba;
				fixed3 lay1 = (tex2D( _Splat0, i.uv[0].xy ));
				fixed3 lay2 = (tex2D( _Splat1, i.uv[1].xy ));
				fixed3 lay3 = (tex2D( _Splat2, i.uv[2].xy ));
   				
    			fixed4 c;
				c.xyz = (lay1.xyz * Mask.r + lay2.xyz * Mask.g + lay3.xyz * Mask.b );
				c.w = 0;

				c.xyz =  (tex2D( _Splat0, i.uv[0].xy ));
				return c;
			}
			ENDCG
		}
	}
} 