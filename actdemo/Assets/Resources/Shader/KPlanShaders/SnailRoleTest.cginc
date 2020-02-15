// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#ifndef SNAIL_ROLE_TEST
#define SNAIL_ROLE_TEST

#include "UnityCG.cginc"
#include "AutoLight.cginc"

struct appdata_xray
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
	float3 normal : NORMAL;
};

struct v2f_xray
{
	float2 uv : TEXCOORD0;
	float4 pos : SV_POSITION;
	half xrayFactor : TEXCOORD1;
};

struct appdata
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
	float3 normal : NORMAL;
#ifdef NORMAL_MAP
	float4 tangent : TANGENT; 
#endif // NORMAL_MAP
};

struct v2f
{
	float2 uv : TEXCOORD0;
	float4 pos : SV_POSITION;
#ifdef NORMAL_MAP
	float3 tlightDir : TEXCOORD1;
	float4 tSpace0 : TEXCOORD2;
	float4 tSpace1 : TEXCOORD3;
	float4 tSpace2 : TEXCOORD4;
#else
	float3 wlightDir : TEXCOORD1;
	float3 wNormal : TEXCOORD2;
#endif // NORMAL_MAP
#ifdef SPECULAR
	float4 wPos : TEXCOORD5;
#endif // SPECULAR
	UNITY_FOG_COORDS(6)
#ifdef SELF_SHADOW
	LIGHTING_COORDS(7,8)
#endif // SELF_SHADOW
};

sampler2D _MainTex;
float4 _MainTex_ST;
sampler2D _BumpMap;
sampler2D _SpecMap;
half4 _Color; 
half4 _BackFaceAmbient;
half4 _SelfShadowAmbient;
half _Intensity;
half _OverlayAdd;
uniform float3 lwp;
float _Glossiness;
float _Shininess;
float4 _LightOffset;

v2f_xray vert_xray (appdata_xray v)
{
	v2f_xray o;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = TRANSFORM_TEX(v.uv, _MainTex);

	float3 viewDir = ObjSpaceViewDir(v.vertex);
	viewDir = normalize(viewDir);
	float3 normal = normalize(v.normal);
	float xrayFactor = dot(normal, viewDir);
	xrayFactor = 1 - max(xrayFactor, 0) + 0.15;
	o.xrayFactor = xrayFactor;

	return o;
}

fixed4 frag_xray (v2f_xray i) : SV_Target
{
	fixed4 col = tex2D(_MainTex, i.uv) * i.xrayFactor;
	return col;
}

v2f vert (appdata v)
{
	v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);
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
#endif // NORMAL_MAP

#ifdef SPECULAR
	o.wPos = mul(unity_ObjectToWorld, v.vertex);
#endif // SPECULAR

	UNITY_TRANSFER_FOG(o,o.pos);

#ifdef SELF_SHADOW
	TRANSFER_VERTEX_TO_FRAGMENT(o)
#endif // SELF_SHADOW

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
#endif // NORMAL_MAP

#ifdef SPECULAR
	float3 dirToLight = normalize(_WorldSpaceCameraPos.xyz + _LightOffset.xyz - i.wPos);
	float3 reflectionV = normalize(reflect(-dirToLight, worldN));
	float3 dirToCam = normalize(_WorldSpaceCameraPos.xyz + _LightOffset.xyz - i.wPos);
	float4 spec = pow(saturate(dot(reflectionV, dirToCam)), _Glossiness * 128) * _Shininess;
#endif // SPECULAR

	fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				
	float diff = saturate(dot (normal, lightDir)); 

	col.rgb *= _Intensity * diff + _BackFaceAmbient.rgb * (1 - diff) + _OverlayAdd;
#ifdef SPECULAR
#ifdef SPECULAR_MAP
	col.rgb += spec * tex2D(_SpecMap, i.uv).rgb;
#else
	col.rgb += spec;
#endif // SPECULAR_MAP
#endif // SPECULAR

#ifdef SELF_SHADOW
	fixed3 attenColor = saturate(LIGHT_ATTENUATION(i) +  _SelfShadowAmbient.rgb);
	col.rgb *= attenColor;
#endif // SELF_SHADOW

	UNITY_APPLY_FOG(i.fogCoord, col);

	return col;
}

#endif // SNAIL_ROLE_TEST