Shader "Custom/UnitShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Sprite (RG)", 2D) = "white" {}
        _PalleteTex ("Unit Pallete (RGB)", 2D) = "white" {}
        _PlayerTex ("Player Colors Pallete (RGB)", 2D) = "white" {}
		_PlayerIndex ("Player Number", int) = 0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

		Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

			float4 _Color;
			float4 _MainTex_ST;
            sampler2D _MainTex;
			sampler2D _PalleteTex;
			sampler2D _PlayerTex;
			int _PlayerIndex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float2 texel = tex2D(_MainTex, i.uv);
                int palIndex = int(texel.r * 255);

				float2 palUV;
				fixed4 col;

				if (8 <= palIndex && palIndex < 16 && _PlayerIndex > 0) {
					
					palUV.x = float(palIndex - 8) / 8.0;
					palUV.y = float(16 - _PlayerIndex) / 16.0;
					
					col.rgb = tex2D(_PlayerTex, palUV);

				} else {

					palUV.x = float(palIndex & 0xF) / 16.0;
					palUV.y = float(0xF - ((palIndex >> 4) & 0xF)) / 16.0;

					col.rgb = tex2D(_PalleteTex, palUV);
				
				}

				col.a =  texel.g;

				return col;
            }
            ENDCG
        }
    }
}
