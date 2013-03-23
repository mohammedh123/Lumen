texture screenTexture;

sampler TextureSampler = sampler_state
{
    Texture = <screenTexture>;
};

struct PixelShaderInput
{
    float4 Position : POSITION0;
	float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
    float4 colorA = tex2D(TextureSampler, input.TexCoord);
    
	return float4(0, 0, 0, 1-colorA.r);
}

technique Technique1
{
    pass Pass1	
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
