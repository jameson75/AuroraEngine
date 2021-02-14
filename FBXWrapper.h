#define FBXSDK_SHARED
#define FBXSDK_NEW_API
#define N_TRIANGLE_POINTS 3
#include <fbxsdk.h>
#include <list>
#include <map>
#include <iostream>
#include "MarshableDXTypes.h"

#define XFBX_KEYFRAME_FLAG_POSITION 0x01
#define XFBX_KEYFRAME_FLAG_ROTATION	0x02
#define XFBX_KEYFRAME_FLAG_SCALE	0x04

using namespace std;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

typedef struct _XFBX_KEYFRAME
 {
	 long Time;
	 int Flags;
	 XMFLOAT3_ Position;
	 XMFLOAT4_ Rotation;
	 XMFLOAT3_ Scale;
 } XFBX_KEYFRAME; 

 typedef struct _XFBX_ANIMATION_LAYER
 {
	 XFBX_KEYFRAME* KeyFrames;
	 int KeyFrameCount;
 } XFBX_ANIMATION_LAYER;

 typedef struct _XFBX_ANIMATION_TAKE
 {
	 XFBX_ANIMATION_LAYER* Layers;
	 int LayerCount;
 } XFBX_ANIMATION_TAKE;

 typedef struct _XFBX_SKIN_WEIGHTS
 {
	 char* TransformNodeName;
	 float* Weights;
	 int WeightCount;
	 int * VertexIndices;
 }XFBX_SKIN_WEIGHTS;

 typedef struct _XFBX_MATERIAL
 {
	 char* Name;
	 XMFLOAT4_ VertexColor;
	 float Power;
	 XMFLOAT3_ SpecularColor;
	 XMFLOAT3_ EmissiveColor;
	 char* TextureFilename;
 } XFBX_MATERIAL;

 typedef struct _XFBX_MESH_MATERIALS
 {	
	 XFBX_MATERIAL* Material;
	 int* MeshIndices;
	 int MeshIndexCount;
 } XFBX_MESH_MATERIALS;

 typedef struct _XFBX_MESH
 {	 
	 XMFLOAT3_* Vertices;
	 int VertexCount;
	 int* Indices;	
	 int IndexCount;
	 XMFLOAT2_* TextureCoords;
	 int TextureCoordCount;
	 XFBX_SKIN_WEIGHTS* SkinWeightsCollection;
	 int SkinWeightsCount;
	 XFBX_MESH_MATERIALS* MeshMaterials;
 } XFBX_MESH;

 typedef struct _XFBX_NODE
 {
	 char* Name;
	 float m[16];
	 XFBX_MESH *Mesh;
	 struct _XFBX_NODE* Children;
	 int ChildCount; 
	 XFBX_ANIMATION_TAKE* AnimationTakes;
	 int TakeCount;
 } XFBX_NODE;

 typedef struct _XFBX_SCENE
 {	 
	 XFBX_NODE* Nodes;
	 int NodeCount;
 } XFBX_SCENE;
 
 class XFbxKeyframe
{
public:
	long Time;
	int Flags;
	XMFLOAT3_ Transform;
	XMFLOAT4_ Rotation;
	XMFLOAT3_ Scale;

public:
	XFbxKeyframe() : Time(0), Flags(0)
	{
		::ZeroMemory(&Transform, sizeof(XMFLOAT3_));
		::ZeroMemory(&Rotation, sizeof(XMFLOAT4));
		::ZeroMemory(&Scale, sizeof(XMFLOAT3));
	}
};

typedef std::map<long, XFbxKeyframe*> XFbxAnimationLayer;

typedef std::list<XFbxAnimationLayer*> XFbxAnimationTake;

class XFbxSkinWeights
{
public:
	string TransformNodeName;
	XMMATRIX Influence;
	
private:
	int _weightCount;
	float* _weightBuffer;
	int* _vertexIndices;

public:
	XFbxSkinWeights() : _weightBuffer(NULL), _weightCount(0), _vertexIndices(NULL), TransformNodeName(NULL)
	{				
		Influence = ::XMMatrixIdentity();
	}

	~XFbxSkinWeights() 
	{
		if(_weightBuffer != NULL)
		{
			::LocalFree(_weightBuffer);
			_weightBuffer = NULL;
			::LocalFree(_vertexIndices);
			_vertexIndices = NULL;
		}
	}

public:
	inline void DetachWeightBuffers() { _weightBuffer = NULL; _vertexIndices = NULL; _weightCount = 0; }	

	//Property: WeightCount
	///////////////////////
	int Get_WeightCount() { return _weightCount; }
	void Set_WeightCount(int value) { _weightCount = value;  }
	__declspec( property(get = Get_WeightCount, put = Set_WeightCount)) int WeightCount;

	//Property: WeightBuffer
	////////////////////////
	float* Get_WeightBuffer() { return _weightBuffer; }
	void Set_WeightBuffer(float *pValue) { _weightBuffer = pValue; }
	__declspec( property(get = Get_WeightBuffer, put = Set_WeightBuffer)) float* WeightBuffer;

	//Property: VertexIndices
	/////////////////////////
	int* Get_VertexIndices() { return _vertexIndices; }
	void Set_VertexIndices(int *pValue) { _vertexIndices = pValue; }
	__declspec( property(get = Get_VertexIndices, put = Set_VertexIndices)) int* VertexIndices;
};

class XFbxMaterial
{
public:
	string Name;
	XMFLOAT4_ VertexColor;
	float Power;
	XMFLOAT3_ Specular;
	XMFLOAT3_ EmissiveColor;
	string TextureFilename;
public:
	XFbxMaterial() : Name(nullptr), Power(0), TextureFilename(nullptr)
	{
		::ZeroMemory(&VertexColor, sizeof(XMFLOAT4_));
		::ZeroMemory(&Specular, sizeof(XMFLOAT3_));
		::ZeroMemory(&EmissiveColor, sizeof(_XMFLOAT3_));
	}
};

 class XFbxMesh
{
	//Fields
private:		
	int _vertexCount;
	XMFLOAT3_* _pVertexBuffer;
	int _indexCount;
	int* _pIndexBuffer;	
	int _texCoordCount;
	XMFLOAT2_* _pTexCoords;
	std::list<XFbxSkinWeights*>* _skinWeights;

	//Static Methods
public:
	//static XFbxMesh* CreateFromFile( const wchar_t* fileName );

	inline void DetachVertexBuffer() { _pVertexBuffer = NULL; _vertexCount = 0; }

	inline void DetachIndexBuffer() { _pIndexBuffer = NULL; _indexCount = 0; }

	inline void DetachTexCoordBuffer() { _pTexCoords = NULL; _texCoordCount = 0; }

	//Constructors/Destructor
public:
	XFbxMesh() : _pVertexBuffer(NULL), _pIndexBuffer(NULL), _pTexCoords(NULL), _vertexCount(0), _indexCount(0), _texCoordCount(0)
	{	
		_skinWeights = new std::list<XFbxSkinWeights*>();
	}

	~XFbxMesh()
	{
		if(_pVertexBuffer != NULL)
		{
			::LocalFree(_pVertexBuffer);
			_pVertexBuffer =NULL;
		}

		if(_pIndexBuffer != NULL)
		{
			::LocalFree(_pIndexBuffer);
			_pIndexBuffer = NULL;
		}

		if(_pTexCoords != NULL)
		{
			::LocalFree(_pTexCoords);
			_pTexCoords = NULL;
		}		

		 std::list<XFbxSkinWeights*>::iterator iterator = _skinWeights->begin();
		 while(iterator != _skinWeights->end())
		 {
			 delete (*iterator);
			 iterator++;
		 }
		 _skinWeights->clear();
		 delete _skinWeights;
		 _skinWeights = nullptr;
	}

	//Properties
public:
#pragma region Properties

	//Property: VertexBuffer
	////////////////////////
	XMFLOAT3_* Get_VertexBuffer() { return _pVertexBuffer; }
	void Set_VertexBuffer(XMFLOAT3_ *pValue) { _pVertexBuffer = pValue;  }
	__declspec( property(get = Get_VertexBuffer, put = Set_VertexBuffer)) XMFLOAT3_* VertexBuffer;

	//Property: VertexCount
	///////////////////////
	int Get_VertexCount() { return _vertexCount; }
	void Set_VertexCount(int value) { _vertexCount = value;  }
	__declspec( property(get = Get_VertexCount, put = Set_VertexCount)) int VertexCount;

	//Property: IndexBuffer
	///////////////////////
	int* Get_IndexBuffer() { return _pIndexBuffer; }
	void Set_IndexBuffer(int *pValue) { _pIndexBuffer = pValue; }
	__declspec( property(get = Get_IndexBuffer, put = Set_IndexBuffer)) int* IndexBuffer;

	//Property: IndexCount
	//////////////////////
	int Get_IndexCount() { return _indexCount; }
	void Set_IndexCount(int value) { _indexCount = value;  }
	__declspec( property(get = Get_IndexCount, put = Set_IndexCount)) int IndexCount;	
	
	//Property: TexCoords
	//////////////////////
	XMFLOAT2_* Get_TexCoords() { return _pTexCoords; }
	void Set_TexCoords(XMFLOAT2_ *pValue) { _pTexCoords = pValue;  }
	__declspec( property(get = Get_TexCoords, put = Set_TexCoords)) XMFLOAT2_* TexCoords;

	//Property: TexCoordCount
	/////////////////////////
	int Get_TexCoordCount() { return _texCoordCount; }
	void Set_TexCoordCount(int value) { _texCoordCount = value;  }
	__declspec( property(get = Get_TexCoordCount, put = Set_TexCoordCount)) int TexCoordCount;

	//Property: SkinWeights
	///////////////////////
	std::list<XFbxSkinWeights*>* Get_SkinWeights() { return _skinWeights; }
	__declspec( property(get = Get_SkinWeights)) std::list<XFbxSkinWeights*>* SkinWeights;

#pragma endregion 	
};

 class XFbxNode
{
	//Fields
private:
	XFbxMesh *_pMesh;

	//Public Fields
public:
	string Name;
	XMMATRIX Transform;
	XFbxNode* Parent;	

	//Fields
private:			
	std::list<XFbxNode*>* _children;
	std::list<XFbxAnimationTake*>* _animationTakes;	
	
	//Constructors/Destructor
public:
	XFbxNode() : Name(nullptr), Parent(nullptr), _pMesh(nullptr)
	{
		_children = new std::list<XFbxNode*>();		
		Transform = ::XMMatrixIdentity();		
		_animationTakes = new std::list<XFbxAnimationTake*>();		
	}

	~XFbxNode()
	{
		if(_pMesh != nullptr)
		{
			delete _pMesh;
			_pMesh = nullptr;
		}

		std::list<XFbxNode*>::iterator iterator = _children->begin();
		while(iterator != _children->end())
		{
			if( (*iterator) != nullptr)
				delete (*iterator);
			iterator++;
		}
		_children->clear();
		delete _children;
		_children = nullptr;

		std::list<XFbxAnimationTake*>::iterator takeIterator = _animationTakes->begin();
		while(takeIterator != _animationTakes->end())
		{
			XFbxAnimationTake* take = (*takeIterator);
			XFbxAnimationTake::iterator layerIterator = take->begin();
			while (layerIterator != take->end())
			{
				XFbxAnimationLayer* layer = (*layerIterator);
				XFbxAnimationLayer::iterator keyframeIterator = layer->begin();
				while(keyframeIterator != layer->end())
				{
					if( (*keyframeIterator).second != nullptr )
						delete (*keyframeIterator).second;
					keyframeIterator++;		
				}
				layer->clear();
				delete layer;
				layer = nullptr;
			}
			take->clear();
			delete take;
			take = nullptr;
		}
		_animationTakes->clear();
		delete _animationTakes;
		_animationTakes = nullptr;
	}

	//Properties
public:
#pragma region Properties

	//Property: Mesh
	////////////////
	XFbxMesh* Get_Mesh() { return _pMesh; }
	void Set_Mesh(XFbxMesh* value) { _pMesh = value; }
	__declspec( property(get = Get_Mesh, put = Set_Mesh)) XFbxMesh* Mesh;

	//Property: Children
	////////////////////
	std::list<XFbxNode*>* Get_Children() { return _children; }
	__declspec( property(get = Get_Children)) std::list<XFbxNode*>* Children;

	//Property: AnimationTakes
	//////////////////////////
	std::list<XFbxAnimationTake*>* Get_AnimationTakes() { return _animationTakes; }
	__declspec( property(get = Get_AnimationTakes)) std::list<XFbxAnimationTake*>* AnimationTakes;

#pragma endregion 	
};

 class XFbxScene
 {
	 //Fields
 private:
	 std::list<XFbxNode*>* _nodes;

	 //Constructors
 public:
	 XFbxScene() 
	 {
		 _nodes = new std::list<XFbxNode*>();
	 }

	 ~XFbxScene()
	 {
		 std::list<XFbxNode*>::iterator nodeIterator = _nodes->begin();
		 while(nodeIterator != _nodes->end())
		 {
			 delete (*nodeIterator);
			 nodeIterator++;
		 }
		 _nodes->clear();
		 delete _nodes;
		 _nodes = nullptr;
	 }

	 //Methods
public:
	XFBX_SCENE* ConvertToMarshableType(bool detachBuffers);

	//Static Methods
public:
	static XFbxScene* CreateFromFile( const wchar_t* fileName );	

	//Properties
 public:
	 //Property: Nodes
	//////////////////
	std::list<XFbxNode*>* Get_Nodes() { return _nodes; }
	__declspec( property(get = Get_Nodes)) std::list<XFbxNode*>* Nodes;
 };