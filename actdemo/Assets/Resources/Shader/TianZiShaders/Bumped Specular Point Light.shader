// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/******************************************************************************************************** 
 * 着色器 : 
 ********************************************************************************************************/
Shader "Snail/Bumped Specular Point Light" 
{
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1) 
		_Specular ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.5
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}

	SubShader {
		LOD 300
		Tags {"Queue"="Transparent"  "RenderType"="Opaque" }
	Pass { 
		Name "FORWARD"
		
		Cull Back	

		CGPROGRAM
		// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct appdata members fog)
#pragma exclude_renderers d3d11 xbox360
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			

			sampler2D _MainTex;
			sampler2D _BumpMap; 
			float4 _MainTex_ST;
			float4 _BumpMap_ST;			
			float4 _Color; 
			float4 _Specular;
			float _Shininess;
			
 
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
				};

			v2f vert (appdata_full v) {
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
   				
   				// Computes object space light direction
				TANGENT_SPACE_ROTATION;
				float3 objSpaceLightPos = mul(unity_WorldToObject, _worldSpaceLightPosition).xyz;
				objSpaceLightPos = objSpaceLightPos.xyz - v.vertex.xyz;
				
				float3 lightDir = mul (rotation, objSpaceLightPos);
				o.lightDir.rgb = lightDir;
				o.lightDir.a = dot(objSpaceLightPos, objSpaceLightPos);
				
				return o;
			}
			
			float4 frag (v2f IN) : COLOR {
				float2 mainUV = IN.pack0.xy;
				
				float4 c2 = tex2D(_MainTex, mainUV);

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