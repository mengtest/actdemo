// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "BakeNormalSpecularToDiffuse"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_BackFaceAmbient ("BackFaceAmbient", Color) = (0.5,0.5,0.5,1)
		_BackFaceAmbientIntensity ("BackFaceAmbient Intensity", Range (-5, 5)) = 0
		_Intensity ("Intensity", Range (0,3)) = 1
		_OverlayAdd("Overlay Add", Range(-2, 2)) = 0
		_Shininess ("Shininess", Range (0, 10)) = 1
		_Glossiness ("Glossiness", Range (0.01, 1)) = 0.1
		_LightOffset("Offset Light Pos", Vector) = (-0.25, 0.45, 0, 0)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SpecMap ("Specularmap", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ NORMAL_MAP
			#pragma multi_compile _ SPECULAR
			#pragma multi_compile _ SPECULAR_MAP
			#pragma multi_compile _ SCREEN_SPACE
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
#ifdef NORMAL_MAP
				float4 tangent : TANGENT; 
#endif
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
#ifdef NORMAL_MAP
				float3 tlightDir : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
#else
				float3 wlightDir : TEXCOORD1;
				float3 wNormal : TEXCOORD2;
#endif
#ifdef SPECULAR
				float4 wPos : TEXCOORD5;
#endif
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BumpMap;
			sampler2D _SpecMap;
			half4 _Color; 
			half4 _BackFaceAmbient;
			half _Intensity;
			half _BackFaceAmbientIntensity;
			half _OverlayAdd;
			uniform float3 lwp;
			float _Glossiness;
			float _Shininess;
			float4 _LightOffset;
			
			v2f vert (appdata v)
			{
				v2f o;
#ifdef SCREEN_SPACE
				#if ((defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)) && defined(SHADER_API_MOBILE)) || defined(SHADER_API_OPENGL)
					o.vertex.xy = v.uv * 2 - 1;
				#else
					o.vertex.xy = v.uv * float2(2, -2) + float2(-1, 1);
				#endif
				o.vertex.z = 0;
				o.vertex.w = 1; 
#else
				o.vertex = UnityObjectToClipPos(v.vertex);
#endif

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

#ifdef NORMAL_MAP
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
				o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
				
				float3 binormal = cross( v.normal, v.tangent.xyz ) * v.tangent.w;  
				float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal ); 

				float3 objSpaceLightPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos + _LightOffset.xyz, 1)).xyz;
				objSpaceLightPos = objSpaceLightPos.xyz - v.vertex.xyz;
				o.tlightDir = normalize(mul (rotation, objSpaceLightPos));
#else
				o.wlightDir = normalize(_WorldSpaceCameraPos + _LightOffset.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
				o.wNormal = normalize(mul(unity_ObjectToWorld, float4(v.normal, 0)));
#endif

#ifdef SPECULAR
				o.wPos = mul(unity_ObjectToWorld, v.vertex);
#endif

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 lightDir;
				half3 normal;
				half3 worldN;

#ifdef NORMAL_MAP
				normal = normalize(UnpackNormal(tex2D(_BumpMap, i.uv)));
				worldN.x = dot(i.tSpace0.xyz, normal);
				worldN.y = dot(i.tSpace1.xyz, normal);
				worldN.z = dot(i.tSpace2.xyz, normal);
				lightDir = i.tlightDir;
#else
				normal = i.wNormal;
				worldN = i.wNormal;
				lightDir = i.wlightDir;
#endif

#ifdef SPECULAR
				float3 dirToLight = normalize(_WorldSpaceCameraPos.xyz + _LightOffset.xyz - i.wPos);
				float3 reflectionV = normalize(reflect(-dirToLight, worldN));
				float3 dirToCam = normalize(_WorldSpaceCameraPos.xyz + _LightOffset.xyz - i.wPos);
				float4 spec = pow(saturate(dot(reflectionV, dirToCam)), _Glossiness * 128) * _Shininess;
#endif

				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				
				float diff = saturate(dot (normal, lightDir)); 

				col.rgb *= _Intensity * diff + (_BackFaceAmbient.rgb + _BackFaceAmbientIntensity) * (1 - diff) + _OverlayAdd;
#ifdef SPECULAR
	#ifdef SPECULAR_MAP
				col.rgb += spec * tex2D(_SpecMap, i.uv).rgb;
	#else
				col.rgb += spec;
	#endif
#endif
	
				return col;
			}
			ENDCG
		}
	}

	CustomEditor "BakeNormalSpecularToDiffuseShaderGUI"
}
