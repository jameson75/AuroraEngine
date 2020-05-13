// FBXWrapper.cpp : Defines the exported functions for the DLL application.
//
#include "stdafx.h"
#include "FBXWrapper.h"
#include <DirectXMath.h>
#include <d3d11.h>
#include <iostream>

using namespace DirectX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

class XFbxUVSet
{
public:
	string Name;
	std::list<XMFLOAT2_> UVCoords;
};

#pragma region FBX SDK Sample GetPosition.cxx Code

// Get the matrix of the given pose
FbxAMatrix GetPoseMatrix(FbxPose* pPose, int pNodeIndex)
{
    FbxAMatrix lPoseMatrix;
    FbxMatrix lMatrix = pPose->GetMatrix(pNodeIndex);

    memcpy((double*)lPoseMatrix, (double*)lMatrix, sizeof(lMatrix.mData));

    return lPoseMatrix;
}

// Get the geometry offset to a node. It is never inherited by the children.
FbxAMatrix GetGeometry(FbxNode* pNode)
{
    const FbxVector4 lT = pNode->GetGeometricTranslation(FbxNode::eSourcePivot);
    const FbxVector4 lR = pNode->GetGeometricRotation(FbxNode::eSourcePivot);
    const FbxVector4 lS = pNode->GetGeometricScaling(FbxNode::eSourcePivot);
    return FbxAMatrix(lT, lR, lS);
}

// Get the global position of the node for the current pose.
// If the specified node is not part of the pose or no pose is specified, get its
// global position at the current time.
FbxAMatrix GetGlobalPosition(FbxNode* pNode, const FbxTime& pTime, FbxPose* pPose = NULL, FbxAMatrix* pParentGlobalPosition = NULL)
{
    FbxAMatrix lGlobalPosition;
    bool        lPositionFound = false;

    if (pPose)
    {
        int lNodeIndex = pPose->Find(pNode);

        if (lNodeIndex > -1)
        {
            // The bind pose is always a global matrix.
            // If we have a rest pose, we need to check if it is
            // stored in global or local space.
            if (pPose->IsBindPose() || !pPose->IsLocalMatrix(lNodeIndex))
            {
                lGlobalPosition = GetPoseMatrix(pPose, lNodeIndex);
            }
            else
            {
                // We have a local matrix, we need to convert it to
                // a global space matrix.
                FbxAMatrix lParentGlobalPosition;

                if (pParentGlobalPosition)
                {
                    lParentGlobalPosition = *pParentGlobalPosition;
                }
                else
                {
                    if (pNode->GetParent())
                    {
                        lParentGlobalPosition = GetGlobalPosition(pNode->GetParent(), pTime, pPose);
                    }
                }

                FbxAMatrix lLocalPosition = GetPoseMatrix(pPose, lNodeIndex);
                lGlobalPosition = lParentGlobalPosition * lLocalPosition;
            }

            lPositionFound = true;
        }
    }

    if (!lPositionFound)
    {
        // There is no pose entry for that node, get the current global position instead.

        // Ideally this would use parent global position and local position to compute the global position.
        // Unfortunately the equation 
        //    lGlobalPosition = pParentGlobalPosition * lLocalPosition
        // does not hold when inheritance type is other than "Parent" (RSrs).
        // To compute the parent rotation and scaling is tricky in the RrSs and Rrs cases.
        lGlobalPosition = pNode->EvaluateGlobalTransform(pTime);
    }

    return lGlobalPosition;
}
#pragma endregion

#pragma region FBX SDK Sample DrawScene.cxx Code
//Compute the transform matrix that the cluster will transform the vertex.
void ComputeClusterDeformation(FbxAMatrix& pGlobalPosition, 
							   FbxMesh* pMesh,
							   FbxCluster* pCluster, 
							   FbxAMatrix& pVertexTransformMatrix,
							   FbxTime pTime, 
							   FbxPose* pPose)
{
    FbxCluster::ELinkMode lClusterMode = pCluster->GetLinkMode();

	FbxAMatrix lReferenceGlobalInitPosition;
	FbxAMatrix lReferenceGlobalCurrentPosition;
	FbxAMatrix lAssociateGlobalInitPosition;
	FbxAMatrix lAssociateGlobalCurrentPosition;
	FbxAMatrix lClusterGlobalInitPosition;
	FbxAMatrix lClusterGlobalCurrentPosition;

	FbxAMatrix lReferenceGeometry;
	FbxAMatrix lAssociateGeometry;
	FbxAMatrix lClusterGeometry;

	FbxAMatrix lClusterRelativeInitPosition;
	FbxAMatrix lClusterRelativeCurrentPositionInverse;
	
	if (lClusterMode == FbxCluster::eAdditive && pCluster->GetAssociateModel())
	{
		pCluster->GetTransformAssociateModelMatrix(lAssociateGlobalInitPosition);
		// Geometric transform of the model
		lAssociateGeometry = GetGeometry(pCluster->GetAssociateModel());
		lAssociateGlobalInitPosition *= lAssociateGeometry;
		lAssociateGlobalCurrentPosition = GetGlobalPosition(pCluster->GetAssociateModel(), pTime, pPose);

		pCluster->GetTransformMatrix(lReferenceGlobalInitPosition);
		// Multiply lReferenceGlobalInitPosition by Geometric Transformation
		lReferenceGeometry = GetGeometry(pMesh->GetNode());
		lReferenceGlobalInitPosition *= lReferenceGeometry;
		lReferenceGlobalCurrentPosition = pGlobalPosition;

		// Get the link initial global position and the link current global position.
		pCluster->GetTransformLinkMatrix(lClusterGlobalInitPosition);
		// Multiply lClusterGlobalInitPosition by Geometric Transformation
		lClusterGeometry = GetGeometry(pCluster->GetLink());
		lClusterGlobalInitPosition *= lClusterGeometry;
		lClusterGlobalCurrentPosition = GetGlobalPosition(pCluster->GetLink(), pTime, pPose);

		// Compute the shift of the link relative to the reference.
		//ModelM-1 * AssoM * AssoGX-1 * LinkGX * LinkM-1*ModelM
		pVertexTransformMatrix = lReferenceGlobalInitPosition.Inverse() * lAssociateGlobalInitPosition * lAssociateGlobalCurrentPosition.Inverse() *
			lClusterGlobalCurrentPosition * lClusterGlobalInitPosition.Inverse() * lReferenceGlobalInitPosition;
	}
	else
	{
		pCluster->GetTransformMatrix(lReferenceGlobalInitPosition);
		lReferenceGlobalCurrentPosition = pGlobalPosition;
		// Multiply lReferenceGlobalInitPosition by Geometric Transformation
		lReferenceGeometry = GetGeometry(pMesh->GetNode());
		lReferenceGlobalInitPosition *= lReferenceGeometry;

		// Get the link initial global position and the link current global position.
		pCluster->GetTransformLinkMatrix(lClusterGlobalInitPosition);
		lClusterGlobalCurrentPosition = GetGlobalPosition(pCluster->GetLink(), pTime, pPose);

		// Compute the initial position of the link relative to the reference.
		lClusterRelativeInitPosition = lClusterGlobalInitPosition.Inverse() * lReferenceGlobalInitPosition;

		// Compute the current position of the link relative to the reference.
		lClusterRelativeCurrentPositionInverse = lReferenceGlobalCurrentPosition.Inverse() * lClusterGlobalCurrentPosition;

		// Compute the shift of the link relative to the reference.
		pVertexTransformMatrix = lClusterRelativeCurrentPositionInverse * lClusterRelativeInitPosition;
	}
}
#pragma endregion 

#pragma region FBX SDK Sample UVSample project Code (slightly modified to return uv array).
void LoadUVInformation(FbxMesh* pMesh)
{
    //get all UV set names
    FbxStringList lUVSetNameList;
    pMesh->GetUVSetNames(lUVSetNameList);

    //iterating over all uv sets
    for (int lUVSetIndex = 0; lUVSetIndex < lUVSetNameList.GetCount(); lUVSetIndex++)
    {
		//get lUVSetIndex-th uv set
        const char* lUVSetName = lUVSetNameList.GetStringAt(lUVSetIndex);
        const FbxGeometryElementUV* lUVElement = pMesh->GetElementUV(lUVSetName);

        if(!lUVElement)
            continue;

        // only support mapping mode eByPolygonVertex and eByControlPoint
        if( lUVElement->GetMappingMode() != FbxGeometryElement::eByPolygonVertex &&
            lUVElement->GetMappingMode() != FbxGeometryElement::eByControlPoint )
            return;

        //index array, where holds the index referenced to the uv data
        const bool lUseIndex = lUVElement->GetReferenceMode() != FbxGeometryElement::eDirect;
        const int lIndexCount= (lUseIndex) ? lUVElement->GetIndexArray().GetCount() : 0;

        //iterating through the data by polygon
        const int lPolyCount = pMesh->GetPolygonCount();

        if( lUVElement->GetMappingMode() == FbxGeometryElement::eByControlPoint )
        {
            for( int lPolyIndex = 0; lPolyIndex < lPolyCount; ++lPolyIndex )
            {
                // build the max index array that we need to pass into MakePoly
                const int lPolySize = pMesh->GetPolygonSize(lPolyIndex);
                for( int lVertIndex = 0; lVertIndex < lPolySize; ++lVertIndex )
                {
                    FbxVector2 lUVValue;

                    //get the index of the current vertex in control points array
                    int lPolyVertIndex = pMesh->GetPolygonVertex(lPolyIndex,lVertIndex);

                    //the UV index depends on the reference mode
                    int lUVIndex = lUseIndex ? lUVElement->GetIndexArray().GetAt(lPolyVertIndex) : lPolyVertIndex;

                    lUVValue = lUVElement->GetDirectArray().GetAt(lUVIndex);					
                }
            }
        }
        else if (lUVElement->GetMappingMode() == FbxGeometryElement::eByPolygonVertex)
        {
            int lPolyIndexCounter = 0;
            for( int lPolyIndex = 0; lPolyIndex < lPolyCount; ++lPolyIndex )
            {
                // build the max index array that we need to pass into MakePoly
                const int lPolySize = pMesh->GetPolygonSize(lPolyIndex);
                for( int lVertIndex = 0; lVertIndex < lPolySize; ++lVertIndex )
                {
                    if (lPolyIndexCounter < lIndexCount)
                    {
                        FbxVector2 lUVValue;

                        //the UV index depends on the reference mode
                        int lUVIndex = lUseIndex ? lUVElement->GetIndexArray().GetAt(lPolyIndexCounter) : lPolyIndexCounter;

                        lUVValue = lUVElement->GetDirectArray().GetAt(lUVIndex);

                        //User TODO:
                        //Print out the value of UV(lUVValue) or log it to a file

                        lPolyIndexCounter++;
                    }
                }
            }
        }
    }
}
#pragma endregion

#pragma region Helpers

void ConstructXFloat3_(float x, float y, float z, XMFLOAT3_* v)
{
	v->X = x;
	v->Y = y;
	v->Z = z;
}

void WcharToCharString(char* destination, const wchar_t* source, int length) 
{
	char* pDestinationTraverser = destination;
	const wchar_t* pSourceTraverser = source;
	for(int i = 0; i < length; i++)
	{
		if( *pSourceTraverser >= 256 )
			throw std::exception("source string could not be recognized.");
		(*pDestinationTraverser) = (char)(*pSourceTraverser);
		pDestinationTraverser++;
		pSourceTraverser++;
	}
}		

static XFbxKeyframe* GetOrCreateNodeKeyFrame(FbxAnimCurve* pCurve, int animationKeyIndex, XFbxNode *pXNode)
{
	FbxTime keyTime = pCurve->KeyGetTime(animationKeyIndex);	
	long mKeyTime = static_cast<long>(keyTime.GetMilliSeconds());	
    XFbxKeyframe* keyFrame = nullptr;	
	XFbxAnimationLayer* pLastLayer = (*pXNode->AnimationTakes->end())->back();
	std::map<long, XFbxKeyframe*>::iterator iterator = pLastLayer->find(mKeyTime);	

	if( iterator == pLastLayer->end() )
		keyFrame = (*iterator).second;
	else 
	{
		keyFrame = new XFbxKeyframe();
		pLastLayer->insert(std::pair<long, XFbxKeyframe*>(mKeyTime, keyFrame));
	}

	return keyFrame;
}


void ThrowOnUnexpectedSkinAttributes(FbxSkin* pSkinDeformer)
{
	FbxSkin::EType skinType = pSkinDeformer->GetSkinningType();	
	FbxCluster::ELinkMode linkMode = pSkinDeformer->GetCluster(0)->GetLinkMode();	
	if( linkMode != FbxCluster::ELinkMode::eNormalize || skinType != FbxSkin::EType::eRigid )
	{
		throw std::exception("Unexpected fbx skin type or link mode.");
	}							
}

#pragma endregion

#pragma region Core Implementation

static void ProcessMeshAnimation(FbxNode* pNode, XFbxNode *pXNode)
{
	FbxScene *pScene = pNode->GetScene();	
	int animationStackCount = pScene->GetSrcObjectCount<FbxAnimStack>();
	for( int i = 0; i < animationStackCount; i++ )
	{
		FbxAnimStack* pAnimationStack = pScene->GetSrcObject<FbxAnimStack>(i);
		XFbxAnimationTake* newAnimationTake = new XFbxAnimationTake();
		pXNode->AnimationTakes->push_back(newAnimationTake);		
	
		pScene->SetCurrentAnimationStack(pAnimationStack);

		int nLayers = pAnimationStack->GetMemberCount<FbxAnimLayer>();
		for( int j = 0; j < nLayers; j++)
		{			
			FbxAnimLayer *pAnimationLayer = pAnimationStack->GetMember<FbxAnimLayer>(j);
			FbxAnimCurve *pAnimationCurve = nullptr;
			newAnimationTake->push_back(new XFbxAnimationLayer());
			
			const char* name = pNode->GetName();
			//Initialize mesh key frame with translation.X values.			
			pAnimationCurve = pNode->LclTranslation.GetCurve(pAnimationLayer, FBXSDK_CURVENODE_COMPONENT_X);
			if(pAnimationCurve != nullptr)
			{
				int nKeyCount = pAnimationCurve->KeyGetCount();
				for( int k = 0; k < nKeyCount; nKeyCount++ )
				{
					XFbxKeyframe* keyFrame = GetOrCreateNodeKeyFrame(pAnimationCurve, k, pXNode);
					keyFrame->Transform.X = pAnimationCurve->KeyGetValue(k);
				}
			}

			//Initialize mesh key frame with translation.Y values.
			pAnimationCurve = pNode->LclTranslation.GetCurve(pAnimationLayer, FBXSDK_CURVENODE_COMPONENT_Y);
			if(pAnimationCurve != nullptr)
			{
				int nKeyCount = pAnimationCurve->KeyGetCount();
				for( int k = 0; k < nKeyCount; nKeyCount++ )
				{
					XFbxKeyframe* keyFrame = GetOrCreateNodeKeyFrame(pAnimationCurve, k, pXNode);
					keyFrame->Transform.Y = pAnimationCurve->KeyGetValue(k);
				}
			}

			//Initialize mesh key frame with translation.Z values.
			pAnimationCurve = pNode->LclTranslation.GetCurve(pAnimationLayer, FBXSDK_CURVENODE_COMPONENT_Z);
			if(pAnimationCurve != nullptr)
			{
				int nKeyCount = pAnimationCurve->KeyGetCount();
				for( int k = 0; k < nKeyCount; nKeyCount++ )
				{
					XFbxKeyframe* keyFrame = GetOrCreateNodeKeyFrame(pAnimationCurve, k, pXNode);
					keyFrame->Transform.Z = pAnimationCurve->KeyGetValue(k);
				}
			}

			//Initialize mesh key frame with rotation.X values.
			pAnimationCurve = pNode->LclRotation.GetCurve(pAnimationLayer, FBXSDK_CURVENODE_COMPONENT_X);
			if(pAnimationCurve != nullptr)
			{
				int nKeyCount = pAnimationCurve->KeyGetCount();
				for( int k = 0; k < nKeyCount; nKeyCount++ )
				{
					XFbxKeyframe* keyFrame = GetOrCreateNodeKeyFrame(pAnimationCurve, k, pXNode);
					keyFrame->Rotation.X = pAnimationCurve->KeyGetValue(k);
				}
			}

			//Initialize mesh key frame with translation.Y values.
			pAnimationCurve = pNode->LclRotation.GetCurve(pAnimationLayer, FBXSDK_CURVENODE_COMPONENT_Y);
			if(pAnimationCurve != nullptr)
			{
				int nKeyCount = pAnimationCurve->KeyGetCount();
				for( int k = 0; k < nKeyCount; nKeyCount++ )
				{
					XFbxKeyframe* keyFrame = GetOrCreateNodeKeyFrame(pAnimationCurve, k, pXNode);
					keyFrame->Rotation.Y = pAnimationCurve->KeyGetValue(k);
				}
			}

			//Initialize mesh key frame with translation.Z values.
			pAnimationCurve = pNode->LclRotation.GetCurve(pAnimationLayer, FBXSDK_CURVENODE_COMPONENT_Z);
			if(pAnimationCurve != nullptr)
			{
				int nKeyCount = pAnimationCurve->KeyGetCount();
				for( int k = 0; k < nKeyCount; nKeyCount++ )
				{
					XFbxKeyframe* keyFrame = GetOrCreateNodeKeyFrame(pAnimationCurve, k, pXNode);
					keyFrame->Rotation.Z = pAnimationCurve->KeyGetValue(k);
				}
			}

				//Initialize mesh key frame with rotation.X values.
			pAnimationCurve = pNode->LclScaling.GetCurve(pAnimationLayer, FBXSDK_CURVENODE_COMPONENT_X);
			if(pAnimationCurve != nullptr)
			{
				int nKeyCount = pAnimationCurve->KeyGetCount();
				for( int k = 0; k < nKeyCount; nKeyCount++ )
				{
					XFbxKeyframe* keyFrame = GetOrCreateNodeKeyFrame(pAnimationCurve, k, pXNode);
					keyFrame->Scale.X = pAnimationCurve->KeyGetValue(k);
				}
			}

			//Initialize mesh key frame with translation.Y values.
			pAnimationCurve = pNode->LclScaling.GetCurve(pAnimationLayer, FBXSDK_CURVENODE_COMPONENT_Y);
			if(pAnimationCurve != nullptr)
			{
				int nKeyCount = pAnimationCurve->KeyGetCount();
				for( int k = 0; k < nKeyCount; nKeyCount++ )
				{
					XFbxKeyframe* keyFrame = GetOrCreateNodeKeyFrame(pAnimationCurve, k, pXNode);
					keyFrame->Scale.Y = pAnimationCurve->KeyGetValue(k);
				}
			}

			//Initialize mesh key frame with translation.Z values.
			pAnimationCurve = pNode->LclScaling.GetCurve(pAnimationLayer, FBXSDK_CURVENODE_COMPONENT_Z);
			if(pAnimationCurve != nullptr)
			{
				int nKeyCount = pAnimationCurve->KeyGetCount();
				for( int k = 0; k < nKeyCount; nKeyCount++ )
				{
					XFbxKeyframe* keyFrame = GetOrCreateNodeKeyFrame(pAnimationCurve, k, pXNode);
					keyFrame->Scale.Z = pAnimationCurve->KeyGetValue(k);
				}
			}
		}
	}
}

static void BuildXSceneNodeTree(FbxNode *pNode, XFbxNode *pXNode /*, FbxMatrix adjustedMeshTransform*/)
{
	FbxScene* pScene = pNode->GetScene();	

	//Process Frame Transformation
	//-----------------------------
	FbxMatrix localTransform = /*adjustedMeshTransform **/ pNode->EvaluateLocalTransform();					
	for(int m = 0; m < 4; m++)					
		pXNode->Transform.r[m] = XMVectorSet((float)localTransform.GetRow(m)[0], (float)localTransform.GetRow(m)[1], (float)localTransform.GetRow(m)[2], (float)localTransform.GetRow(m)[3]);					

	int attributeCount = pNode->GetNodeAttributeCount();	

	for( int j = 0; j < attributeCount; j++ )
	{
		FbxNodeAttribute *pAttribute = pNode->GetNodeAttributeByIndex(j);
		if( pAttribute->GetAttributeType() == FbxNodeAttribute::EType::eMesh )
		{
			FbxMesh *pMesh = static_cast<FbxMesh*>(pAttribute);		
			XFbxMesh *pXMesh = new XFbxMesh();
			
			//if( pMeshAttribute->IsTriangleMesh()) //Seems buggy... didn't work for simple cube export - returned false for 8 control points and 36 vertices.
			if(true)
			{						
				//Process Vertices
				//----------------
				FbxVector4* pControlPointTraverser = pMesh->GetControlPoints();
				int nControlPoints = pMesh->GetControlPointsCount();
				//NOTE: We use ::LocalAlloc (instead of malloc) so that this memory can be SAFELY freed inside a .NET runtime using System.Runtime.InteropServices.Marshal.FreeHGlobal
				pXMesh->VertexBuffer = static_cast<XMFLOAT3_*>(::LocalAlloc(0, sizeof(XMFLOAT3_) * nControlPoints));
				pXMesh->VertexCount = nControlPoints;
				XMFLOAT3_ *pVertexTraverser = pXMesh->VertexBuffer;					
				for( int k = 0; k < nControlPoints; k++ )
				{
					ConstructXFloat3_((float)pControlPointTraverser->Buffer()[0], (float)pControlPointTraverser->Buffer()[1], (float)pControlPointTraverser->Buffer()[2], pVertexTraverser);
					pVertexTraverser++;
					pControlPointTraverser++;
				}			

				//Process Indices
				//---------------
				int* pControlPointIndexTraverser = pMesh->GetPolygonVertices();		
				int indexCount = pMesh->GetPolygonVertexCount();								
				//NOTE: We use ::LocalAlloc (instead of malloc) so that this memory can be SAFELY freed inside a .NET runtime using System.Runtime.InteropServices.Marshal.FreeHGlobal
				pXMesh->IndexBuffer = static_cast<int*>(::LocalAlloc(0, sizeof(int) * indexCount));
				pXMesh->IndexCount = indexCount;
				int* pIndexTraverser = pXMesh->IndexBuffer;
				for(int k = 0; k < indexCount; k++)
				{
					(*pIndexTraverser) = (*pControlPointIndexTraverser);
					pControlPointIndexTraverser++;
					pIndexTraverser++;
				}

				//Process Skin/Bones
				//------------------
				int deformerCount = pMesh->GetDeformerCount();
				for(int k = 0; k < deformerCount; k++)
				{
					FbxDeformer* pDeformer = pMesh->GetDeformer(k);
					if( pDeformer->GetDeformerType() == FbxDeformer::eSkin )
					{
						FbxSkin* pSkinDeformer = (FbxSkin*)pDeformer;	
#ifdef _DEBUG
						ThrowOnUnexpectedSkinAttributes(pSkinDeformer);										
#endif
						int clusterCount = pSkinDeformer->GetClusterCount();											
						for(int m = 0; m < clusterCount; m++)
						{								
							FbxCluster* pCluster = pSkinDeformer->GetCluster(m);
							
							FbxAMatrix clusterTransform;
							FbxAMatrix meshGlobalPosition = pMesh->GetNode()->EvaluateGlobalTransform();
							::ComputeClusterDeformation(meshGlobalPosition, pMesh, pCluster, clusterTransform, FBXSDK_TIME_INFINITE, nullptr);
							XFbxSkinWeights* pXSkinWeights = new XFbxSkinWeights();
							for(int m = 0; m < 4; m++)					
								pXSkinWeights->Influence.r[m] = XMVectorSet((float)clusterTransform.GetRow(m)[0], (float)clusterTransform.GetRow(m)[1], (float)clusterTransform.GetRow(m)[2], (float)clusterTransform.GetRow(m)[3]);					 								
							int* pClusterVerticesTraverser = pCluster->GetControlPointIndices();
							int clusterVertexCount = pCluster->GetControlPointIndicesCount();
							//NOTE: We expect the number of weights to be the same as the number of vertices in a cluster.
							double* pClusterWeightsTraverser = pCluster->GetControlPointWeights();	
							pXSkinWeights->WeightBuffer = static_cast<float*>(::LocalAlloc(0, sizeof(float) * clusterVertexCount));
							pXSkinWeights->VertexIndices = static_cast<int*>(::LocalAlloc(0, sizeof(int) * clusterVertexCount));
							float *pWeightsTraverser = pXSkinWeights->WeightBuffer;
							int *pVertIndexTraverser = pXSkinWeights->VertexIndices;
							pXSkinWeights->WeightCount = clusterVertexCount;														
							for(int n = 0; n < clusterVertexCount; n++)
							{
								//*****************************************************************************************
								//NOTE: The SDK documentations warns that it's possible for there to be assigned weights
								//to vertices that don't (no longer) exist. So we check for that condition and
								//skip processing when it occurs.
								//*****************************************************************************************
								if(n >= nControlPoints)
									continue;													
								(*pWeightsTraverser) = (float)(*pClusterWeightsTraverser);
								(*pVertIndexTraverser) = (*pClusterVerticesTraverser);
								pClusterVerticesTraverser++;
								pClusterWeightsTraverser++;		
								pWeightsTraverser++;
								pVertIndexTraverser++;
							}								
							pXSkinWeights->TransformNodeName = pCluster->GetLink()->GetName();
							pXMesh->SkinWeights->push_front(pXSkinWeights);							
						}							
					}
				}				

				//TODO: Process Texture Coords.	

				//Process mesh material.				
				int nMaterials = pNode->GetMaterialCount();
				for( int k = 0; k < nMaterials; k++)
				{
					FbxSurfaceMaterial* pMaterial = pNode->GetMaterial(k);					
				}
			}
				
			else
			{
				//TODO: Report warning that this mesh was skipped becuase it did
				//not consist totally of triangular polygons.
			}				
		}		
	}	

	//Process Animation
	//-----------------
	::ProcessMeshAnimation(pNode, pXNode);	

	int childCount = pNode->GetChildCount();
	for( int i = 0; i < childCount ; i++ )
	{
		FbxNode* pChildNode = pNode->GetChild(i);		
		XFbxNode *pChildXNode = new XFbxNode();
		pXNode->Children->push_back(pChildXNode);
		pChildXNode->Parent = pXNode;
		BuildXSceneNodeTree(pChildNode, pChildXNode /*,adjustedMeshTransform*/);
	}
}

XFbxScene* XFbxScene::CreateFromFile( const wchar_t* fileName ) 
{
	const int NULL_TERMINATOR_LENGTH = 1;
	FbxManager *pSdkManager = FbxManager::Create();
	if( pSdkManager == NULL )
		//throw std::exception("SDK Manager could not be created");
			return NULL;

	int cFileNameSize = lstrlenW(fileName) + NULL_TERMINATOR_LENGTH;
	char* cFileName = static_cast<char*>(malloc(cFileNameSize));
	WcharToCharString(cFileName, fileName, cFileNameSize);
	FbxIOSettings* pIOSettings = FbxIOSettings::Create(pSdkManager, IOSROOT);
	
	pSdkManager->SetIOSettings(pIOSettings);	

	FbxImporter *pImporter = FbxImporter::Create(pSdkManager, "");

	if( !pImporter->Initialize( cFileName, -1, pSdkManager->GetIOSettings()))
		//throw std::exception("FBX Importer failed to initialize.");
			return NULL;
	
	FbxScene* pScene = FbxScene::Create(pSdkManager, "X Scene");
	pImporter->Import(pScene);

	//FbxAxisSystem::DirectX.ConvertScene(pScene); //Doesn't work as expected.	
	
	XFbxNode* pXRootNode = new XFbxNode();
	BuildXSceneNodeTree(pScene->GetRootNode(), pXRootNode /*, FbxMatrix()*/);	
	
	pScene->Destroy();
	pScene = NULL;

	pImporter->Destroy();
	pImporter = NULL;

	pIOSettings->Destroy();
	pIOSettings = NULL;

	pSdkManager->Destroy();
	pSdkManager = NULL;

	free(cFileName);
	cFileName = NULL;

	//Commented out this line because it results in a memory leak (from root mesh not being deleted).
	//return ( pMesh->Children->size() == 1 ) ? pMesh->Children->front() : pMesh;

	XFbxScene* pXScene = new XFbxScene();
	pXScene->Nodes->push_back(pXRootNode);
	
	return pXScene;
}

void ConvertToMarshableType(XFbxNode* pXNode, XFBX_NODE* pXMNode, bool detachBuffers)
{
	//Name...
	pXMNode->Name = static_cast<char*>(::LocalAlloc(0, pXNode->Name.length() + 1));
	::memcpy(pXMNode->Name, pXNode->Name.c_str(), pXNode->Name.length() + 1);

	//m (transform)...
	for(int i = 0; i < 4; i++)
		for(int j = 0; j < 4; j++)
			pXMNode->m[i * 4 + j] = DirectX::XMVectorGetByIndex(pXNode->Transform.r[i], j);

	pXMNode->Mesh = static_cast<XFBX_MESH*>(::LocalAlloc(0, sizeof(XFBX_MESH)));
	
	//Vertex Count...
	pXMNode->Mesh->VertexCount = pXNode->Mesh->VertexCount;
	//Index Count...
	pXMNode->Mesh->IndexCount = pXNode->Mesh->IndexCount;
	//TexCoord Count...
	pXMNode->Mesh->TextureCoordCount =pXNode->Mesh->TexCoordCount;

	if(!detachBuffers)
	{
		//Vertices...
		pXMNode->Mesh->Vertices = static_cast<XMFLOAT3_*>(::LocalAlloc(0, sizeof(XMFLOAT3_) * pXMNode->Mesh->VertexCount));
		::memcpy(pXMNode->Mesh->Vertices, pXNode->Mesh->VertexBuffer, sizeof(XMFLOAT3_) * pXMNode->Mesh->VertexCount);
		//Indices...
		pXMNode->Mesh->Indices = static_cast<int*>(::LocalAlloc(0, sizeof(int) * pXMNode->Mesh->IndexCount));
		::memcpy(pXMNode->Mesh->Indices, pXNode->Mesh->IndexBuffer, sizeof(int) * pXMNode->Mesh->IndexCount);
		//TexCoords...
		pXMNode->Mesh->TextureCoords = static_cast<XMFLOAT2_*>(::LocalAlloc(0, sizeof(XMFLOAT2_) * pXMNode->Mesh->TextureCoordCount));
		::memcpy(pXMNode->Mesh->TextureCoords, pXNode->Mesh->TexCoords, sizeof(XMFLOAT2_) * pXMNode->Mesh->TextureCoordCount);
	}
	else 
	{
		//Vertices...
		pXMNode->Mesh->Vertices = pXNode->Mesh->VertexBuffer;
		pXNode->Mesh->DetachVertexBuffer();
		//Indices...
		pXMNode->Mesh->Indices = pXNode->Mesh->IndexBuffer;
		pXNode->Mesh->DetachIndexBuffer();
		//TexCoords...
		pXMNode->Mesh->TextureCoords = pXNode->Mesh->TexCoords;
		pXNode->Mesh->DetachTexCoordBuffer();
	}

	//Skin Weights Count
	pXMNode->Mesh->SkinWeightsCount = pXNode->Mesh->SkinWeights->size();
	list<XFbxSkinWeights*>::iterator it = pXNode->Mesh->SkinWeights->begin();
	pXMNode->Mesh->SkinWeightsCollection = static_cast<XFBX_SKIN_WEIGHTS*>(::LocalAlloc(0, sizeof(XFBX_SKIN_WEIGHTS) * pXMNode->ChildCount));
	int i = 0;
	while(it != pXNode->Mesh->SkinWeights->end())
	{
		XFBX_SKIN_WEIGHTS* pXMSkinWeights = (pXMNode->Mesh->SkinWeightsCollection + i);		
		//(skin weights).TransformNodeName...
		pXMSkinWeights->TransformNodeName = static_cast<char*>(::LocalAlloc(0, (*it)->TransformNodeName.length() + 1));
		::memcpy(pXMSkinWeights->TransformNodeName, (*it)->TransformNodeName.c_str(), (*it)->TransformNodeName.length() + 1);
		//(skin weights).WeightCount...
		pXMSkinWeights->WeightCount = (*it)->WeightCount;		
		if(!detachBuffers)
		{
			//(skin weights).Weights...
			pXMSkinWeights->Weights = static_cast<float*>(::LocalAlloc(0, sizeof(float) * pXMSkinWeights->WeightCount));
			::memcpy(pXMSkinWeights->Weights, (*it)->WeightBuffer, sizeof(float) * pXMSkinWeights->WeightCount);
			//(skin weights).VertexIndices
			pXMSkinWeights->VertexIndices = static_cast<int*>(::LocalAlloc(0, sizeof(int) * pXMSkinWeights->WeightCount));
			::memcpy(pXMSkinWeights->VertexIndices, (*it)->VertexIndices, sizeof(int) * pXMSkinWeights->WeightCount);
		}
		else 
		{
			//(skin weights).Weights...
			pXMSkinWeights->Weights = (*it)->WeightBuffer;
			//(skin weights).VertexIndices...
			pXMSkinWeights->VertexIndices = (*it)->VertexIndices;
			(*it)->DetachWeightBuffers();
		}
		i++;
	}
}

XFBX_SCENE* XFbxScene::ConvertToMarshableType(bool detachBuffers)
{
	XFBX_SCENE* pXMScene = static_cast<XFBX_SCENE*>(::LocalAlloc(0, sizeof(XFBX_SCENE)));
	pXMScene->NodeCount = this->Nodes->size();
	pXMScene->Nodes = static_cast<XFBX_NODE*>(::LocalAlloc(0, sizeof(XFBX_NODE) * pXMScene->NodeCount));
	list<XFbxNode*>::iterator iterator = this->Nodes->begin();
	int i = 0;
	while(iterator != this->Nodes->end())
	{
		::ConvertToMarshableType((*iterator), (pXMScene->Nodes + i), detachBuffers);
		i++;
	}
	return pXMScene;
}
#pragma endregion

////////////////////////
//OBSOLETE
//
//static void CollapseMeshData(XFbxMesh *pMesh, std::list<XMFLOAT3_> *pVertices, std::list<int> *pIndices)
//{
//	XMFLOAT3_* pVBTraverser = pMesh->VertexBuffer;
//	for( int i = 0; i < pMesh->VertexCount; i++)
//	{
//		//TODO: TRANSFORM vertices here.
//
//		pVertices->push_back(*pVBTraverser);
//		pVBTraverser++;		
//	}
//
//	int* pIBTraverser = pMesh->IndexBuffer;
//	for (int i = 0; i < pMesh->IndexCount; i++)
//	{
//		pIndices->push_back(*pIBTraverser);
//		pIBTraverser++;
//	} 
//
//	//TODO: (Possibly) collapse texture data here.
//
//	//TODO: Figure out how I would collapse animation and material data.
//	// -- I don't see how as they operate on individual nodes.
//}

////////////////////////////////
//OBSOLETE
//
//XFbxMesh* XFbxMesh::CollapseToSingleMesh()
//{
//	XFbxMesh *pResult = new XFbxMesh();
//	/*float adjustedTransform[16] = this->Transform;
//	std::list<XMFLOAT3_> vertices = std::list<XMFLOAT3_>();
//	std::list<int> indices = std::list<int>();
//	CollapseMeshData(this, &_vertices, &_indices);
//	for each( LOCALE_SSHORTESTDAYNAME7 *pMeshPart in this->Children)
//	{
//		
//	}*/
//	return pResult;
//}

