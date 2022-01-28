fixed4 frag(v2f i) : SV_Target
{
	float4 finalUV = UNITY_SAMPLE_TEX2DARRAY(_MainTex, i.uv) * _Color * i.color;

	clip(finalUV.a - _alphaClip);
	
	finalUV = (_light == 1)*finalUV*float4(i.lighting, 1) + (_light != 1)*finalUV;

	
	return finalUV;	
}