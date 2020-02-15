// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

/*********************************************************************************************
 * 着色器 : 纹理 + 烘焙贴图
 *************************************************************************************************/
Shader "Snail/Texture-Lightmap" {
    Properties {
	    _Color ("Main Color", Color) = (1,1,1,1)
	    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }
	SubShader {
	    Tags { "RenderType"="Opaque" }
	    LOD 200
		
		Pass {
		    Cull Off
		
		    CGPROGRAM
		    #pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precesion_hint_fastest
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			// uniform sampler2D unity_Lightmap;
			// uniform half4 unity_LightmapST;
			uniform fixed4 _Color;
			uniform half _Cutoff;
			
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
				fixed3 lm = DecodeLightmap( UNITY_SAMPLE_TEX2D( unity_Lightmap, i.uv1 ) );
				tex.rgb *= lm;
				#else
				tex.rgb *= 0.65;
				#endif
				
				return tex;
			}

		    ENDCG		
		}
	} 
	FallBack "Diffuse"
}
