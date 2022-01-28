struct VertIn
{
float4 pos : POSITION;
float2 uv : TEXCOORD0;
};

struct v2f
{
    float3 uv : TEXCOORD0;
    float4 pos : SV_POSITION;
	float3 color : COLOR;
	UNITY_FOG_COORDS(1)
};

UNITY_DECLARE_TEX2DARRAY(_MainTex);
float4 _Params;
half _alphaClip;
uint _light;
half _frame;
int _Dir;
float4 _Color;

v2f vert(VertIn v)
{
	
    v2f o;
	
	// Get the position in between the two cameras if the viewer is in VR, otherwise get the position of the
	// camera. If you don't do this, the sprite will look very stereo-incorrect as it will be oriented toward
	// both eyes simultaneously
	float4 cameraPos = obj_cam_pos();
	float2 cameraDir = obj_cam_dir();
	float4 pos = rotate_sprite(cameraPos, cameraDir, v.pos);
	o.pos = UnityObjectToClipPos(pos);
	UNITY_TRANSFER_FOG(o, o.pos);
	int dir = sprite_dir(_Dir, cameraPos, cameraDir);
	o.uv.z = dir;
	o.uv.xy = sprite_sheet_uvs(v.uv, _Params, _frame);
	o.color = simple_lighting_origin();
	o.color = _light == 1 ? o.color : float3(1, 1, 1);
	
    return o;
}



fixed4 frag(v2f i) : SV_Target
{
	float4 finalColor = UNITY_SAMPLE_TEX2DARRAY(_MainTex, i.uv) * _Color;

	clip(finalColor.a - _alphaClip);
	
	finalColor = finalColor*float4(i.color,1);
	UNITY_APPLY_FOG(i.fogCoord, finalColor);
	return finalColor;	
}