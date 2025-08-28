Shader "Custom/StencilMask"
{
    SubShader
    {
        Tags { "Queue" = "Geometry-10" "RenderType" = "Opaque" }
        Pass
        {
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }
            ColorMask 0     // ��д��ɫ��ֻдStencil
            ZWrite On       // �ڵ�������������
        }
    }
}
