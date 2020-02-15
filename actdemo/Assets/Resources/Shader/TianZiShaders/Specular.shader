// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/***************************************************************************************************
 * 功能 : 高光着色器
 ****************************************************************************************************/
Shader "Snail/MobileSpecular" {

Properties {
	_MainTex ("基础纹理", 2D) = "white" {}
	_Color ("自发光颜色", Color) = (.5,.5,.5,1)
	_Shininess ("高光强度", Range (0.01, 1)) = 0.078125
	_SHLightingScale("高光范围",float) = 1
	_SpecularColor ("高光颜色", Color) = (.5,.5,.5,1)
	_RimColor ("边缘光颜色", Color) = (0.26,0.19,0.16,0.0)
	_RimPower ("边缘光范围", Range(0.5,8.0)) = 3.0
}

SubShader {
	Tags { "RenderType"="Opaque" }
	Cull Off
	
	CGINCLUDE
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members viewDir)
#pragma exclude_renderers d3d11 xbox360
	#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest		
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float4 _Color;
	float4 _SpecularColor;
	float4 _RimColor;
	float3 _SpecDir;
	float3 _SpecColor;
	
	float _Shininess;
	float _SHLightingScale;
	float _RimPower;

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		fixed3 spec : TEXCOORD1;
		fixed3 SHLighting: TEXCOORD2;
	
		float3 cubenormal : TEXCOORD3;
		float3 viewDir;

		float3 test;
	};
	
	v2f vert (appdata_full v)
	{
		v2f o;
		// 输出透视投影坐标到裁剪空间 
		o.pos = UnityObjectToClipPos(v.vertex);

		o.uv = v.texcoord;
		float3 worldNormal = mul((float3x3)unity_ObjectToWorld, v.normal);
		float3 worldV = normalize(-WorldSpaceViewDir(v.vertex));
		float3 refl = reflect(worldV, worldNormal);
		float3 shl = ShadeSH9(float4(worldNormal,1));
		
		float3 worldLightDir = _WorldSpaceLightPos0;
		
		o.test = 1;
		/**
		

		o.spec = normalize(shl) * pow(saturate(dot(worldLightDir, refl)), _Shininess * 128) * 2;
		
		o.SHLighting	= shl * _SHLightingScale;
		
		o.cubenormal = mul (UNITY_MATRIX_MV, float4(v.normal,0));

		o.viewDir = worldV;**/
		return o;
	}

	fixed4 frag (v2f i) : COLOR
	{
		fixed4 col;//	= tex2D (_MainTex, i.uv) * _Color * 10;

		col.rgb = i.test;
		col.a = 1;
		return col;

		//col.rgb *= i.SHLighting;
		//col.rgb += i.spec.rgb * col.a * _SpecularColor.rgb;

		//half rim = 1.0 - saturate(dot (normalize(i.viewDir), i.cubenormal));
		//col.rgb = col.rgb + _RimColor.rgb * pow (rim, _RimPower);
		//return col;
	}
	ENDCG



}
}

