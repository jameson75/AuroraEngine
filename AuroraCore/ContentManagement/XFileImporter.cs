using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SharpDX;
using SharpDX.Direct3D11;
using CoreTransform = CipherPark.Aurora.Core.Animation.Transform;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Animation.Controllers;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.Effects;

namespace CipherPark.Aurora.Core.Content
{
    public class XFileImporter
    {
        public static Model ImportX(IGameApp game, string fileName, SurfaceEffect effect, XFileImportOptions options)
        {
            XFileTextDocument doc = new XFileTextDocument();          

            //Read X-File Data/DOM
            //--------------------
            doc.Load(System.IO.File.Open(fileName, FileMode.Open, FileAccess.ReadWrite));

            var targetModelType = DetermineTargetModelType(doc);

            switch (targetModelType)
            {
                case ModelType.SkinnedMesh:
                    return BuildSkinnedMeshModel(game, doc, effect, options);
                case ModelType.MultiMesh:
                    return BuildMultiMeshModel(game, doc, effect, options);
                case ModelType.StaticMesh:
                    return BuildStaticMeshModel(game, doc, effect, options);
                default:
                    throw new InvalidOperationException("Could not determine target model type from file");
            }
        }

        private static SkinnedMeshModel BuildSkinnedMeshModel(IGameApp game, XFileTextDocument doc, SurfaceEffect effect, XFileImportOptions options)
        {            
            Mesh mesh = null;
            Frame rootBoneFrame = null;
            List<KeyframeAnimationController> animationControllers = null;
            List<SkinOffset> skinOffsets = null;
            var shaderByteCode = effect.GetVertexShaderByteCode();
            var effectDataChannels = effect.GetDataChannels();

            //Access X-File's only mesh
            //-------------------------           
            XFileMeshObject xMesh = GetSingleMesh(doc);

            //Calculate mesh bounding box from mesh Data
            //------------------------------------------
            BoundingBox boundingBox = BoundingBox.FromPoints(xMesh.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray());

            //Construct model vertex indices from mesh data
            //----------------------------------------------            
            short[] _indices = xMesh.Faces.SelectMany(e => e.FaceVertexIndices.Select(x => (short)x)).ToArray();

            //Extract texture coords from mesh data
            //-------------------------------------          
            Vector2[] texCoords = null;
            if ((effectDataChannels & EffectDataChannels.Texture) != 0)
            {
                texCoords = GetMeshTextureCoords(xMesh);
            }

            //Extract normals.
            //----------------
            Vector3[] normals = null;
            if ((effectDataChannels & EffectDataChannels.Normal) != 0)
            {
                normals = GetMeshNormals(xMesh, options.HasFlag(XFileImportOptions.GenerateMissingNormals));
            }

            //Extract Color from default material
            //-----------------------------------
            Color[] colors = null;
            if ((effectDataChannels & EffectDataChannels.Color) != 0)
            {
                colors = GetMeshVertexColors(xMesh, doc, options.HasFlag(XFileImportOptions.IgnoreMissingColors));
            }

            if ((effectDataChannels & EffectDataChannels.Skinning) != 0)
            {
                if (xMesh.SkinWeightsCollection.Count == 0)
                    throw new InvalidOperationException("Expected skin data was not present.");

                //Construct skinned model vertices from mesh data
                //-----------------------------------------------                                       

                VertexPositionNormalTextureSkin[] _vertices = xMesh.Vertices.Select((v, i) => new VertexPositionNormalTextureSkin()
                {
                    Position = new Vector4(v.X, v.Y, v.Z, 1.0f),
                    Normal = normals[i],
                    TextureCoord = texCoords[i]
                }).ToArray();

                //Convert skinning information in .x file to skinning information required by a skinning shader.
                //(A skinning shader associates each vertex with 4 bone indices and 4 bone weights - we extract this 
                //information from SkinWeights data object).
                //-------------------------------------------------------------------------------------------------

                List<float>[] weights = new List<float>[_vertices.Length];
                List<int>[] boneIndices = new List<int>[_vertices.Length];
                List<string>[] boneNames = new List<string>[_vertices.Length];
                skinOffsets = new List<SkinOffset>();

                for (int i = 0; i < xMesh.SkinWeightsCollection.Count; i++)
                {
                    XFileSkinWeightsObject xSkinWeights = xMesh.SkinWeightsCollection[i];
                    CoreTransform skinOffsetTransform = new CoreTransform(new Matrix(xSkinWeights.MatrixOffset.m));
                    skinOffsets.Add(new SkinOffset() { Name = xSkinWeights.TransformNodeName, Transform = skinOffsetTransform });
                    for (int j = 0; j < xSkinWeights.NWeights; j++)
                    {
                        int k = xSkinWeights.VertexIndices[j];

                        if (boneIndices[k] == null)
                            boneIndices[k] = new List<int>();
                        boneIndices[k].Add(i);

                        if (weights[k] == null)
                            weights[k] = new List<float>();
                        weights[k].Add(xSkinWeights.Weights[j]);
                    }
                }
                for (int i = 0; i < _vertices.Length; i++)
                {
                    float[] weightValues = new float[4] { 0, 0, 0, 0 };
                    int[] boneIndicesValues = new int[4] { 0, 0, 0, 0 };
                    if (weights[i] != null)
                    {
                        float[] _weights = weights[i].ToArray();
                        //TODO: assert that _weights.Length <= weightValues.Length (we assume the .x file will have no more than 4 per vertex).
                        Array.Copy(_weights, weightValues, _weights.Length);
                    }
                    if (boneIndices[i] != null)
                    {
                        int[] _boneIndices = boneIndices[i].ToArray();
                        //TODO: assert that _bondIndices.Length <= boneIndicesValues.Length (we assume the .x file will have no more than 4 per vertex).
                        Array.Copy(_boneIndices, boneIndicesValues, _boneIndices.Length);
                    }
                    _vertices[i].Weights = new Vector4(weightValues);
                    _vertices[i].BoneIndices = new Int4(boneIndicesValues);
                }

                //Construct bone hierarchy (skeleton) from frame data
                //----------------------------------------------------              
                //****************************************************
                //NOTE: It is assumed that
                //there is only one bone at the root and, therefore,
                //one bone-frame at the root level of the X-File
                //****************************************************                  
                XFileFrameObject xRootBoneFrame = GetBoneRootFrame(doc);
                rootBoneFrame = new Frame() { Name = xRootBoneFrame.Name, Transform = new CoreTransform(new Matrix(xRootBoneFrame.FrameTransformMatrix.m)) };
                BuildBoneTree(xRootBoneFrame, rootBoneFrame, skinOffsets);

                //Construct animation data from frame data
                //----------------------------------------            
                XFileAnimationSetObject xAnimationSet = doc.DataObjects.GetDataObject<XFileAnimationSetObject>(1);
                if (xAnimationSet == null)
                    throw new InvalidOperationException("Execpted animation data was not present.");
                List<Frame> targetFrames = rootBoneFrame.FlattenToList();
                animationControllers = BuildAnimationRig(xAnimationSet, targetFrames);

                mesh = ContentBuilder.BuildMesh<VertexPositionNormalTextureSkin>(game.GraphicsDevice, shaderByteCode, _vertices, _indices, VertexPositionNormalTextureSkin.InputElements, VertexPositionNormalTextureSkin.ElementSize, boundingBox);
                mesh.Name = xMesh.Name;
                SkinnedMeshModel riggedModel = new SkinnedMeshModel(game);
                riggedModel.Mesh = mesh;
                riggedModel.SkinOffsets.AddRange(skinOffsets);
                riggedModel.FrameTree = rootBoneFrame;
                riggedModel.AnimationRig.AddRange(animationControllers);
                riggedModel.Effect = effect;
                return riggedModel;
            }
            else
            {
                throw new ArgumentException("The target model type of the file is a skinned model. However, the effect does not support a skin data channel.", nameof(effect));
            }
        }

        private static StaticMeshModel BuildStaticMeshModel(IGameApp game, XFileTextDocument doc, SurfaceEffect effect, XFileImportOptions options)
        {                  
            var xMesh = GetSingleMesh(doc);
            var mesh = BuildStandardMesh(game, xMesh, doc, effect, options);
            StaticMeshModel basicModel = new StaticMeshModel(game);
            basicModel.Mesh = mesh;
            basicModel.Effect = effect;
            return basicModel;
        }

        private static MultiMeshModel BuildMultiMeshModel(IGameApp game, XFileTextDocument doc, SurfaceEffect effect, XFileImportOptions options)
        {
            var meshes = new List<Mesh>();
            var shaderByteCode = effect.GetVertexShaderByteCode();
            var effectDataChannels = effect.GetDataChannels();
            Frame frameTree = null;
            List<KeyframeAnimationController> animationControllers = null;

            bool domHasFrames = doc.DataObjects.GetDataObject<XFileFrameObject>(1) != null;
            if (!domHasFrames)
            {
                meshes.AddRange(doc.DataObjects.Where(x => x is XFileMeshObject)
                                               .Select(x => BuildStandardMesh(game, (XFileMeshObject)x, doc, effect, options)));                               
            }
            else
            {
                var hierarchyRootFrame = GetMeshFrame(doc);
                var hierarchyMeshes = new List<XFileMeshObject>();
                GetMeshesFromHierarchy(hierarchyRootFrame, hierarchyMeshes);
                meshes.AddRange(hierarchyMeshes.Select(x => BuildStandardMesh(game, (XFileMeshObject)x, doc, effect, options)));
                frameTree = new Frame() { Name = hierarchyRootFrame.Name, Transform = new CoreTransform(new Matrix(hierarchyRootFrame.FrameTransformMatrix.m)) };
                BuildMeshFrameTree(hierarchyRootFrame, frameTree);

                //Construct animation data from frame data
                //----------------------------------------            
                XFileAnimationSetObject xAnimationSet = doc.DataObjects.GetDataObject<XFileAnimationSetObject>(1);
                if (xAnimationSet != null)
                {
                    animationControllers = BuildAnimationRig(xAnimationSet, frameTree.FlattenToList());
                }
            }           

            MultiMeshModel multiMeshModel = new MultiMeshModel(game);
            multiMeshModel.Effect = effect;
            multiMeshModel.Meshes.AddRange(meshes);
            multiMeshModel.FrameTree = frameTree;
            if (animationControllers != null)
            {
                multiMeshModel.AnimationRig.AddRange(animationControllers);
            }
            return multiMeshModel;
        }
        
        private static void GetMeshesFromHierarchy(XFileFrameObject frame, IList<XFileMeshObject> meshes)
        {
            if (frame.Meshes != null)
            {
                foreach(var mesh in frame.Meshes)
                {
                    meshes.Add(mesh);
                }
            }

            if (frame.ChildFrames != null)
            {
                foreach (var childFrame in frame.ChildFrames)
                {
                    GetMeshesFromHierarchy(childFrame, meshes);   
                }                        
            }
        }

        private static Mesh BuildStandardMesh(IGameApp game, XFileMeshObject xMesh, XFileTextDocument doc, SurfaceEffect effect, XFileImportOptions options)
        {
            Mesh mesh = null;
            var shaderByteCode = effect.GetVertexShaderByteCode();
            var effectDataChannels = effect.GetDataChannels();

            //Calculate mesh bounding box from mesh Data
            //------------------------------------------
            BoundingBox boundingBox = BoundingBox.FromPoints(xMesh.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray());

            //Construct model vertex indices from mesh data
            //----------------------------------------------            
            short[] _indices = xMesh.Faces.SelectMany(e => e.FaceVertexIndices.Select(x => (short)x)).ToArray();

            //Extract texture coords from mesh data
            //-------------------------------------          
            Vector2[] texCoords = null;
            if ((effectDataChannels & EffectDataChannels.Texture) != 0)
            {
                texCoords = GetMeshTextureCoords(xMesh);
            }

            //Extract normals.
            //----------------
            Vector3[] normals = null;
            if ((effectDataChannels & EffectDataChannels.Normal) != 0)
            {
                normals = GetMeshNormals(xMesh, options.HasFlag(XFileImportOptions.GenerateMissingNormals));
            }

            //Extract Color from default material
            //-----------------------------------
            Color[] colors = null;
            if ((effectDataChannels & EffectDataChannels.Color) != 0)
            {
                colors = GetMeshVertexColors(xMesh, doc, options.HasFlag(XFileImportOptions.IgnoreMissingColors));
            }

            mesh = BuildMeshForChannels(effectDataChannels, game.GraphicsDevice, shaderByteCode, xMesh.Vertices, _indices, texCoords, normals, colors, boundingBox);
            
            mesh.Name = xMesh.Name;

            return mesh;
        }

        private static Vector2[] GetMeshTextureCoords(XFileMeshObject xMesh)
        {
            var texCoords = xMesh.DeclData?.GetTextureCoords() ??
                            xMesh.MeshTextureCoords?.TextureCoords.ToArray();

            if (texCoords == null)
            {
                throw new InvalidOperationException("Expected texture coordinate data was not present.");
            }
            
            return texCoords;
        }

        private static Vector3[] GetMeshNormals(XFileMeshObject xMesh, bool generateMissingNormals)
        {
            var normals = xMesh.DeclData?.GetNormals() ??
                          xMesh.MeshNormals?.Normals.ToArray();

            if (normals == null && !generateMissingNormals)
            {
                throw new InvalidOperationException("Expected normal data was not present.");
            }
            else
            {
                normals = GenerateNormals(xMesh);
            }

            return normals;
        }

        private static Color[] GetMeshVertexColors(XFileMeshObject xMesh, XFileTextDocument doc, bool ignoreMissingColors)
        {            
            Color[] colors = null;
            if (xMesh.MeshMaterialList != null)
            {
                XFileMaterialObject defaultMaterial = null;

                if (xMesh.MeshMaterialList.MaterialRefs != null)
                    defaultMaterial = doc.DataObjects.GetDataObjects<XFileMaterialObject>().First(m => m.Name == xMesh.MeshMaterialList.MaterialRefs[0]);
                else if (xMesh.MeshMaterialList.Materials != null)
                    defaultMaterial = xMesh.MeshMaterialList.Materials[0];
                else
                    throw new InvalidOperationException("Could not find default material");                

                colors = xMesh.Vertices.Select(v => new Color(defaultMaterial.FaceColor.Red,
                                                              defaultMaterial.FaceColor.Green,
                                                              defaultMaterial.FaceColor.Blue,
                                                              1.0f))
                                       .ToArray();
            }
            else if(xMesh.MeshVertexColors != null)
            {
                var indexMap = xMesh.MeshVertexColors.VertexColors.ToDictionary(x => x.Index, x => x.ColorRGBA);
                colors = xMesh.Vertices.Select((v, i) => new Color(
                    indexMap[i].Red,
                    indexMap[i].Green,
                    indexMap[i].Blue,
                    indexMap[i].Alpha)).ToArray();
            }
            if (colors == null)
            {
                if (!ignoreMissingColors)
                    throw new InvalidOperationException("Expected color data was not present.");
                else
                    colors = xMesh.Vertices.Select(v => Color.Gray).ToArray();
            }
            return colors;
        }

        private static ModelType DetermineTargetModelType(XFileTextDocument doc)
        {
            //If the x-file has multiple meshes at the root level then we target the multi-mesh model.                     
            if (doc.DataObjects.GetDataObject<XFileMeshObject>(2) != null)
                return ModelType.MultiMesh;

            var xMeshFrame = GetMeshFrame(doc);
            if( xMeshFrame != null)
            {
                var xMeshes = new List<XFileMeshObject>();
                GetMeshesFromHierarchy(xMeshFrame, xMeshes);
                if (xMeshes.Count > 1)
                {
                    return ModelType.MultiMesh;
                }
            }

            //Otherwise, we assume a single mesh to exists.
            var xMesh = GetSingleMesh(doc);

            //at this points, if we do not find a mesh at neither the root level nor inside the root frame, 
            //we conclude we're unable to determine the target model type.
            if (xMesh == null)
            {
                return ModelType.Unknown;
            }

            //determine if the single mesh is skinned/rigged.
            if( xMesh.SkinWeightsCollection != null && xMesh.SkinWeightsCollection.Any())
            {
                return ModelType.SkinnedMesh;
            }
            else
            {
                return ModelType.StaticMesh;
            }
        }

        private static XFileMeshObject GetSingleMesh(XFileTextDocument doc)
        {
            //Get the first mesh we encounter either at the root level or inside the root frame.
            return doc.DataObjects.GetDataObject<XFileMeshObject>(1) ??
                   doc.DataObjects.GetDataObject<XFileFrameObject>(1).Meshes?.FirstOrDefault();
        }

        private static XFileFrameObject GetBoneRootFrame(XFileTextDocument doc)
        {
            return doc.DataObjects.GetDataObject<XFileFrameObject>(2);
        }

        private static XFileFrameObject GetMeshFrame(XFileTextDocument doc)
        {
            return doc.DataObjects.GetDataObject<XFileFrameObject>(1);
        }

        private static Vector3[] GenerateNormals(XFileMeshObject xMesh)
        {
            return Enumerable.Repeat(Vector3.Up, xMesh.Vertices.Length).ToArray();
        }

        private static List<KeyframeAnimationController> BuildAnimationRig(XFileAnimationSetObject xAnimationSet, List<Frame> targetFrames)
        {
            List<KeyframeAnimationController> animationControllers = new List<KeyframeAnimationController>();

            for (int i = 0; i < xAnimationSet.Animations.Count; i++)
            {
                XFileAnimationObject xAnimation = xAnimationSet.Animations[i];
                Frame animationTarget = targetFrames.Find(f => f.Name == xAnimation.FrameRef);
                if (animationTarget != null)
                {
                    Dictionary<long, Matrix> matrixTransformKeys = new Dictionary<long, Matrix>();
                    for (int j = 0; j < xAnimation.Keys.Count; j++)
                    {
                        XFileAnimationKeyObject xAnimationKey = xAnimation.Keys[j];
                        switch (xAnimationKey.KeyType)
                        {
                            case KeyType.Matrix:
                                for (int k = 0; k < xAnimationKey.NKeys; k++)
                                {
                                    matrixTransformKeys.Add(xAnimationKey.TimedFloatKeys[k].Time,
                                                            new Matrix(xAnimationKey.TimedFloatKeys[k].Values));
                                }
                                break;
                        }
                    }
                    TransformAnimation modelAnimation = new TransformAnimation();
                    foreach (long time in matrixTransformKeys.Keys)
                        modelAnimation.SetKeyFrame(new AnimationKeyFrame((ulong)time, new CoreTransform(matrixTransformKeys[time])));
                    animationControllers.Add(new KeyframeAnimationController(modelAnimation, animationTarget));
                }
            }

            return animationControllers;
        }

        private static Mesh BuildMeshForChannels(EffectDataChannels channels, Device device, byte[] shaderByteCode, XFileVector[] vertices, short[] indices, Vector2[] texCoords, Vector3[] normals, Color[] colors, BoundingBox boundingBox)
        {
            if ((channels & EffectDataChannels.Color) != 0)
            {
                if ((channels & EffectDataChannels.Normal) != 0)
                {
                    //VERTEXPOSITIONNORMALCOLOR
                    //-------------------------
                    VertexPositionNormalColor[] _vertices = vertices.Select((v, i) => new VertexPositionNormalColor()
                    {
                        Position = new Vector4(v.X, v.Y, v.Z, 1.0f),
                        Normal = normals[i],
                        Color = colors[i].ToVector4()
                    }).ToArray();
                    return ContentBuilder.BuildMesh<VertexPositionNormalColor>(device, shaderByteCode, _vertices, indices, VertexPositionNormalColor.InputElements, VertexPositionNormalColor.ElementSize, boundingBox);

                }
                else
                {
                    //VERTEXPOSITIONCOLOR
                    //-------------------
                    VertexPositionColor[] _vertices = vertices.Select((v, i) => new VertexPositionColor()
                    {
                        Position = new Vector4(v.X, v.Y, v.Z, 1.0f),
                        Color = colors[i].ToVector4()
                    }).ToArray();
                    return ContentBuilder.BuildMesh<VertexPositionColor>(device, shaderByteCode, _vertices, indices, VertexPositionColor.InputElements, VertexPositionColor.ElementSize, boundingBox);

                }
            }
            else if ((channels & EffectDataChannels.Texture) != 0)
            {
                if ((channels & EffectDataChannels.Normal) != 0)
                {
                    //VERTEXPOSITIONNORMALTEXTURE
                    //---------------------------
                    VertexPositionNormalTexture[] _vertices = vertices.Select((v, i) => new VertexPositionNormalTexture()
                    {
                        Position = new Vector4(v.X, v.Y, v.Z, 1.0f),
                        Normal = normals[i],
                        TextureCoord = texCoords[i]
                    }).ToArray();
                    return ContentBuilder.BuildMesh<VertexPositionNormalTexture>(device, shaderByteCode, _vertices, indices, VertexPositionNormalTexture.InputElements, VertexPositionNormalTexture.ElementSize, boundingBox);

                }
                else
                {
                    //VERTEXPOSITIONTEXTURE
                    //---------------------
                    VertexPositionTexture[] _vertices = vertices.Select((v, i) => new VertexPositionTexture()
                    {
                        Position = new Vector4(v.X, v.Y, v.Z, 1.0f),
                        TextureCoord = texCoords[i]
                    }).ToArray();
                    return ContentBuilder.BuildMesh<VertexPositionTexture>(device, shaderByteCode, _vertices, indices, VertexPositionTexture.InputElements, VertexPositionTexture.ElementSize, boundingBox);
                }
            }
            else
                //UNSUPPORTED-VERTEX-FORMAT
                //-------------------------
                throw new InvalidOperationException("Could not determine appropiate vertex format from effect channels.");
        }

        private static void BuildBoneTree(XFileFrameObject xRootBone, Frame rootBone, List<SkinOffset> skinOffsets)
        {
            foreach (XFileFrameObject xChildBone in xRootBone.ChildFrames)
            {                
                Frame childBone = new Frame() { Name = xChildBone.Name, Transform = new CoreTransform(new Matrix(xChildBone.FrameTransformMatrix.m)) };                
                SkinOffset skinOffset = skinOffsets.Find(b => b.Name == xChildBone.Name);
                if (skinOffset != null)
                    skinOffset.BoneReference = childBone;         
                rootBone.Children.Add(childBone);
                BuildBoneTree(xChildBone, childBone, skinOffsets);
            }
        }

        private static void BuildMeshFrameTree(XFileFrameObject xRootFrame, Frame rootFrame)
        {
            foreach (XFileFrameObject xChildFrame in xRootFrame.ChildFrames)
            {
                Frame childFrame = new Frame() { Name = xChildFrame.Name, Transform = new CoreTransform(new Matrix(xChildFrame.FrameTransformMatrix.m)) };               
                if (xChildFrame.Meshes?.Any() == true)
                {
                    childFrame.MeshNames = xChildFrame.Meshes.Select(x => x.Name).ToArray();
                }
                rootFrame.Children.Add(childFrame);
                BuildMeshFrameTree(xChildFrame, childFrame);
            }
        }
    }
}
