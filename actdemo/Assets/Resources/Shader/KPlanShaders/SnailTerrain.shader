// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SnailTerrain" {
	Properties {
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
		_Color ("Color", Color) = (1, 1, 1, 1)
		_OffsetLightDir("Offset Light Dir", Vector) = (0, 0, 0, 0)

		// set by terrain engine
		_Control ("Control (RGBA)", 2D) = "red" {}
		_Splat0 ("Layer 0 (R)", 2D) = "white" {}
		_Splat1 ("Layer 1 (G)", 2D) = "white" {}
		_Splat2 ("Layer 2 (B)", 2D) = "white" {}
		_Splat3 ("Layer 3 (A)", 2D) = "white" {}
		
		_Normal0 ("Normal 0 (R)", 2D) = "bump" {}
		_Normal1 ("Normal 1 (G)", 2D) = "bump" {}
		_Normal2 ("Normal 2 (G)", 2D) = "bump" {}
		_Normal3 ("Normal 3 (G)", 2D) = "bump" {}
	}

	SubShader {
		Tags {
			"SplatCount" = "4"
			"Queue" = "Geometry-100"
			"RenderType" = "Opaque"
		}

		CGPROGRAM
		#pragma surface surf SnailBlinnPhong vertex:SnailSplatmapVert finalcolor:SnailSplatmapFinalColor exclude_path:prepass exclude_path:deferred noforwardadd nodynlightmap nodirlightmap
		#pragma multi_compile_fog
		#pragma only_renderers d3d9 d3d11 d3d11_9x opengl gles gles3 metal
		#pragma target 3.0

		half3 _OffsetLightDir;
		fixed4 _Color;
		half _Shininess;
		sampler2D _Control;
		float4 _Control_ST;
		sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
		sampler2D _Normal0, _Normal1, _Normal2, _Normal3;

		struct Input
		{
			float2 uv_Splat0 : TEXCOORD0;
			float2 uv_Splat2 : TEXCOORD2;
			float2 uv_Splat1 : TEXCOORD1;
			float2 uv_Splat3 : TEXCOORD3;
			float2 tc_Control : TEXCOORD4;	// Not prefixing '_Contorl' with 'uv' allows a tighter packing of interpolators, which is necessary to support directional lightmap.
			UNITY_FOG_COORDS(5)
		};

		inline void LightingSnailBlinnPhong_GI (
			SurfaceOutput s,
			UnityGIInput data,
			inout UnityGI gi)
		{
			gi = UnityGlobalIllumination (data, 1.0, s.Normal);
		}

		inline fixed4 SnailBlinnPhongLight (SurfaceOutput s, half3 viewDir, UnityLight light)
		{
			half3 lightDir = light.dir;
			lightDir += _OffsetLightDir;
			half3 h = normalize (normalize(lightDir) + viewDir);
	
			fixed diff = max (0, dot (s.Normal, light.dir));
	
			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, s.Specular*128.0) * s.Gloss;
	
			fixed4 c;
			c.rgb = s.Albedo * light.color * diff + light.color * _SpecColor.rgb * spec;
			c.a = s.Alpha;

			return c;
		}

		inline fixed4 LightingSnailBlinnPhong (SurfaceOutput s, half3 viewDir, UnityGI gi)
		{
			fixed4 c;
			c = SnailBlinnPhongLight (s, viewDir, gi.light);

			#if defined(DIRLIGHTMAP_SEPARATE)
				#ifdef LIGHTMAP_ON
					c += UnityBlinnPhongLight (s, viewDir, gi.light2);
				#endif
				#ifdef DYNAMICLIGHTMAP_ON
					c += UnityBlinnPhongLight (s, viewDir, gi.light3);
				#endif
			#endif

			#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
				c.rgb += s.Albedo * gi.indirect.diffuse;
			#endif

			return c;
		}

		void SnailSplatmapFinalColor(Input IN, SurfaceOutput o, inout fixed4 color)
		{
			color *= o.Alpha;
			color.rgb *= _Color.rgb;
			UNITY_APPLY_FOG(IN.fogCoord, color);
		}

		void SnailSplatmapVert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			data.tc_Control = TRANSFORM_TEX(v.texcoord, _Control);	// Need to manually transform uv here, as we choose not to use 'uv' prefix for this texcoord.
			float4 pos = UnityObjectToClipPos (v.vertex);
			UNITY_TRANSFER_FOG(data, pos);

			v.tangent.xyz = cross(v.normal, float3(0,0,1));
			v.tangent.w = -1;
		}

		void SnailSplatmapMix(Input IN, out half4 splat_control, out fixed4 mixedDiffuse, inout fixed3 mixedNormal)
		{
			splat_control = tex2D(_Control, IN.tc_Control);

			mixedDiffuse = 0.0f;
			
			mixedDiffuse += splat_control.r * tex2D(_Splat0, IN.uv_Splat0);
			mixedDiffuse += splat_control.g * tex2D(_Splat1, IN.uv_Splat1);
			mixedDiffuse += splat_control.b * tex2D(_Splat2, IN.uv_Splat2);
			mixedDiffuse += splat_control.a * tex2D(_Splat3, IN.uv_Splat3);


			fixed3 lay1B = UnpackNormal (tex2D(_Normal0, IN.uv_Splat0));
			fixed3 lay2B = UnpackNormal (tex2D(_Normal1, IN.uv_Splat1));
			fixed3 lay3B = UnpackNormal (tex2D(_Normal2, IN.uv_Splat2));
			fixed3 lay4B = UnpackNormal (tex2D(_Normal3, IN.uv_Splat3));
			mixedNormal = (lay1B * splat_control.r + lay2B * splat_control.g + lay3B * splat_control.b + lay4B * splat_control.a);
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			half4 splat_control;
			fixed4 mixedDiffuse;
			SnailSplatmapMix(IN, splat_control, mixedDiffuse, o.Normal);
			o.Albedo = mixedDiffuse.rgb;
			o.Alpha = 1;
			o.Gloss = mixedDiffuse.a;
			o.Specular = _Shininess;
		}
		ENDCG
	}

	Fallback "Nature/Terrain/Diffuse"
}
