// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/BlendModesOverlay" {
	Properties {
		_MainTex ("Screen Blended", 2D) = "" {}
		_Overlay ("Color", 2D) = "grey" {}
	}
	
	CGINCLUDE

	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv[2] : TEXCOORD0;
	};
			
	sampler2D _Overlay;
	sampler2D _MainTex;
	
	half _Intensity;
	half4 _MainTex_TexelSize;
	half4 _UV_Transform = half4(1, 0, 0, 1);
		
	v2f vert( appdata_img v ) { 
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		
		o.uv[0] = float2(
			dot(v.texcoord.xy, _UV_Transform.xy),
			dot(v.texcoord.xy, _UV_Transform.zw)
		);
		
		#if UNITY_UV_STARTS_AT_TOP
		if(_MainTex_TexelSize.y<0.0)
			o.uv[0].y = 1.0-o.uv[0].y;
		#endif
		
		o.uv[1] =  v.texcoord.xy;	
		return o;
	}

	half4 fragMultiply (v2f i) : COLOR {
		return (tex2D(_MainTex, i.uv[1]) * tex2D(_Overlay, i.uv[0])) * _Intensity;
	}

	ENDCG 
	
Subshader {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }  
      ColorMask RGB	

 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragMultiply
      ENDCG
  }  
}

Fallback off
	
} // shader