// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Snail/TopGradualColor" 
{
	Properties {
		_MainTex ("Base (RGB)", RECT) = "white" {}
		_GradualColor ("Gradual Color", Color) = (1.0,0,0,1)

		_fGradualStart("GradualStart", Float) = 0.5
		_fGradualEnd("GradualEnd", Float) = 1.5
		_fGradualExp("GradualExp", Float) = 0.5
		// 色彩调整:亮度，对比度，饱和度
		_vColorAdjustParam ("ColorAdjustParam", Color) = (11.5,11.5,110.005,1)
		// 基色
		_GradualBaseColor ("GradualBaseColor", Color) = (1.5,1.5,1.5,1)

	}

    SubShader {
        Pass {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f_surf members scrPos)
#pragma exclude_renderers d3d11 xbox360
            #pragma vertex myVert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

			sampler2D _MainTex;

			float4	_GradualColor;
			float	_fGradualStart;
			float	_fGradualEnd;
			float	_fGradualExp;

			// 色彩调整:亮度，对比度，饱和度
			float3	_vColorAdjustParam;
			// 基色
			float4	_GradualBaseColor;

			struct myappdata_img {
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;	
				float4 tangent : TANGENT;
			};				
			struct myv2f_img {
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
				half3 viewDir : TEXCOORD1;
			};

			float2 myMultiplyUV (float4x4 mat, float2 inUV) {
				float4 temp = float4 (inUV.x, inUV.y, 0, 0);
				temp = mul (mat, temp);
				return temp.xy;
			}

			myv2f_img myVert_img( myappdata_img v )
			{
				myv2f_img o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv = myMultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord );

				TANGENT_SPACE_ROTATION;
				o.viewDir = normalize(mul( rotation, ObjSpaceViewDir( v.vertex )));    
				o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
				return o;
			}


			float3 ColorAdjust(float3 scolor, float3 param, float3 basecolor)
			{
				float brightness = param.x;                    // 亮度
			    float contrast = param.y;                      // 对比度
			    float saturation = param.z;                    // 饱和度
    
				float ave = (scolor.r + scolor.g + scolor.b) / 3;
				float crTempC = pow(ave, contrast);
				float3 crTempS = lerp(float3(ave), scolor.rgb * crTempC, saturation) * brightness;
				return crTempS * basecolor; 
			}
			
			float4 frag (myv2f_img i) : COLOR 
            {
				float weight = 1.0 - (i.uv.y - _fGradualStart) / (_fGradualEnd - _fGradualStart);
				weight += saturate(i.viewDir.y);
				weight = saturate(weight);
				weight = pow(weight, _fGradualExp);
				
                float4 original = tex2D(_MainTex, i.uv);
				original += ((_GradualColor - 0.5) * 2.0 * weight) * (1.0 - saturate(-i.viewDir.y));
				
				float4 output = original;
				output.rgb = ColorAdjust(output.rgb, _vColorAdjustParam, _GradualBaseColor);
				return output;
            }

            ENDCG
        }
    }
}
