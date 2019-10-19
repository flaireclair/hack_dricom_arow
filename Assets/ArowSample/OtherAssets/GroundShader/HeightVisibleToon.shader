Shader "Custom/HeightVisibleAndToon" {
	Properties {
//        [Header("基本カラーを除き、全てスクリプトから設定する")]
//        [Header("地面の基本カラー")]
        _Color ("Color", Color) = (1,0.5,0,1)
//        [Header("道のテクスチャ表示をする場合にON")]
        [Toggle]_IsRoadTex("Use RoadTex", Float) = 0    // bool
        _MainTex ("RoadTex", 2D) = "white" {}
//        [Header("標高を色で表現をする場合にON")]
        [Toggle]_IsVisibleColorHeight("Visible ColorHeight", Float) = 0     // bool
        _HeightColorTex ("HeightColor", 2D) = "white" {}
//        [Header("標高の色表現を行う際の高さ設定")]
        _MaxHeight ("Max ColorHeight", Float) = 100
        _MinHeight ("Min ColorHeight", Float) = 100
//        [Header("等高線の表示をする場合にON　※簡易機能")]
        [Toggle]_IsVisibleContourLine("Visible Contour Line", Float) = 0    // bool       
//        [Header("等高線の高さ")]
        _LoopHeightContourLine("Contour Line", Float) = 10

//        [Header("トゥーン（テクスチャ）影を使用する場合にON")]
        _IsToonLighting("Use ToonLighting", Float) = 10
        _RampTex ("ToonLightTex", 2D) = "white" {}        
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf ToonRamp fullforwardshadows vertex:VertFunction

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)  
        
        #pragma multi_compile Lighting_ON

        #include "HeightVisibleToonMeta.cginc"
        
        #include "HeightVisibleToonCore.cginc"
        
		ENDCG
	}
	FallBack "Diffuse"
}
