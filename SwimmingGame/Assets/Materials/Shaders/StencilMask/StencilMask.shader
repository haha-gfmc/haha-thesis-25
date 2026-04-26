Shader "Unlit/StencilMask"
{
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Geometry-10"
        }

        Pass
        {
            ColorMask 0
            ZWrite Off
            ZTest LEqual
            Cull Off

            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }
        }
    }
}
