// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/MobilePaticleBlend" {
	Properties {
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	Cull Off 
	Lighting Off 
	ZWrite Off 
	Fog { Color (0,0,0,0) }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  Pass {
		
CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

fixed4 _TintColor;
sampler2D _MainTex;

struct appdata_t {
	float4 vertex : POSITION;
	fixed4 color : COLOR;
	float2 texcoord : TEXCOORD0;
};

struct v2f {
    float4  pos : SV_POSITION;
	fixed4 color : COLOR;
    float2  uv : TEXCOORD0;
};

float4 _MainTex_ST;

v2f vert (appdata_t v)
{
    v2f o;
    o.pos = UnityObjectToClipPos (v.vertex);
    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
	o.color = v.color;
    return o;
}

half4 frag (v2f i) : COLOR
{
    half4 texcol = tex2D (_MainTex, i.uv);
    return 2.0h  * i.color *  texcol * _TintColor;
}
ENDCG
		}
	} 
	FallBack "Diffuse"
}
