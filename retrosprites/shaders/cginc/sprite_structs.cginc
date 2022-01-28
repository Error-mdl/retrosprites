/**
 * @file sprite_structs.cginc
 * @author Error.mdl
 * @date 2019-06-05
 * @brief Defines the input structures for the vertex and fragment programs used by the object variants of the
 * directional billboard shaders
 */

struct VertIn
{
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
	float3 uv : TEXCOORD0;
	float4 pos : SV_POSITION;
	float3 color : COLOR;
	UNITY_FOG_COORDS(1)
	UNITY_VERTEX_OUTPUT_STEREO
};