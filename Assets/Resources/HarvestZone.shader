Shader "Hidden/HarvestZone"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
			Blend One OneMinusSrcAlpha

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			fixed4 _MainTex_TexelSize;
			sampler2D _Composite;
			fixed4 _Color;
			float _ZebraSize;
			float _ShadowIntensity;
			float _EffectIntensity;

			fixed4 zebra(fixed2 uv, float size) {
				float value = lerp(uv.x * _MainTex_TexelSize.z / size, uv.y * _MainTex_TexelSize.w / size, 0.5);
				value = floor(frac(value) + 0.5);

				fixed4 col = lerp(fixed4(0,0,0,0), _Color, value);
				//col.r = ceil(sin(uv.x * _MainTex_TexelSize.z / 10) + (sin(uv.y * _MainTex_TexelSize.w / 10)));

				return col;
			}

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 mask = tex2D(_Composite, i.uv);
				fixed4 image = tex2D(_MainTex, i.uv);

				fixed4 zebCol = zebra(i.uv, _ZebraSize);
				fixed3 imgCol = image.rgb * _ShadowIntensity;

				fixed3 col = lerp(image.rgb, imgCol * (1 - zebCol.a) + zebCol.rgb * zebCol.a , (1 - mask.a) * _EffectIntensity);


				return fixed4(col, 1.0);
            }
            ENDCG
        }
    }
}
