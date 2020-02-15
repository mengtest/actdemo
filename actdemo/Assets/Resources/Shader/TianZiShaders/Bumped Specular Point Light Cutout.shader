// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Snail/Bumped Specular Point Light Cutout" 
{
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_Specular ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.5
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		// 光源设置
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}

	SubShader {
		LOD 300

	Pass {
		Name "FORWARD"
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		Cull Back	

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
 
 			// To debug
			//#pragma debug

			sampler2D _MainTex;
			sampler2D _BumpMap; 
			float4 _MainTex_ST;
			float4 _BumpMap_ST;			
			float4 _Color; 
			float4 _Specular;
			float _Shininess;
			
			float _Cutoff;
 
 			// Light & Camera
			float4 _worldSpaceLightPosition;
			float4 _worldSpaceViewPos;
			float4 _lightColor;
			float _lightRange;
			float _lightIntensity;
 
			struct v2f {
					float4 pos : SV_POSITION;
					float4 pack0 : TEXCOORD0;
					float4 lightDir : TEXCOORD1;
  					float3 viewDir : TEXCOORD2;
				};

			v2f vert (appdata_full v) {
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
   				// Computes object space light direction
				float3 objSpaceLightPos = mul(unity_WorldToObject, _worldSpaceLightPosition).xyz;
				objSpaceLightPos = objSpaceLightPos.xyz - v.vertex.xyz;
				
				float3 binormal = cross( v.normal, v.tangent.xyz ) * v.tangent.w;
				float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal );

				float3 lightDir = mul (rotation, objSpaceLightPos);
				o.lightDir.rgb = lightDir;
				o.lightDir.a = dot(objSpaceLightPos, objSpaceLightPos);
			
				return o;
			}
			
			float4 frag (v2f IN) : COLOR {
				float2 mainUV = IN.pack0.xy;
				
				float4 c2 = tex2D(_MainTex, mainUV);
				clip (c2.a - _Cutoff);

				// Texture & Normal
				float4 albedo_alpha = c2 * _Color * 10;
				float3 Normal = normalize(UnpackNormal(tex2D(_BumpMap, mainUV)));
				
				// Blinn Phong
				float diff = max (0, dot (Normal, IN.lightDir.rgb));
				float atten = 1.0 / (1.0 + (25.0 * IN.lightDir.a / 1444));
				// atten = 0.0005;
				float4 c; 
				// albedo_alpha.rgb = 2;
				c.rgb = albedo_alpha.rgb * 0.3 + (albedo_alpha.rgb * _lightColor.rgb * _lightIntensity * diff ) * (atten * 2);
				c.rgb *= 0.6;
				c.a = albedo_alpha.a;
				 
				return c;
			}
		ENDCG 
		}
	}
	Fallback "VertexLit"
}