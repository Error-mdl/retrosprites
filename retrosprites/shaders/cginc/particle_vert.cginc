v2f vert(VertIn v)
{
	
    v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	//UNITY_TRANSFER_INSTANCE_ID(v, o);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	// Get the position in between the two cameras if the viewer is in VR, otherwise get the position of the
	// camera. If you don't do this, the sprite will look very stereo-incorrect as it will be oriented toward
	// both eyes simultaneously
	float4 cameraPos = wrld_cam_pos();
	float2 cameraDir = obj_cam_dir();
	float3 pCenter = float3(v.uv_center.zw, v.center_velocity.x);
	float3 velocity = v.center_velocity.yzw;

	cameraPos.xyz = pCenter - cameraPos.xyz;
	//float3 ortho = normalize(cross(float3(0, 1, 0), velocity));
	float3 ortho = normalize(-velocity);
	float4 cameraPos2 = rotate_camera(float4(cameraPos.xyz,1), ortho);
	//cameraPos2.x = -cameraPos2.x;
	

	v.pos.xyz -= pCenter;

	float4 pos = rotate_sprite(cameraPos, cameraDir, v.pos);
	pos.xyz += pCenter;
	o.pos = UnityObjectToClipPos(pos);
	
	int dir = sprite_dir(_Dir, cameraPos2, cameraDir, _InvRot);
	//o.uv.z = dir;
	float4 params = float4(1, 1, _Params.z, _Params.w);
	o.uv = sprite_sheet_uvs(v.uv_center.xy, dir, _Dir, params, _frame);
	//o.uv.z = dir;
	o.lighting = simple_lighting(pCenter);
	o.color = v.color;
	
    return o;
}