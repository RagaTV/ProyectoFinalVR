Shader "Custom/MonedasBlancas"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // Configuramos para que soporte transparencia
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100
        ZWrite Off

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Leemos el color original de tu imagen
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // INVERSIÓN: 
                // Si el pixel era negro (0), ahora es blanco (1).
                // Mantenemos el Alpha original para que el fondo siga siendo transparente.
                return fixed4(1.0 - col.rgb, col.a);
            }
            ENDCG
        }
    }
}