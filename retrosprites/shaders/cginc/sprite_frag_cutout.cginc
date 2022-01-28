fixed4 frag(v2f i) : SV_Target
{
	float4 finalColor = UNITY_SAMPLE_TEX2DARRAY(_MainTex, i.uv) * _Color;

	clip(finalColor.a - _alphaClip);

	finalColor = finalColor * float4(i.color,1);
	UNITY_APPLY_FOG(i.fogCoord, finalColor);
	return finalColor;
}