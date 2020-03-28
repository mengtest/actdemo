// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Snail/Bumped Specular Point Light Cutout Add" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1) 
		_Specular ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.5
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		// 光源设置
		
	}

	SubShader {
		LOD 300

	Pass {
		Name "FORWARD"
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		Blend One One  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdbase novertexlight
			#include "HLSLSupport.cginc"
			#define UNITY_PASS_FORWARDBASE
			#include "UnityCG.cginc"
			#include "Lighting.cginc"  
			#include "AutoLight.cginc"
			#define INTERNAL_DATA
 
 			// To //Debug
			//#pragma //Debug

			sampler2D _MainTex;
			sampler2D _BumpMap; 
			half4 _MainTex_ST;
			half4 _BumpMap_ST;			
			half4 _Color; 
			half4 _Specular;
			half _Shininess;
			
			half _Cutoff;
 
 			// Light & Camera
			uniform half4 _worldSpaceLightPosition;
			uniform half4 _worldSpaceViewPos;
			uniform half4 _lightColor;
			uniform half _lightRange;
			uniform half _lightIntensity;

 
			#ifdef LIGHTMAP_OFF
				struct v2f {
					float4 pos : SV_POSITION;
					half4 pack0 : TEXCOORD0;
					half4 lightDir : TEXCOORD1;
  					half3 viewDir : TEXCOORD2;
					LIGHTING_COORDS(3,4)
				};
			#endif
			
			#ifndef LIGHTMAP_OFF
				struct v2f {
					float4 pos : SV_POSITION;
					half4 pack0 : TEXCOORD0;
					half2 lmap : TEXCOORD1;
					half4 lightDir : TEXCOORD2;
  					half3 viewDir : TEXCOORD3;
					LIGHTING_COORDS(4,5)
				};
				// sampler2D unity_Lightmap;
				// half4 unity_LightmapST;	
			#endif

			v2f vert (appdata_full v) {
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.pack0.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);
   
   				// Computes object space light direction
				TANGENT_SPACE_ROTATION;
				fixed3 objSpaceLightPos = mul(unity_WorldToObject, _worldSpaceLightPosition).xyz;
				objSpaceLightPos = objSpaceLightPos.xyz - v.vertex.xyz;
				
				fixed3 lightDir = mul (rotation, objSpaceLightPos);
				o.lightDir.rgb = lightDir;
				o.lightDir.a = dot(objSpaceLightPos, objSpaceLightPos);
				
				// Computes object space view direction
				fixed3 objSpaceCameraPos = mul(unity_WorldToObject, fixed4(_worldSpaceViewPos.xyz, 1)).xyz * 1.0;
				objSpaceCameraPos = objSpaceCameraPos - v.vertex.xyz;
				fixed3 viewDirForLight = mul (rotation, objSpaceCameraPos);
				o.viewDir = viewDirForLight;		
   				
   				// Lightmap coordinates
				#ifndef LIGHTMAP_OFF
					o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif 
				
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}
			
			fixed4 frag (v2f IN) : COLOR {
				half2 uv_MainTex = IN.pack0.xy;
				half2 uv_BumpMap = IN.pack0.zw; 
				
				half4 c2 = tex2D(_MainTex, uv_MainTex);
				clip (c2.a - _Cutoff);
				
				// Texture & Normal
				half4 albedo_alpha = c2 * _Color * 10;
				half3 Normal = normalize(UnpackNormal(tex2D(_BumpMap, uv_BumpMap)));
				
				// Blinn Phong
				half3 h = normalize (IN.lightDir.rgb + IN.viewDir);
				half diff = max (0, dot (Normal, IN.lightDir.rgb));
				half nh = max (0, dot (Normal, h));
				half spec = pow (nh, _Shininess*128.0f) * albedo_alpha.a;
				half atten = 1.0f / (1.0f + (25.0f * IN.lightDir.a / (19.7* 19.7)));
				atten = 0.001;
				half4 c;
				// albedo_alpha.rgb = 2;
				c.rgb = albedo_alpha.rgb * 0.3 + (albedo_alpha.rgb * _lightColor.rgb * _lightIntensity * diff + _lightColor.rgb * _lightIntensity *  _Specular.rgb * spec) * (atten * 2);
				c.rgb *= 0.6;
				c.a = albedo_alpha.a + _lightColor.a * _Specular.a * spec * atten;
				 c.a = 0.5;
				#ifndef LIGHTMAP_OFF
					half4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy);
					half3 lm = DecodeLightmap (lmtex);
					c.rgb += albedo_alpha.rgb * lm;
					c.a += albedo_alpha.a;
				#endif // LIGHTMAP_OFF
				
				return c;
			}
		ENDCG 
		}
	}
}