Shader "Custom/VertexColors" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader 
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		
        Pass
        {
			CGPROGRAM
	        #pragma vertex vert
	        #pragma fragment frag
	        #pragma multi_compile_fog
            #include "UnityCG.cginc"

			sampler2D _MainTex;
	        float4 _MainTex_ST;

	        struct appdata
	        {
	            float4 vertex : POSITION;
	            float2 uv : TEXCOORD0;
				float4 color : COLOR;
	        };

	        struct v2f
	        {
	            float2 uv : TEXCOORD0;
	            UNITY_FOG_COORDS(1)
	            float4 vertex : SV_POSITION;
	        };

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

	        v2f vert(appdata v)
	        {
	            v2f o;
	            o.vertex = UnityObjectToClipPos(v.vertex);
	            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
	            UNITY_TRANSFER_FOG(o,o.vertex);
	            return o;
	        }

	        fixed4 frag(v2f i) : SV_Target
	        {
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
	            UNITY_APPLY_FOG(i.fogCoord, col);
	            return col;
	        }
			ENDCG
		}
	}
	FallBack "Diffuse"
}