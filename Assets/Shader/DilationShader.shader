Shader "Custom/NewImageEffectShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
		_DilationCoefficent ("DilationCoefficent", Range(-1, 1)) = 0.5
		//_Intensity ("Intensity", Range(0, 50)) = 0
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        //[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
				//float3 localPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            fixed4 _Color;
			half _DilationCoefficent;
            //fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            v2f vert(appdata_t v)
            {
                v2f OUT;

                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

			#define R_MAX 0.85

			float f(float x,float alpha){
			  if(x>R_MAX) return x; // pas de déformation au delà de R_MAX

			  if(alpha>=0)
				return (1-alpha) * x  + alpha *x*x*x / R_MAX / R_MAX; 
			  else
				return (alpha+1) * x - alpha * pow( x / R_MAX, 0.33) * R_MAX;
			}


            fixed4 frag(v2f IN) : SV_Target
            {
				//Calculate relative position
				//fixed2 relativeWorld = fixed2(IN.worldPosition.x + IN.localPos.x, IN.worldPosition.y + IN.localPos.y);
   
				//This becomes the UV for the texture I want to apply to the sprite ( using the sprites width and height )
				//fixed2 relativePos = fixed2((relativeWorld.x + _Width), (relativeWorld.y + _Height));
  

				float r,a;
				float2 pos,texcoord;
				
				pos= 2 * IN.texcoord - float2(1,1);
				r=sqrt( (pos.x)*(pos.x) + (pos.y)*(pos.y) );
				//if(r>R_MAX) r=R_MAX;
				a= f(r, _DilationCoefficent); //r*( (1-_DilationCoefficent) + _DilationCoefficent * r * r );
				/*texcoord.x= IN.texcoord.x - 0.5f + 0.5f;

				r=2*IN.texcoord.y - 1;
				a= f(r, _DilationCoefficent); //a=r*( 1-_DilationCoefficent + _DilationCoefficent * r * r );
				texcoord.y= a *0.5f + 0.5f;
				*/
				//texcoord.x= IN.texcoord.x+_DilationCoefficent;
				//texcoord.y=IN.texcoord.y;
				
				//texcoord = float2(0.5,0.5) + 0.5 * pos * (1 + (r-R_MAX)*(r-R_MAX)*_DilationCoefficent);
				texcoord = float2(0.5,0.5) +  0.5* pos * a/r;

                half4 color = (tex2D(_MainTex, texcoord )) * IN.color;

                return color;
            }
        ENDCG
        }
    }
}
