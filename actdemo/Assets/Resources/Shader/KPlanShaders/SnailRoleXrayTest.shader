Shader "SnailRoleXrayTest"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_BackFaceAmbient ("BackFaceAmbient Color", Color) = (0.5,0.5,0.5,1)
		_SelfShadowAmbient ("SelfShadowAmbient Color", Color) = (0.5,0.5,0.5,1)
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
		Tags { "Queue"="AlphaTest+1" }

		Pass
		{
			Blend One One
			ZWrite Off
			ZTest Greater
			Cull Back

			CGPROGRAM
			#pragma vertex vert_xray
			#pragma fragment frag_xray

			#include "SnailRoleTest.cginc"
			ENDCG
		}

		Pass
		{
			Name "FORWARD"
            Tags { "LightMode"="ForwardBase" }

			Blend One Zero
			ZWrite On
			ZTest LEqual
			Cull Back

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase_fullshadows
			#pragma multi_compile _ NORMAL_MAP
			#pragma multi_compile _ SPECULAR
			#pragma multi_compile _ SPECULAR_MAP
			#pragma multi_compile _ SELF_SHADOW
			#pragma target 3.0

			#include "SnailRoleTest.cginc"
			ENDCG
		}
	}

	FallBack "Diffuse"
	CustomEditor "SnailRoleTestShaderGUI"
}
