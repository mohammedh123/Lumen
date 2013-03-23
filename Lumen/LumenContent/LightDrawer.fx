texture screenTexture;

sampler TextureSampler = sampler_state
{
    Texture = <screenTexture>;
};

float2  lightPosition;
float   lightRadius;

struct PixelShaderInput
{
    float4 Position : POSITION0;
	float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
    float4 color = tex2D(TextureSampler, input.TexCoord);
    float2 dist = lightPosition - input.TexCoord;
    float2 dx = dist*float2(1024,768);
    float alpha = length(dx)/lightRadius;

    if(length(dx) < lightRadius)
    {
        return float4(1-alpha,0,0,1-alpha);
    }

    return color;
}

technique Technique1
{
    pass Pass1	
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
