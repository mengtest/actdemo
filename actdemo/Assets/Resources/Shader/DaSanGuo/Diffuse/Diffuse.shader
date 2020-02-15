﻿Shader "DaSanGuo/Diffuse/Diffuse" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert exclude_path:prepass noforwardadd
		#pragma exclude_renderers d3d11 d3d11_9x xbox360 ps3 flash

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = 1;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
