using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CipherPark.AngelJacket.Core.Effects;
using CipherPark.AngelJacket.Core.Kinetics;
using CipherPark.AngelJacket.Core.World.Renderers;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Content;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Geometry
{
    public class TrackBuilder
    {        
        private readonly Vector2 segmentFaceCount = new Vector2();

        public Track Build(SharpDX.Direct3D11.Device graphicsDevice, TrackSegmentBuilder[] segmentBuilders, TrackLayoutItem[] layoutItems, Path path, SurfaceEffect effect, int partitionSize)
        {
            int totalInstances = layoutItems.Sum( i => i.InstanceCount );
            float stepSize = path.Distance / totalInstances;  
            List<short> meshIndices = new List<short>();
            List<Vector2> meshTextureCoords = new List<Vector2>();
            List<Vector3> meshVertices = new List<Vector3>();
            List<Vector3> meshNormals = new List<Vector3>();
            short indexOffset = 0;
            int partitionCount = 0;
            List<Mesh> trackMeshes = new List<Mesh>();

            for (int i = 0; i < layoutItems.Length; i++)
            {
                bool excludeNearEdge = i > 0;
                List<short> indices = segmentBuilders[i].GetIndices();
                List<Vector2> textureCoords = segmentBuilders[i].GetTextureCoords();                            
                for (int j = 0; j < layoutItems[j].InstanceCount; j++)
                {
                    List<Vector3> vertices = segmentBuilders[i].GenerateVertices(path, i * stepSize, stepSize);               
                    List<Vector3> normals = ContentBuilder.GenerateNormals(vertices.ToArray(), indices.ToArray()).ToList();
                    List<short> adjustedIndices = indices.Select( e => (short)(e + indexOffset)).ToList();
                    meshIndices.AddRange(adjustedIndices);
                    meshTextureCoords.AddRange(meshTextureCoords);
                    meshVertices.AddRange(vertices);
                    meshNormals.AddRange(normals);
                    indexOffset += (short)vertices.Count;
                    partitionCount++;
                    if (partitionCount == partitionSize || i == layoutItems.Length - 1)
                    {
                        TrackVertex[] meshTrackVertices = meshVertices.Select((v,k) => new TrackVertex()
                        {
                            Position = new Vector4(v, 1.0f),
                            Normal = meshNormals[k],
                            TextureCoord = textureCoords[k]
                        }).ToArray();
                        BoundingBox box = BoundingBoxExtension.Empty;
                        Mesh mesh = ContentBuilder.BuildMesh<TrackVertex>(graphicsDevice,
                                                                          effect.SelectShaderByteCode(),
                                                                          meshTrackVertices.ToArray(),
                                                                          meshIndices.ToArray(),
                                                                          TrackVertex.InputElements,
                                                                          TrackVertex.ElementSize,
                                                                          box);

                        trackMeshes.Add(mesh);

                        meshIndices.Clear();
                        meshVertices.Clear();
                        meshNormals.Clear();
                        meshTextureCoords.Clear();
                        indexOffset = 0;
                        partitionCount = 0;
                    }
                }
            }

            return new Track()
            {
                Effect = effect,
                Meshes = trackMeshes
            };
        }      
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct TrackVertex
    {
        public Vector4 Position;
        public Vector3 Normal;
        public Vector2 TextureCoord;

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }

        static TrackVertex()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("NORMAL", 0, Format.R32G32B32_Float, 16, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32_Float, 28, 0)
             };
            _elementSize = 36;
        }

        public TrackVertex(Vector3 position, Vector3 normal, Vector2 textureCoord)
        {
            Position = new Vector4(position, 1.0f);
            Normal = normal;
            TextureCoord = textureCoord;
        }
    }

    public abstract class TrackSegmentBuilder
    {
        public abstract List<short> GetIndices();
        public abstract List<Vector2> GetTextureCoords();
        public abstract List<Vector3> GenerateVertices(Path path, float subPathStart, float subPathStart);

    }

    public class TrackLayoutItem
    {        
        public int DescriptionIndex { get; set; }
        public int InstanceCount { get; set; }
    }

    public class Track
    {           
        public SurfaceEffect Effect { get; set; }
        public List<Mesh> Meshes { get; set; }
        public void Draw(GameTime gameTime)
        {            
            if (Effect != null)
            {
                Effect.World = Matrix.Identity;               
                Effect.Apply();
                foreach (Mesh mesh in Meshes)
                    mesh.Draw(gameTime);
                Effect.Restore();
            }  
        }
    }

    public static class EnumerableExtensions
    {
        public static void RemoveAll<T>(this IList<T> data, System.Func<T, bool> selector)
        {
            var itemsToDelete = data.Where(selector);
            foreach (var d in data)
                data.Remove(d);
        }
    }
}
