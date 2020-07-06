float4x4 World;
float4x4 View;
float4x4 Projection;
float4 Color;
float FarClip;

texture TunnelTexture;
sampler2D textureSampler = sampler_state {
	Texture = (TunnelTexture);
	MipFilter = LINEAR;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	AddressU = Wrap;
	AddressV = Clamp;
};


struct VertexShaderInput
{
	float4 Position : SV_POSITION;
	float2 TexUV : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TexUV : TEXCOORD0;
	float Depth : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
		float4 viewPosition = mul(worldPosition, View);
		output.Position = mul(viewPosition, Projection);
	output.TexUV = input.TexUV;
	output.Depth = output.Position.z;
	return output;
}

float DistanceSquared(float4 coords)
{
	return coords.x * coords.x + coords.y * coords.y + coords.z * coords.z;
}

float4 TunnelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 output = Color * tex2D(textureSampler, input.TexUV);
	output.a = 1.0f;

	float dist = saturate(input.Depth / FarClip);
	dist = 1.0f - dist;

	output.r *= dist;
	output.g *= dist;
	output.b *= dist;

	return output;
}

technique Tunnel
{
	pass Pass1
	{
		ZENABLE = TRUE;
		ZWRITEENABLE = TRUE;
		CULLMODE = CCW;
		VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
		PixelShader = compile ps_4_0_level_9_1 TunnelShaderFunction();
	}
}
