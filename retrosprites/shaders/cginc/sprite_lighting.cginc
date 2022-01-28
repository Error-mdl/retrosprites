/**
 * @file sprite_lighting.cginc
 * @author Error.mdl
 * @date 2019-06-05
 * @brief Contains simple lighting functions for use in the directional billboard shader. 
 */


/**
 * @brief get the light color from baked lighting, a directional light, and four vertex lights at the object-space origin
 * @return the sum of the lights hitting the object space origin.
 */

float3 simple_lighting_origin()
{
	float3 light;
	
	//Baked lighting. Taking the length of each of the linear terms of the spherical harmonics polynomial gives
	//us close to the maximum baked light value at that point.
	light = float3(length(unity_SHAr), length(unity_SHAg), length(unity_SHAb));

	//Directional light
	light += _LightColor0.rgb;
	
	//Vertex lights
	#ifdef VERTEXLIGHT_ON
		// Calculate object's center position in world-space (only works if the mesh hasn't been batched!)
		float4 worldOrigin = UNITY_MATRIX_M._m03_m13_m23_m33;

		// to light vectors
		float4 toLightX = unity_4LightPosX0 - worldOrigin.x;
		float4 toLightY = unity_4LightPosY0 - worldOrigin.y;
		float4 toLightZ = unity_4LightPosZ0 - worldOrigin.z;

		// squared lengths
		float4 lengthSq = 0;
		lengthSq += toLightX * toLightX;
		lengthSq += toLightY * toLightY;
		lengthSq += toLightZ * toLightZ;

		// attenuation
		float4 atten = 1.0 / mad(lengthSq, unity_4LightAtten0, 1.0);

		// final color
		light += unity_LightColor[0].rgb * atten.x;
		light += unity_LightColor[1].rgb * atten.y;
		light += unity_LightColor[2].rgb * atten.z;
		light += unity_LightColor[3].rgb * atten.w;

	#endif


	return light;
}


/**
 * @brief get the light color from baked lighting, a directional light, and four vertex lights at a given position
 * @param pos World-space position to calculate the lighting at
 * @return the sum of the lights hitting the given postion.
 */
float3 simple_lighting(float3 pos)
{
	float3 light;

	//Baked lighting. Taking the length of each of the linear terms of the spherical harmonics polynomial gives
	//us close to the maximum baked light value  at that point.
	light = float3(length(unity_SHAr), length(unity_SHAg), length(unity_SHAb));

	//Directional light
	light += _LightColor0.rgb;

	//Vertex lights
	#ifdef VERTEXLIGHT_ON
		// to light vectors
		float4 toLightX = unity_4LightPosX0 - pos.x;
		float4 toLightY = unity_4LightPosY0 - pos.y;
		float4 toLightZ = unity_4LightPosZ0 - pos.z;

		// squared lengths
		float4 lengthSq = 0;
		lengthSq += toLightX * toLightX;
		lengthSq += toLightY * toLightY;
		lengthSq += toLightZ * toLightZ;

		// attenuation
		float4 atten = 1.0 / mad(lengthSq, unity_4LightAtten0, 1.0);

		// final color
		light += unity_LightColor[0].rgb * atten.x;
		light += unity_LightColor[1].rgb * atten.y;
		light += unity_LightColor[2].rgb * atten.z;
		light += unity_LightColor[3].rgb * atten.w;
	#endif


	return light;
}