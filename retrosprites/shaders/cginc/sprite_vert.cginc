/**
 * @file sprite_vert.cginc
 * @author Error.mdl
 * @date 2019-06-05
 * @brief Vertex function for directional sprites shared between the opaque and transparent variants
 */

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
	//o.uv.z = dir;
	o.uv = sprite_sheet_uvs(v.uv, dir, _Dir, _Params, _frame);
	o.color = simple_lighting_origin();
	o.color = _light == 1 ? o.color : float3(1, 1, 1);

	return o;
}