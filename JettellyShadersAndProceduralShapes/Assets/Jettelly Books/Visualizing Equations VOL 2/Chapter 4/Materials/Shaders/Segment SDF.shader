Shader "Jettelly/Chapter 4/UI/Segment SDF"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _ShineVolumeMask ("Shine Volume Mask", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)        
        _Orientation ("Orientation", Range(0.0, 1.0)) = 0.5            
        _Radius ("Radius", Range(0.01, 0.5)) = 0.1
        _Smooth ("Smooth", Range(0.01, 0.5)) = 0.01
        [Space(20)]
        _TimeInSeconds ("Time in Seconds", Range(1.0, 3.0)) = 2.0
        [Space(20)]
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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
        Blend One One
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #include "Assets/Jettelly Books/Visualizing Equations VOL 2/Chapter 4/Materials/HLSL/SegmentSDF.hlsl"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP


            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4  mask : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            sampler2D _ShineVolumeMask;
            
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;
            int _UIVertexColorAlwaysGammaSpace;
            float _Orientation;
            float _Radius;
            float _Smooth;
            float _TimeInSeconds;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;

                float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                OUT.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));


                if (_UIVertexColorAlwaysGammaSpace)
                {
                    if(!IsGammaSpace())
                    {
                        v.color.rgb = UIGammaToLinear(v.color.rgb);
                    }
                }

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {                
                const half alphaPrecision = half(0xff);
                const half invAlphaPrecision = half(1.0/alphaPrecision);
                
                IN.color.a = round(IN.color.a * alphaPrecision) * invAlphaPrecision;
                half4 color = IN.color * (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);

                // Add a displacement for the shine segment.
                half shine_volume_mask = tex2D(_ShineVolumeMask, IN.texcoord);
                // Time in seconds. 1 equals 1 second.
                float seconds = _TimeInSeconds;
                float t = _Time.y / seconds;
                // For the animation, be sure that the segment comes from outside of the UI image borders.
                float offset = frac(t) * 3.0 - 1.5;
                // Clamp the values between 0 and 1.
                float offset_mask = ceil(sin(t * UNITY_PI));
                offset *= offset_mask; 
                // Initialize the segment.
                float4 segment = 0;
                // Initialize the first point.
                half2 p0 = half2(_Orientation + offset, 0.0);
                // Initialize the second point.
                half2 p1 = half2((1 - _Orientation) + offset, 1.0);
                // Generate the segment SD and include the displacement for the UV coordinates.
                segment_sd_float(IN.texcoord * shine_volume_mask, p0, p1, segment);
                float s = _Smooth;
                float r = _Radius;
                segment = smoothstep(segment, segment + s, r);
                // Be sure to apply the mask again to remove the initial segment position when the animation is running.
                segment *= offset_mask;

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                color.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                // color.rgb *= color.a;
                segment.rgb *= IN.color;
                segment.rgb *= color.a;                

                return segment;
            }
        ENDCG
        }
    }
}
