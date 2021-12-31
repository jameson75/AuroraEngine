Texture2D RenderTargetTexture;
Texture2D CurFrameVelocityTexture;
Texture2D LastFrameVelocityTexture;

SamplerState RenderTargetSampler 
{  
    MinFilter = POINT;  
    MagFilter = POINT;
    AddressU = Clamp;
    AddressV = Clamp;
};

SamplerState CurFramePixelVelSampler
{    
    MinFilter = POINT;
    MagFilter = POINT;
    AddressU = Clamp;
    AddressV = Clamp;
};

SamplerState LastFramePixelVelSampler
{    
    MinFilter = POINT;
    MagFilter = POINT;
    AddressU = Clamp;
    AddressV = Clamp;
};

cbuffer Constants
{
	float PixelBlurConst = 1.0f;
};

static const float NumberOfPostProcessSamples = 12.0f;

//-----------------------------------------------------------------------------
// Name: PostProcessMotionBlurPS 
// Type: Pixel shader                                      
// Desc: Uses the pixel's velocity to sum up and average pixel in that direction
//       to create a blur effect based on the velocity in a fullscreen
//       post process pass.
//-----------------------------------------------------------------------------
float4 PostProcessMotionBlurPS( float2 OriginalUV : TEXCOORD0 ) : SV_TARGET
{
    float2 pixelVelocity;
    
    // Get this pixel's current velocity and this pixel's last frame velocity
    // The velocity is stored in .r & .g channels
    float4 curFramePixelVelocity = CurFrameVelocityTexture.Sample(CurFramePixelVelSampler, OriginalUV);
    float4 lastFramePixelVelocity = LastFrameVelocityTexture.Sample(LastFramePixelVelSampler, OriginalUV);

    // If this pixel's current velocity is zero, then use its last frame velocity
    // otherwise use its current velocity.  We don't want to add them because then 
    // you would get double the current velocity in the center.  
    // If you just use the current velocity, then it won't blur where the object 
    // was last frame because the current velocity at that point would be 0.  Instead 
    // you could do a filter to find if any neighbors are non-zero, but that requires a lot 
    // of texture lookups which are limited and also may not work if the object moved too 
    // far, but could be done multi-pass.
    float curVelocitySqMag = curFramePixelVelocity.r * curFramePixelVelocity.r +
                             curFramePixelVelocity.g * curFramePixelVelocity.g;
    float lastVelocitySqMag = lastFramePixelVelocity.r * lastFramePixelVelocity.r +
                              lastFramePixelVelocity.g * lastFramePixelVelocity.g;
                                   
    if( lastVelocitySqMag > curVelocitySqMag )
    {
        pixelVelocity.x =  lastFramePixelVelocity.r * PixelBlurConst;   
        pixelVelocity.y = -lastFramePixelVelocity.g * PixelBlurConst;
    }
    else
    {
        pixelVelocity.x =  curFramePixelVelocity.r * PixelBlurConst;   
        pixelVelocity.y = -curFramePixelVelocity.g * PixelBlurConst;    
    }
    
    // For each sample, sum up each sample's color in "Blurred" and then divide
    // to average the color after all the samples are added.
    float3 Blurred = 0;    
    for(float i = 0; i < NumberOfPostProcessSamples; i++)
    {   
        // Sample texture in a new spot based on pixelVelocity vector 
        // and average it with the other samples        
        float2 lookup = pixelVelocity * i / NumberOfPostProcessSamples + OriginalUV;
        
        // Lookup the color at this new spot
        float4 Current = RenderTargetTexture.Sample(RenderTargetSampler, lookup);
        
        // Add it with the other samples
        Blurred += Current.rgb;
    }
    
    // Return the average color of all the samples
    return float4(Blurred / NumberOfPostProcessSamples, 1.0f);
}
