Shader "Unlit/TexturedAnimations_OldShader"
{
    Properties
	{
        [MainColor] _BaseColor("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
        _CurrentAnimationTime("Current Animation Time", Float) = 0
        _NextAnimationTime("Next Animation Time", Float) = 0

        [Space(50)]

        _Blend("Blend", Range(0, 1)) = 0
        [Space(20)]
		_CurrentAnimMap ("Current AnimMap", 2D) = "white" {}
		_CurrentAnimLen("Current Anim Length", Float) = 0
        [Space(30)]
        _NextAnimMap ("Next AnimMap", 2D) = "white" {}
        _NextAnimLen("Next Anim Length", Float) = 0


	}
	
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Cull off
		LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //gpu instancing
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float2 uv : TEXCOORD0;
                float4 pos : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            fixed4 _BaseColor;

            sampler2D _MainTex;
            float4 _MainTex_ST;


            float _CurrentAnimationTime;
            float _NextAnimationTime;

            sampler2D _CurrentAnimMap;
            float4 _CurrentAnimMap_TexelSize;//x == 1/width
            float _CurrentAnimLen;

            sampler2D _NextAnimMap;
            float4 _NextAnimMap_TexelSize;
            float _NextAnimLen;


            float _Blend;

            
            v2f vert (appdata v, uint vid : SV_VertexID)
            {
               UNITY_SETUP_INSTANCE_ID(v);
               
               // Current Anim Map
               //float currentF = _Time.y / _CurrentAnimLen;
               //float currentF = _CurrentAnimationTime;

               float currentAnimMap_x = (vid + 0.5) * _CurrentAnimMap_TexelSize.x;
               float currentAnimMap_y = _Time.y / _CurrentAnimLen;;
               
               // Next Anim Map
               //float nextF = _Time.y / _NextAnimLen;
               //float nextF = _NextAnimationTime;
               float nextAnimMap_x = (vid + 0.5) * _NextAnimMap_TexelSize.x;
               float nextAnimMap_y = _Time.y / _NextAnimLen;;

               //float4 pos = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));


               //float4 targetPos = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));
               //float4 currentPos = v.pos * 0.015;
               //float4 direction =  normalize(targetPos - currentPos);
               //float distance = length(targetPos - currentPos);
               //
               //float4 pos = currentPos + direction * (distance / _AnimLen);


               //float4 currentPos = v.pos * 0.015;
               //float4 targetPos = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));
               //float4 pos = lerp(currentPos, targetPos, _Blend);
              


               float4 currentPos = tex2Dlod(_CurrentAnimMap, float4(currentAnimMap_x, currentAnimMap_y, 0, 0));
               float4 targetPos = tex2Dlod(_NextAnimMap, float4(nextAnimMap_x, nextAnimMap_y, 0, 0));
               float4 pos = lerp(currentPos, targetPos, _Blend);


               v2f o;
               o.vertex = UnityObjectToClipPos(pos);
               o.uv = TRANSFORM_TEX(v.uv, _MainTex);
               

               return o;


            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _BaseColor;
                return col;
            }
            ENDCG
        }
	}
}
