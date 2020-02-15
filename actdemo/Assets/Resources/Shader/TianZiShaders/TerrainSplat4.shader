/*******************************************************************************************
 * 着色器 ：地形渲染(无发现模式-实时光渲染)
 * 注 ： 地形面积太大，着色器最好采用单面渲染
 ********************************************************************************************/
 
Shader "Snail/TerrainSplat4" {
	Properties {
		_Splat0 ("Layer 1", 2D) = "white" {}
		_Splat1 ("Layer 2", 2D) = "white" {}
		_Splat2 ("Layer 3", 2D) = "white" {}
		_Splat3 ("Layer 4", 2D) = "white" {}
		_Control ("Control (RGBA)", 2D) = "white" {}

		_MainTex ("未使用", 2D) = "white" {}
	}

	SubShader {
		Tags {
	   "RenderType" = "Opaque"
		}

	CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360 because it uses wrong array syntax (type[size] name)
#pragma exclude_renderers d3d11 xbox360
	#pragma surface surf Lambert    vertex:vert
	#pragma exclude_renderers xbox360 ps3 gles
	#include "UnityCG.cginc"
	struct Input {
		float2 uv_Control ;

		// float2 uv_Splat[4];
		float2 uv_Splat0 ;
		float2 uv_Splat1 ;
		float2 uv_Splat2 ;
		float2 uv_Splat3 ;
	};
 
	sampler2D _Control;
	sampler2D _Splat0;
	sampler2D _Splat1;
	sampler2D _Splat2;
	sampler2D _Splat3;

	void vert (inout appdata_full v, out Input o) {
			  UNITY_INITIALIZE_OUTPUT(Input,o);

		  }

	

	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 splat_control = tex2D (_Control, IN.uv_Control).rgba;
		fixed3 lay1 = tex2D (_Splat0, IN.uv_Splat0);
		fixed3 lay2 = tex2D (_Splat1, IN.uv_Splat1);
		fixed3 lay3 = tex2D (_Splat2, IN.uv_Splat2);
		fixed3 lay4 = tex2D (_Splat3, IN.uv_Splat3);
		o.Alpha = 0.0;
		o.Albedo.rgb = (lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b + lay4 * splat_control.a);

	}
	ENDCG 
	}
	// Fallback to Diffuse
	// Fallback "Diffuse"
}
