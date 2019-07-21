
// Used for billboarded terrain details; waving and bending are controlled by the alpha of the waving tint.
// Replaces terrain grass shader (when billboard option is enabled).
Shader "Hidden/TerrainEngine/Details/BillboardWavingDoublePass" 
{
    Properties
    {
        _WavingTint("Fade Color", Color) = (.7,.6,.5, 0)
        _MainTex("Base (RGB) Alpha (A)", 2D) = "white" {}
        _WaveAndDistance("Wave and distance", Vector) = (12, 3.6, 1, 1)
        _AlphaCutoff("Cutoff", float) = 0.95
    }

    CGINCLUDE

    #include "UnityCG.cginc"
    #include "TerrainEngine.cginc"

    uniform float4 _BendData[16];
    sampler2D _MainTex;
    float4 _MainTex_ST;
    fixed _AlphaCutoff;

    struct v2f
    {
        float4 pos : SV_POSITION;
        fixed4 color : COLOR;
        float2 texcoord : TEXCOORD0;
        UNITY_FOG_COORDS(1)
        UNITY_VERTEX_OUTPUT_STEREO
    };

    v2f vert(appdata_full v)
    {
        v2f o;

        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        // Bending.
        for (int i = 0; i < 16; i++)
        {
            float bendRadius = _BendData[i].w;
            float3 benderWorldPos = _BendData[i].xyz;
            float3 vertexWorldPos = mul(unity_ObjectToWorld, v.vertex);

            float distToBender = distance(float3(vertexWorldPos.x, 0, vertexWorldPos.z), float3(benderWorldPos.x, 0, benderWorldPos.z));
            float bendPower = (bendRadius - min(bendRadius, distToBender)) / (bendRadius + 0.001);
            float3 bendDir = normalize(vertexWorldPos - benderWorldPos);
            float2 vertexOffset = bendDir.xz * bendPower * v.texcoord.y * v.tangent.y;
            v.vertex.xz += lerp(float2(0, 0), vertexOffset, saturate(bendRadius * v.color.w));
        }

        // Billboarding.
        TerrainBillboardGrass(v.vertex, v.tangent.xy);

        // Waving by the wind.
        float waveAmount = v.tangent.y * v.color.w;
        o.color = TerrainWaveGrass(v.vertex, waveAmount, v.color);
        o.color.rgb *= ShadeVertexLights(v.vertex, v.normal);

        o.pos = UnityObjectToClipPos(v.vertex);
        o.texcoord = v.texcoord;

        UNITY_TRANSFER_FOG(o, o.pos);

        return o;
    }

    ENDCG

    SubShader
    {
        Tags
        {
            "Queue" = "Geometry+501"
            "IgnoreProjector" = "True"
            "RenderType" = "GrassBillboard"
            "DisableBatching" = "True"
            "LightMode" = "Vertex"
        }

        Cull Off
        LOD 200

        // First pass: render any pixels that are more than [_Cutoff] opaque (alpha testing).
        // This will yeild the correct depth sorting, but the edges will look jagged.
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            fixed4 frag(v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.texcoord);
                clip(color.a - _AlphaCutoff);
                color.rgb *= i.color;
                UNITY_APPLY_FOG(i.fogCoord, color);
                return color;
            }

            ENDCG
        }

        // Second pass: render the semitransparent details using alpha blending.
        // This will fix the jagged edges, though in potenitally wrong depth order.
        Pass
        {
            ZWrite off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            fixed4 frag(v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.texcoord);
                clip(_AlphaCutoff - color.a);
                color.rgb *= i.color;
                UNITY_APPLY_FOG(i.fogCoord, color);
                return color;
            }

            ENDCG
        }
    }
}
