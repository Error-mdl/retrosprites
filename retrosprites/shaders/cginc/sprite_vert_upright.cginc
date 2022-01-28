v2f vert(VertIn v)
{

	v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_TRANSFER_INSTANCE_ID(v, o);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	// Get the position in between the two cameras if the viewer is in VR, otherwise get the position of the
	// camera. If you don't do this, the sprite will look very stereo-incorrect as it will be oriented toward
	// both eyes simultaneously
	float4 cameraPos = wrld_cam_pos();
	float2 cameraDir = obj_cam_dir();
	float4 oCenter = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
	float3 forward = mul(unity_ObjectToWorld, float4(0, 0, 1, 0)).xyz;
	//v.pos.xyz *= _Scale.xyz;
	v.pos.xyz *= length(mul(unity_ObjectToWorld, float4(0, 0, 1, 0)).xyz);

	cameraPos.xyz = oCenter - cameraPos.xyz;
	//float3 ortho = normalize(cross(float3(0, 1, 0), velocity));
	float3 ortho = normalize(-forward);
	float4 cameraPos2 = rotate_camera(float4(cameraPos.xyz, 1), ortho);
	cameraPos2.x = -cameraPos2.x;
	//v.pos.xyz -= oCenter.xyz;
	float4 pos = rotate_sprite(cameraPos, cameraDir, v.pos);
	pos.xyz += oCenter.xyz;
	pos = mul(unity_WorldToObject, pos);
	o.pos = UnityObjectToClipPos(pos);
	UNITY_TRANSFER_FOG(o, o.pos);
	int dir = sprite_dir(_Dir, cameraPos2, cameraDir, _InvRot);
	//o.uv.z = dir;
	o.uv = sprite_sheet_uvs(v.uv, dir, _Dir, _Params, _frame);
	o.color = simple_lighting_origin();
	o.color = _light == 1 ? o.color : float3(1, 1, 1);

	return o;
}