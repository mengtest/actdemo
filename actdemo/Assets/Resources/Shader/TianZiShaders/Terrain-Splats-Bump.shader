/****************************************************************************************************
 * 着色器 : 地形溅斑法线烘焙
 *****************************************************************************************************/
Shader "Snail/Terrain-Splats-Bump" {
	Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _BumpMap ("Bumpmap", 2D) = "bump" {}

	  _WaveScale ("波纹缩放", Range (0.002,0.15)) = .07							// 
		WaveSpeed ("波纹速度 (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)			// 水波速度		
		_DeepTex("深度纹理", 2D) = "white" {}										// 深度纹理

		_BumpMap ("Water BumpMap ", 2D) = "" {}										// 水面法线
		_RefrDistort ("Refr Distort", Range (0,1.5)) = 0.2							// 折算扭曲值
		_WaterHeight ("Water BumpMap ", Float) = 0.5								// 水面高度
    }

    SubShader {
		
      //Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent" }
	  //Blend SrcAlpha OneMinusSrcAlpha  
	  Tags { "RenderType"="Opaque" }
	  Cull Off

      CGPROGRAM
      #pragma surface surf Lambert

      struct Input {
        float2 uv_MainTex;
        float2 uv_BumpMap;
      };

      sampler2D _MainTex;
      sampler2D _BumpMap;

      void surf (Input IN, inout SurfaceOutput o) {
        o.Albedo = (tex2D (_MainTex, IN.uv_MainTex));
        o.Normal = (tex2D (_BumpMap, IN.uv_BumpMap) * 2 - 1) ;
      }
      ENDCG

    } 
    Fallback "Diffuse"
}
