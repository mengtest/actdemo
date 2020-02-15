// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "AnimatedVegetation" {
	Properties {
		[Enum(UnityEngine.Rendering.CullMode)] _CullMode("Cull Mode", Float) = 2
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		_WaveRange("Wave Range", Range(0.0, 5)) = 1
		_WaveFrequency("Wave Frequency", Range(0.0, 5)) = 1
		_WaveOffset("Wave Offset", Range(-5, 5.0)) = 0
	}
	SubShader {
		Tags { "Queue"="AlphaTest" }
		LOD 200
		Cull [_CullMode]
		
		CGPROGRAM
		#pragma surface surf StandardSpecular vertex:vert fullforwardshadows addshadow exclude_path:prepass exclude_path:deferred noforwardadd nodynlightmap nodirlightmap
		#pragma only_renderers d3d9 d3d11 d3d11_9x opengl gles gles3 metal
		#pragma multi_compile _ IS_GLOBAL_WIND
		#pragma multi_compile _ ALPHA_TEST_ENABLED
		#pragma multi_compile _ FORCE_ENABLED
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input 
		{
			float2 uv_MainTex : TEXCOORD1;
		};

		fixed4 _Color;
		half _WaveRange;
		half _WaveFrequency;
		half _WaveOffset;
		half _Cutoff;
		uniform half3 _Force = half3(0, 0, 0);

		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			float4 vPos = mul(unity_ObjectToWorld, v.vertex);
			float rnd = vPos.x + vPos.y + vPos.z;
			half4 weight = v.color;
			float4 offset = float4(0, 0, 0, 0);
#ifdef FORCE_ENABLED
			offset.xyz += weight.xyz * _WaveRange * (sin((_Time.y + rnd) * _WaveFrequency) + _WaveOffset) + weight.a * _Force;
#else
			offset.xyz += weight.xyz * _WaveRange * (sin((_Time.y + rnd) * _WaveFrequency) + _WaveOffset);
#endif
#ifdef IS_GLOBAL_WIND
			offset = mul(unity_WorldToObject, offset);
#endif
			v.vertex.xyz += offset.xyz;
		}

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

#ifdef ALPHA_TEST_ENABLED
			clip(c.a - _Cutoff);
#endif

			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
