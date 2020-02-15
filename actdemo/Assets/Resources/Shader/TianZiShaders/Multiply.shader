// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "AngryBots/FX/Multiply" {
	Properties {
		_MainTex ("Base", 2D) = "white" {}
	}
	
	CGINCLUDE
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members fog)
#pragma exclude_renderers d3d11 xbox360

		#include "UnityCG.cginc"

		sampler2D _MainTex;
						
			float4 _FogColor;
			float4 _FogParam;
			float4 _FogIntensity;

		struct v2f {
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			float3 fog;
		};

		v2f vert(appdata_full v) {
			v2f o;
			
			o.pos = UnityObjectToClipPos (v.vertex);	
			o.uv.xy = v.texcoord.xy;
					

				// æ‡¿ÎŒÌ
				float4 wsPos = mul (unity_ObjectToWorld, v.vertex);
				float3 camDir = wsPos.xyz - _WorldSpaceCameraPos;
				float fogInt = saturate(length(camDir) * (1 / _FogParam.x) - 1.0) * (100-_FogParam.x);	

				// ∏ﬂ∂»ŒÌ
				float fogVert = max(0.0, (wsPos.y - _FogParam.w) * (1 / _FogParam.z));
				fogVert *= fogVert;
				fogVert = (exp (-fogVert));

				float intensity = (1-exp(-_FogParam.y*fogInt*0.01)) * fogVert;

				//float intensity = (1-exp(-_FogParam.y*fogInt*0.01));
				o.fog = _FogIntensity.xyz * intensity;


			return o; 
		}
		
		fixed4 frag( v2f i ) : COLOR {	
			//return tex2D (_MainTex, i.uv.xy);
			float4 c =  tex2D(_MainTex, i.uv);
			//c.rgb = lerp (c.rgb, _FogColor.rgb, i.fog.xyz);
			return c;
		}
	
	ENDCG
	
	SubShader {
		Tags {"IgnoreProjector"="True" "RenderType"="Transparent"}
		Cull Off
		Lighting Off
		ZTest  On  
		ZWrite Off
		Fog { Mode Off }
		Blend Zero SrcColor
		
	Pass {
	
		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG
		 
		}
				
	} 
	FallBack Off
}
