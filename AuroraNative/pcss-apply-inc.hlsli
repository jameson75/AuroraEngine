// -------------------------------------
// STEP 1: Search for potential blockers
// -------------------------------------
float findBlocker(float2 uv,
		float4 LP,		
		float bias,
		float searchWidth,
		float numSamples,
		Texture2D ShadMap,
		SamplerState ShadSampler)
{
        // divide filter width by number of samples to use
        float stepSize = 2 * searchWidth / numSamples;

        // compute starting point uv coordinates for search
        uv = uv - float2(searchWidth, searchWidth);

        // reset sum to zero
        float blockerSum = 0;
        float receiver = LP.z;
        float blockerCount = 0;
        float foundBlocker = 0;

        // iterate through search region and add up depth values
        for (int i=0; i<numSamples; i++) {
               for (int j=0; j<numSamples; j++) {
                       float shadMapDepth = ShadMap.Sample(ShadSampler, uv +
                                                 float2(i*stepSize,j*stepSize)).x;
                       // found a blocker
                       if (shadMapDepth < receiver) {
                               blockerSum += shadMapDepth;
                               blockerCount++;
                               foundBlocker = 1;
                       }
               }
        }

		float result;
		
		if (foundBlocker == 0) {
			// set it to a unique number so we can check
			// later to see if there was no blocker
			result = 999;
		}
		else {
		    // return average depth of the blockers
			result = blockerSum / blockerCount;
		}
		
		return result;
}

// ------------------------------------------------
// STEP 2: Estimate penumbra based on
// blocker estimate, receiver depth, and light size
// ------------------------------------------------
float estimatePenumbra(float4 LP,
			float Blocker,
			float LightSize)
{
       // receiver depth
       float receiver = LP.z;
       // estimate penumbra using parallel planes approximation
       float penumbra = (receiver - Blocker) * LightSize / Blocker;
       return penumbra;
}

// ----------------------------------------------------
// Step 3: Percentage-closer filter implementation with
// variable filter width and number of samples.
// This assumes a square filter with the same number of
// horizontal and vertical samples.
// ----------------------------------------------------

float PCF_Filter(float2 uv, float4 LP, 
                 float bias, float filterWidth, float numSamples,
				 Texture2D ShadMap, SamplerState ShadSampler)
{
       // compute step size for iterating through the kernel
       float stepSize = 2 * filterWidth / numSamples;

       // compute uv coordinates for upper-left corner of the kernel
       uv = uv - float2(filterWidth,filterWidth);

       float sum = 0;  // sum of successful depth tests

       // now iterate through the kernel and filter
       for (int i=0; i<numSamples; i++) {
               for (int j=0; j<numSamples; j++) {
                       // get depth at current texel of the shadow map
                       float shadMapDepth = 0;
                       
                       shadMapDepth = ShadMap.Sample(ShadSampler, uv +
                                            float2(i*stepSize,j*stepSize)).x;

                       // test if the depth in the shadow map is closer than
                       // the eye-view point
                       float shad = LP.z < shadMapDepth;

                       // accumulate result
                       sum += shad;
               }
       }
       
       // return average of the samples
       return sum / (numSamples*numSamples);
}

float caclSoftShadow(float4 LP, float ShadBias, float SceneScale, float LightSize, 
					 Texture2D ShadMap, SamplerState ShadSampler)
{
	   // The soft shadow algorithm follows:

   // Compute uv coordinates for the point being shaded
   // Saves some future recomputation.
   float2 uv = float2(.5,-.5)*(LP.xy)/LP.w + float2(.5,.5);

   // ---------------------------------------------------------
   // Step 1: Find blocker estimate
   float searchSamples = 6;   // how many samples to use for blocker search
   float zReceiver = LP.z;
   float searchWidth = SceneScale * (zReceiver - 1.0) / zReceiver;
   float blocker = findBlocker(uv, LP, ShadBias,
                              SceneScale * LightSize / LP.z, searchSamples,
							  ShadMap, ShadSampler);
   
   //return (blocker*0.3);  // uncomment to visualize blockers
   
   // ---------------------------------------------------------
   // Step 2: Estimate penumbra using parallel planes approximation
   float penumbra;  
   penumbra = estimatePenumbra(LP, blocker, LightSize);

   //return penumbra*32;  // uncomment to visualize penumbrae

   // ---------------------------------------------------------
   // Step 3: Compute percentage-closer filter
   // based on penumbra estimate
   float samples = 8;	// reduce this for higher performance

   // Now do a penumbra-based percentage-closer filter
   float shadowed; 

   shadowed = PCF_Filter(uv, LP, ShadBias, penumbra, samples, ShadMap, ShadSampler);
   
   // If no blocker was found, just return 1.0
   // since the point isn't occluded
   
   if (blocker > 998) 
   		shadowed = 1.0;

   return shadowed;
}
