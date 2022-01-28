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
	UNITY_VERTEX_OUTPUT_STEREO
};

UNITY_DECLARE_TEX2DARRAY(_MainTex);
float4 _Params;
half _alphaClip;
uint _light;
half _frame;
int _Dir;
float4 _Color;
float _InvRot;

v2f vert(VertIn v)
{

	v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	float4 cameraPos = obj_cam_pos();
	float2 cameraDir = obj_cam_dir();
	float4 pos = rotate_sprite(cameraPos, cameraDir, v.pos);
	o.pos = UnityObjectToClipPos(pos);
	UNITY_TRANSFER_FOG(o, o.pos);
	int dir = sprite_dir(_Dir, cameraPos, cameraDir, _InvRot);
	o.uv = sprite_sheet_uvs(v.uv, dir, _Dir, _Params, _frame);

	return o;
}



fixed4 frag(v2f i) : SV_Target
{
	float4 finalColor = UNITY_SAMPLE_TEX2DARRAY(_MainTex, i.uv) * _Color;

	clip(finalColor.a - _alphaClip);

	return finalColor;
}