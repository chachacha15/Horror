Shader "Custom/OutlineShader"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (1, 1, 1, 1) // アウトラインの色
        _OutlineThickness("Outline Thickness", Range(0.0, 0.1)) = 1000 // アウトラインの厚み
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "OUTLINE"
            Cull Front          // 内側のポリゴンを無視して外側のみ描画
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            float _OutlineThickness; // アウトラインの厚み
            float4 _OutlineColor;    // アウトラインの色

            v2f vert(appdata v)
            {
                v2f o;

                // 法線方向に頂点を移動（外側にのみオフセットを適用）
                float3 norm = normalize(v.normal);
                float3 offset = norm * _OutlineThickness;
                o.pos = UnityObjectToClipPos(v.vertex + float4(offset, 0.0));
                o.color = _OutlineColor;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return i.color; // アウトラインの色
            }
            ENDCG
        }

        Pass
        {
            Name "BASE"
            Cull Back          // 通常描画
            ZWrite On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return half4(1, 1, 1, 1); // ベースの色
            }
            ENDCG
        }
    }
}
