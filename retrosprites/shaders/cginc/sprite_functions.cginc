/**
 * @file sprite_functions.cginc
 * @author Error.mdl
 * @date 2019-06-05
 * @brief A collection of miscellaneous functions used by my directional billboard shaders
 */

bool IsInMirror()
{
    return unity_CameraProjection[2][0] != 0.f || unity_CameraProjection[2][1] != 0.f;
}

/**
 * @brief Returns the world space postion of the camera, corrected to be the midpoint between the eyes for VR users
 */
float4 wrld_cam_pos()
{
	#if UNITY_SINGLE_PASS_STEREO
		float4 cameraPos = float4((unity_StereoWorldSpaceCameraPos[0] + unity_StereoWorldSpaceCameraPos[1])*0.5, 1);
	#else
		float4 cameraPos = mul(unity_CameraToWorld, float4(0,0,0,1));
	#endif
	return cameraPos;
}

float2 obj_cam_dir()
{
    float2 cameraDir = float2(0, 0);
    UNITY_BRANCH if (IsInMirror())
    {
        //cameraDir = mul((float3x3)unity_WorldToObject, mul((float3x3)unity_CameraToWorld, float3(0, 0, 1))).xz;
        cameraDir = mul((float3x3)unity_WorldToObject, unity_CameraWorldClipPlanes[5].xyz).xz;
    }
    return cameraDir;
}

/**
 * @Brief Returns the object space position of the camera, corrected for VR
 */
float4 obj_cam_pos()
{
	float4 cameraPos = wrld_cam_pos();
	cameraPos =  mul(unity_WorldToObject, cameraPos);
	return cameraPos;
}



/**
 * @brief Given the position of a vertex and camera, rotates the vertex around the origin in the xz plane so that the 
 * Z-axis points towards the camera.
 * @param cameraPos object-space position of the camera
 * @param vertPos object-space position of vertex
 * @return New position of the vertex rotated about the origin
 */
float4 rotate_sprite(float4 cameraPos, float2 cameraForward, float4 vertPos)
{
    float len, cosa, sina = 0.0;
    UNITY_BRANCH if (!IsInMirror())
    {
        len = distance(float2(0, 0), float2(cameraPos[0], cameraPos[2]));

        cosa = (cameraPos[2]) / len;
        sina = (cameraPos[0]) / len;
    }
    else
    {
        cameraForward = -normalize(cameraForward);
        cosa = cameraForward.y;
        sina = cameraForward.x;
    }
	
	float2x2 R = float2x2(
		cosa,	sina,
		-sina,	cosa
		);
	
	vertPos.xz = mul(R, vertPos.xz);
	return vertPos;
}

/**
 * @brief Given the position of a camera and a forward vector, rotates the camera around the origin in the xz plane so that the
 * z+ unit vector in the camera's original space is now aligned with the forward vector.
 *
 * Used to rotate the camera to match the forward velocity so that calculating the sprite direction is easier
 * for particle variants of the shader
 *
 * @param cameraPos position of the camera
 * @param vertPos forward vector
 * @return New position of the camera rotated about the origin
 */
float4 rotate_camera(float4 cameraPos, float3 forward)
{
	float cosa = forward.z;
	float sina = -forward.x;

	float2x2 R = float2x2(
		cosa, sina,
		-sina, cosa
		);

	cameraPos.xz = mul(R, cameraPos.xz);
	return cameraPos;
}


/**
 * @brief Calculates the direction on the sprite sheet to use.
 * 
 * Starts at 0 for +z and increases clockwise. So for example in a four directional sprite, looking in the -z direction
 * you will see image 0, looking in -x you will see image 1, looking in +z you will see image 2, and looking in +x you
 * will see image 3.
 *
 * @param totalDivisions The number of different directions you have in your texture array
 * @param cameraPos Object-space camera position
 * @return dir The index of the texture in the texture array to use for the direction calcuated.
 */
int sprite_dir(int totalDivisions, float4 cameraPos, float2 cameraDir, float InvRot)
{
	//Get the angle between the camera and (0,0,-1) in the xz plane, ranges from -pi to pi
    float angle = 0;
    float rotSign = InvRot > 0 ? -1 : 1;
    UNITY_BRANCH if (!IsInMirror())
    {
        angle = atan2(rotSign*-cameraPos[0], -cameraPos[2]);
    }
    else
    {
        angle = atan2(rotSign * cameraDir.x, cameraDir.y);
    }
	//Calculate the fraction of 2pi each direction occupies (eg for 8 directions, each division is 0.25*pi)
	float div = 1.0 / ((float)totalDivisions);
	// Calculate which texture in the texture array to use. Starts at 0 for the +z spritesheet and increases
	// counter-clockwise. 
	int dir = floor((angle + (1 + div)*UNITY_PI)/(UNITY_TWO_PI*div)) % totalDivisions;
	return dir;
}


/**
 * @brief Transform the give uv coordinates to a single tile of the sprite sheet if using sprite sheets, or calculate
 *        array index if using an individual sprite array.
 *
 * If using sprite sheets, transforms the given UVs (assuming a 0-1 range on both axes) to only span the portion of the
 * active tile in the sprite sheet. Calculates which tile is active based on time, framerate, and manual frame number.
 * If using individual sprites, calculates the array index corresponding to the direction and animation frame
 *
 * @param uv            the original uv coordinates
 * @param dir           the direction the sprite is being viewed from, as an integer ranging from 0 to the total number of directions minus one
 * @param totalDir      the total number of directions with a unique sprite representing them
 * @param Params        float4 containing in order: the number of columns in the sheet, the number of rows,
 *                      the total number of tiles (allows you to leave spaces empty at the end of the sprite sheet), and the framerate.
 * @param manualFrame   With the framerate set to 0, allows you to manually increment the active frame in the sprite sheet.
 *                      Useful for animating the sprite through a unity animation.
 * @return UV coordinates transformed to the active tile in the sheet.
 */
float3 sprite_sheet_uvs(float2 uv, float dir, float totalDir, float4 Params, float manualFrame)
{
    float3 tile_uv;
    uint3 dim = floor(Params.xyz);
    uint frame_num = floor(fmod(_Time[1] * Params.w + manualFrame, Params.z));
#ifdef SPRITE_SHEET
	int2 frame = int2(frame_num % dim.x, frame_num / dim.x);
	tile_uv = float3((uv.x + frame[0])/Params.x, ((uv.y - frame[1])/Params.y) + (Params.y - 1.0)/Params.y, dir);
#else
    tile_uv = float3(uv, frame_num * totalDir + dir);
#endif
	return tile_uv;
}

