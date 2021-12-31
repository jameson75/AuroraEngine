
using namespace DirectX;

#ifndef _MARSHABLE_TYPES_H
#define _MARSHABLE_TYPES_H
 
///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

typedef struct _XVECTOR4
{	
    float C1;
    float C2;
    float C3;
    float C4;   	
 } XVECTOR4;

typedef struct _XMFLOAT4_
{	
    float X;
    float Y;
    float Z;
    float W;   	
 } XMFLOAT4_;

 typedef struct _XMFLOAT3_
 {
	 float X;
	 float Y;
	 float Z;
 } XMFLOAT3_;

 typedef struct _XMFLOAT2_
 {
	 float X;
	 float Y;
 } XMFLOAT2_;
 

#endif