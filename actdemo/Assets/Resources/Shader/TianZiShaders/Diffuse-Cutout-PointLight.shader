// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

/*************************************************************************************************************
 * 功能 ： 光照贴图下漫反射着色器 - 模拟点光源
 ***************************************************************************************************************/

 Shader "Snail/Diffuse-Cutout-PointLight" {
Properties {
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 200
	Cull Off
	
	CGINCLUDE
	// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members fog)
	#pragma exclude_renderers d3d11 xbox360
	#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
	#include "UnityCG.cginc"
	sampler2D _MainTex;
	
    float _Cutoff;

	float4 _FogColor;
	float4 _FogParam;
	float4 _FogIntensity;

    float4 _PointLightPos;					        // 
	float _PointLightRangeMin;                      // 
	float _PointLightRangeMax;                      // 
	float _PointLightIntensity;                     // 

	// 无光模式下定义光照贴图
	#ifndef LIGHTMAP_OFF
	// float4 unity_LightmapST;
	// sampler2D unity_Lightmap;
	#endif

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float3 fogAndLight;
		#ifndef LIGHTMAP_OFF
		float2 lmap : TEXCOORD1;
		#endif
	};

	
	v2f vert (appdata_full v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord;
		
		// 光照贴图UV
		#ifndef LIGHTMAP_OFF
		o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
		#endif

		// 高度距离雾
		float4 wsPos = mul (unity_ObjectToWorld, v.vertex);
		float3 camDir = wsPos.xyz - _WorldSpaceCameraPos;
		float fogInt = saturate(length(camDir) * (1 / _FogParam.x) - 1.0) * (100-_FogParam.x);	
		float fogVert = max(0.0, (wsPos.y - _FogParam.w) * (1 / _FogParam.z));
		fogVert *= fogVert; 
		fogVert = (exp (-fogVert));
		float intensity = (1-exp(-_FogParam.y*fogInt*0.01)) * fogVert;
		o.fogAndLight.x = _FogIntensity.x * intensity;

		half dis = distance(_PointLightPos.xyz, wsPos.xyz) ;
		o.fogAndLight.y = smoothstep(_PointLightRangeMin, _PointLightRangeMax, dis);

		return o;
	}
	ENDCG


	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest		
		fixed4 frag (v2f i) : COLOR
		{
			fixed4 c = tex2D (_MainTex, i.uv);
            clip (c.a - _Cutoff);

			#ifndef LIGHTMAP_ON
			c.rgb *= 1f;
			#else
			fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
			c.rgb *= lm;
			#endif

            c.rgb = c.rgb + c.rgb * (1 - i.fogAndLight.y) * _PointLightIntensity;
			c.rgb = lerp (c.rgb, _FogColor.rgb, i.fogAndLight.x);
			
			return c;
		}
		ENDCG 
	}	
}
}
