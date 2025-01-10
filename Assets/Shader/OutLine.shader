Shader "Custom/OutlineShader"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (1, 1, 1, 1) // �A�E�g���C���̐F
        _OutlineThickness("Outline Thickness", Range(0.0, 0.1)) = 1000 // �A�E�g���C���̌���
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "OUTLINE"
            Cull Front          // �����̃|���S���𖳎����ĊO���̂ݕ`��
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

            float _OutlineThickness; // �A�E�g���C���̌���
            float4 _OutlineColor;    // �A�E�g���C���̐F

            v2f vert(appdata v)
            {
                v2f o;

                // �@�������ɒ��_���ړ��i�O���ɂ̂݃I�t�Z�b�g��K�p�j
                float3 norm = normalize(v.normal);
                float3 offset = norm * _OutlineThickness;
                o.pos = UnityObjectToClipPos(v.vertex + float4(offset, 0.0));
                o.color = _OutlineColor;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return i.color; // �A�E�g���C���̐F
            }
            ENDCG
        }

        Pass
        {
            Name "BASE"
            Cull Back          // �ʏ�`��
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
                return half4(1, 1, 1, 1); // �x�[�X�̐F
            }
            ENDCG
        }
    }
}
