#ifndef SEA_FORWARD_INCLUDED
#define SEA_FORWARD_INCLUDED

#include "UnityStandardCore.cginc"

float _Freq1;
float _Freq2;
float _Freq3;
float _Freq4;
float _Amp1;
float _Amp2;
float _Amp3;
float _Amp4;
float _AmpFactor;
float _Phase1;
float _Phase2;
float _Phase3;
float _Phase4;

VertexOutputForwardBase vertSeaForwardBase (VertexInput v)
{
	//On perturbe le vertex en entree
	v.vertex.z += _AmpFactor * _Amp1 * sin(_Time.y*_Freq1+v.vertex.x + _Phase1) +
	              _AmpFactor * _Amp2 * sin(_Time.y*_Freq2+v.vertex.y + _Phase2) +
	              _AmpFactor * _Amp3 * sin(_Time.y*_Freq3+v.vertex.x + _Phase3) +
	              _AmpFactor * _Amp4 * sin(_Time.y*_Freq4+v.vertex.y + _Phase4);

	return vertForwardBase(v);
} 

#endif // SEA_FORWARD_INCLUDED
