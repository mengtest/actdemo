// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Custom/Diffuse-LightMap" {
	Properties {
	    _Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100
		
		Pass {
		    CGPROGRAM
		    #pragma vertex vert
		    #pragma fragment frag
		    #pragma fragmentoption ARB_precision_hint_fastest 
		    #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON			
			#include "UnityCG.cginc"

		    sampler2D _MainTex;
			half4 _MainTex_ST;
		    fixed4 _Color;
			// half4 unity_LightmapST;
			// sampler2D unity_Lightmap;

		    struct v2f {
		        half4 pos : SV_POSITION;
			    half2 uv : TEXCOORD0;
			    half2 uv1 : TEXCOORD1;
		    };
			
			v2f vert( appdata_full v ) {
			    v2f o = (v2f)0;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = TRANSFORM_TEX( v.texcoord, _MainTex );
				o.uv1 = v.texcoord1 * unity_LightmapST.xy + unity_LightmapST.zw;
				return o;
			}
			
			fixed4 frag( v2f i ) : COLOR {
			    fixed4 tex = tex2D( _MainTex, i.uv ) * _Color;
				#ifdef LIGHTMAP_ON 
				fixed3 lm = DecodeLightmap( UNITY_SAMPLE_TEX2D( unity_Lightmap, i.uv1 ) ) ;
				tex.rgb *= lm;
				//#else
				//tex.rgb *= 0;	
				#endif
				
				return tex;
			}
			ENDCG
        }
	} 
	FallBack "Diffuse"
}
