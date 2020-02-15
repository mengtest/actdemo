// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Test6" {  
    Properties {  
        _MainTex ("Base (RGB)", 2D) = "white" {}  
        _Float1("Float1", Float) = 0.5  
    }  
      
    SubShader {  
        Tags {"RenderType" = "Timeshift"}  
        PASS {  
            CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag  
  
            sampler2D _MainTex;  
            uniform float _Float1;  
  
            struct Input {  
                float4 pos : POSITION;  
                float4 normal : NORMAL;  
                float4 color : COLOR;  
            };  
              
            struct InputFrag {  
                float4 pos : SV_POSITION;  
                float4 color : COLOR;  
            };  
  
             InputFrag vert (Input i) {  
                InputFrag o;  
                o.pos = UnityObjectToClipPos (i.pos);  
                o.color = i.normal * -0.5 + 0.5;  
                return o;  
            }  
              
            float4 frag(InputFrag i) : COLOR{  
                return i.color;  
            }  
            ENDCG  
        }  
    }   
}  