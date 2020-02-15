// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*********************************************************************************************************
 * 着色器 ：河流渲染
 **********************************************************************************************************/
Shader "Snail/River" {
Properties {
	_MainTex ("Base layer (RGB)", 2D) = "white" {}
	_DetailTex ("2nd layer (RGB)", 2D) = "white" {}
	_ScrollX ("层1 X方向滚动速度", Float) = 1.0
	_ScrollY ("层1 Y方向滚动速度", Float) = 0.0
	_Scroll2X ("层2 X方向滚动速度", Float) = 1.0
	_Scroll2Y ("层2 Y方向滚动速度", Float) = 0.0
	_SineAmplX ("层1 正弦振幅 X",Float) = 0.5 
	_SineAmplY ("层1 正弦振幅 Y",Float) = 0.5
	_SineFreqX ("层1 正弦频率 X",Float) = 10 
	_SineFreqY ("层1 正弦频率 Y",Float) = 10

	_SineAmplX2 ("层2 正弦振幅 X",Float) = 0.5 
	_SineAmplY2 ("层2 正弦振幅 Y",Float) = 0.5
	_SineFreqX2  ("层2 正弦频率 X",Float) = 10 
	_SineFreqY2 ("层2 正弦频率 Y",Float) = 10

	_Color1("Color1", Color) = (1,1,1,1)
	_Color2("Color2", Color) = (1,1,1,1)
	
	_MMultiplier ("颜色增强值", Float) = 2.0
}

	
SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off Lighting Off ZWrite Off 
	
	LOD 100
	
	
	
	CGINCLUDE
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members color)
#pragma exclude_renderers d3d11 xbox360
	#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
	#pragma exclude_renderers molehill    
	#include "UnityCG.cginc"
	sampler2D _MainTex;
	sampler2D _DetailTex;

	float4 _MainTex_ST;
	float4 _DetailTex_ST;
	
	float _ScrollX;
	float _ScrollY;
	float _Scroll2X;
	float _Scroll2Y;
	
	float _SineAmplX;
	float _SineAmplY;
	float _SineFreqX;
	float _SineFreqY;

	float _SineAmplX2;
	float _SineAmplY2;
	float _SineFreqX2;
	float _SineFreqY2;
	float4 _Color1;
	float4 _Color2;
	
	struct v2f {
		float4 pos : SV_POSITION;
		float4 uv : TEXCOORD0;
		float4 color;
	};

	
	v2f vert (appdata_full v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex) + frac(float2(_ScrollX, _ScrollY) * _Time);
		o.uv.zw = TRANSFORM_TEX(v.texcoord.xy,_DetailTex) + frac(float2(_Scroll2X, _Scroll2Y) * _Time);
		
		o.uv.x += sin(_Time * _SineFreqX) * _SineAmplX;
		o.uv.y += sin(_Time * _SineFreqY) * _SineAmplY;
		
		o.uv.z += sin(_Time * _SineFreqX2) * _SineAmplX2;
		o.uv.w += sin(_Time * _SineFreqY2) * _SineAmplY2;

		o.color = v.color;
		
		return o;
	}
	ENDCG


	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
//		#pragma fragmentoption ARB_precision_hint_fastest		
		fixed4 frag (v2f i) : COLOR
		{
			fixed4 o;
			fixed4 tex = tex2D (_MainTex, i.uv.xy);
			fixed4 tex2 = tex2D (_DetailTex, i.uv.zw);
			
			o = tex * _Color1 * i.color + tex2 * _Color2 * i.color;
						
			return o;
		}
		ENDCG 
	}	
}
}
