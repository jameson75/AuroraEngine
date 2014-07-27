using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
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

        public void Build(TrackSegmentBuilder[] segmentBuilders, TrackLayoutItem[] layoutItems, Path path, SurfaceEffect effect, int partitionSize)
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
                List<short> indices = segmentBuilders[i].GetIndices(excludeNearEdge);
                List<Vector2> textureCoords = segmentBuilders[i].GetTextureCoords(excludeNearEdge);                            
                for (int j = 0; j < layoutItems[j].InstanceCount; j++)
                {
                    List<Vector3> vertices = segmentBuilders[i].GenerateVertices(path, i * stepSize, stepSize, excludeNearEdge);               
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
                        Mesh mesh = ContentBuilder.BuildMesh<TrackVertex>(graphicsDevice,
                                                                          effect.SelectShaderByteCode(),
                                                                          meshTrackVertices.ToArray(),
                                                                          meshIndices.ToArray(),
                                                                          TrackVertex.InputElements,
                                                                          TrackVertex.Size,
                                                                          boundingBox);

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
        }       
    }

    public abstract class TrackSegmentBuilder
    {
       
    }

    public class TrackLayoutItem
    {        
        public int DescriptionIndex { get; set; }
        public int InstanceCount { get; set; }
    }

    public class Track
    {
        public Mesh Mesh { get; set; }
        public Effect Effect { get; set; }
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
