// Upgrade NOTE: replaced '_CameraToWorld' with 'unity_CameraToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Error.mdl/Retro Sprites v2/Particle Transparent"
{
    Properties
	{
		[NoScaleOffset] _MainTex("Texture Array", 2DArray) = "white" {}
		[HDR] _Color("Color", color) = (1,1,1,1)
		[Toggle(_)] _light("Enable Lighting?", int) = 1
		_Dir("Number of Viewing Angles", int) = 8
		[ToggleUI] _InvRot("Invert Rotation Direction", float) = 0
		_frame("Manual Frame Number", float) = 0.0
		_Params("Parameters", Vector) = (1,1,1,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "PreviewType" = "Plane"}
		
        LOD 100
        
        Cull off
        ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
        
        Pass {
			Tags {"LightMode" = "ForwardBase"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_fog
			#pragma shader_feature_local SPRITE_SHEET
			#pragma multi_compile _ VERTEXLIGHT_ON
            #include "UnityCG.cginc"
			#include "Lighting.cginc"
            #include "UnityLightingCommon.cginc"
			#include "cginc/sprite_lighting.cginc"
			#include "cginc/sprite_functions.cginc" 
			#include "cginc/particle_structs.cginc"
           
			UNITY_DECLARE_TEX2DARRAY(_MainTex);
			float4 _Params;
			uint _light;
			half _frame;
			int _Dir;
			float4 _Color;
			float _InvRot;

			#include "cginc/particle_vert.cginc"
			#include "cginc/particle_frag.cginc"
            ENDCG
        }
    }
			CustomEditor "RetrospriteInspector"
}