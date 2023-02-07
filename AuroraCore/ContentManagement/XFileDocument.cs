using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Content
{
    public abstract class XFileDocument
    {
        private DataObjectCollection dataObjects = new DataObjectCollection();

        public XFileHeader Header { get; protected set; }

        public DataObjectCollection DataObjects { get { return dataObjects; } }
    }

    public class XFileTextDocument : XFileDocument
    {
        public void Load(Stream dataStream)
        {
            StreamReader reader = new StreamReader(dataStream);
            Header = ReadHeader(reader);
            while (!reader.EndOfStream)
            {
                XFileDataObject nextDataObject = null;
                ParseNextDataObject(reader, out nextDataObject);
                if (nextDataObject != null)
                    DataObjects.Add(nextDataObject);
            }
        }        

        private void ParseNextDataObject(TextReader reader, out XFileDataObject dataObject)
        {
            dataObject = null;
            XFileObjectBlock ob = ReadNextDataBlock(reader);
            if (ob != null)
            {
                StringReader dataReader = new StringReader(ob.Data);
                switch (ob.Type)
                {
                    case XFileMaterialObject.TemplateName:
                        dataObject = ParseMaterialObject(dataReader, ob.Name);
                        break;
                    case XFileFrameObject.TemplateName:
                        dataObject = ParseFrameObject(dataReader, ob.Name);
                        break;
                    case XFileMeshObject.TemplateName:
                        dataObject = ParseMeshObject(dataReader, ob.Name);
                        break;
                    case XFileAnimationTicksPerSecondObject.TemplateName:
                        dataObject = ParseAnimationTicksPerSecond(dataReader, ob.Name);
                        break;
                    case XFileAnimationSetObject.TemplateName:
                        dataObject = ParseAnimationSetObject(dataReader, ob.Name);
                        break;
                }
            }
        }

        public XFileAnimationTicksPerSecondObject ParseAnimationTicksPerSecond(TextReader reader, string name)
        {
            XFileAnimationTicksPerSecondObject result = new XFileAnimationTicksPerSecondObject();
            result.TicksPerSecond = ReadNextInt(reader).Value;
            return result;
        }

        public XFileFrameObject ParseFrameObject(TextReader reader, string name)
        {
            XFileFrameObject frame = new XFileFrameObject();
            frame.Name = name;

            XFileObjectBlock ob = ReadNextDataBlock(reader);
            while (ob != null)
            {
                StringReader dataReader = new StringReader(ob.Data);
                XFileDataObject childObject = null;
                switch (ob.Type)
                {
                    case "FrameTransformMatrix":
                        frame.FrameTransformMatrix = ParseFrameTransformMatrix(dataReader);
                        break;
                    case XFileFrameObject.TemplateName:
                        childObject = ParseFrameObject(dataReader, ob.Name);
                        frame.ChildFrames.Add((XFileFrameObject)childObject);
                        break;
                    case XFileMeshObject.TemplateName:
                        childObject = ParseMeshObject(dataReader, ob.Name);
                        frame.Meshes.Add((XFileMeshObject)childObject);
                        break;
                }
                ob = ReadNextDataBlock(reader);
            }
            return frame;
        }

        private XFileDataObject ParseAnimationSetObject(TextReader reader, string name)
        {
            XFileAnimationSetObject animationSet = new XFileAnimationSetObject();
            animationSet.Name = name;

            XFileObjectBlock ob = ReadNextDataBlock(reader);
            while (ob != null)
            {
                XFileAnimationObject childObject = null;
                StringReader dataReader = new StringReader(ob.Data);
                switch (ob.Type)
                {
                    case XFileAnimationObject.TemplateName:
                        childObject = ParseAnimationObject(dataReader, ob.Name);
                        animationSet.Animations.Add(childObject);
                        break;
                }
                ob = ReadNextDataBlock(reader);
            }

            return animationSet;
        }

        private XFileAnimationObject ParseAnimationObject(TextReader reader, string name)
        {
            XFileAnimationObject animation = new XFileAnimationObject();
            animation.Name = name;
            animation.FrameRef = ReadNextReference(reader);
            XFileObjectBlock ob = ReadNextDataBlock(reader);

            while (ob != null)
            {
                XFileDataObject childObject = null;
                StringReader dataReader = new StringReader(ob.Data);
                switch (ob.Type)
                {
                    case XFileAnimationKeyObject.TemplateName:
                        childObject = ParseAnimationKeyObject(dataReader, ob.Name);
                        animation.Keys.Add((XFileAnimationKeyObject)childObject);
                        break;
                }
                ob = ReadNextDataBlock(reader);
            }
            return animation;
        }

        private XFileAnimationKeyObject ParseAnimationKeyObject(TextReader reader, string name)
        {
            XFileAnimationKeyObject animationKey = new XFileAnimationKeyObject();
            animationKey.Name = name;
            animationKey.KeyType = (KeyType)Math.Min(ReadNextInt(reader).Value, 3);
            animationKey.NKeys = ReadNextInt(reader).Value;
            const int tfkOffset = 2;
            animationKey.TimedFloatKeys = new XFileTimedFloatKey[animationKey.NKeys];
            int nAnimationKeyComponents = 0;
            switch (animationKey.KeyType)
            {
                case KeyType.Matrix:
                    nAnimationKeyComponents = 18;
                    break;
                case KeyType.Position:
                    nAnimationKeyComponents = 5;
                    break;
                case KeyType.Scale:
                    nAnimationKeyComponents = 5;
                    break;
                case KeyType.Rotation:
                    nAnimationKeyComponents = 6;
                    break;
            }
            for (int i = 0; i < animationKey.NKeys; i++)
            {
                int j = i * nAnimationKeyComponents + tfkOffset;
                XFileTimedFloatKey timedFloatKey = new XFileTimedFloatKey();
                timedFloatKey.Time = ReadNextInt(reader).Value;
                timedFloatKey.NValues = ReadNextInt(reader).Value;
                timedFloatKey.Values = new float[timedFloatKey.NValues];
                const int valuesOffset = 2;
                for (int k = 0; k < timedFloatKey.NValues; k++)
                {
                    int m = j + k + valuesOffset;
                    timedFloatKey.Values[k] = ReadNextFloat(reader).Value;
                }
                animationKey.TimedFloatKeys[i] = timedFloatKey;
            }
            return animationKey;
        }

        private XFileMeshObject ParseMeshObject(TextReader reader, string name)
        {           
            //************************************************
            //NOTE: We always assume faces are triangular    *
            //************************************************     
            
            XFileMeshObject mesh = new XFileMeshObject();
            mesh.Name = name;

            mesh.NVertices = ReadNextInt(reader).Value;            
            mesh.Vertices = new XFileVector[mesh.NVertices];
            for (int i = 0; i < mesh.NVertices; i++)
            {                
                mesh.Vertices[i] = new XFileVector()
                {
                    X = ReadNextFloat(reader).Value,
                    Y = ReadNextFloat(reader).Value,
                    Z = ReadNextFloat(reader).Value
                };
            }
            mesh.NFaces = ReadNextInt(reader).Value;            
            mesh.Faces = new XFileMeshFaceObject[mesh.NFaces];

            for (int i = 0; i < mesh.NFaces; i++)
            {
                mesh.Faces[i] = ParseMeshFace(reader);
            }

            XFileObjectBlock ob = ReadNextDataBlock(reader);
            while (ob != null)
            {
                XFileDataObject childObject;
                StringReader dataReader = new StringReader(ob.Data);
                switch (ob.Type)
                {
                    case XFileMeshMaterialListObject.TemplateName:
                        childObject = ParseMeshMaterialListObject(dataReader, ob.Name);
                        mesh.MeshMaterialList = (XFileMeshMaterialListObject)childObject;
                        break;
                    case XFileDeclDataObject.TemplateName:
                        childObject = ParseDeclDataObject(dataReader, ob.Name);
                        mesh.DeclData = (XFileDeclDataObject)childObject;
                        break;
                    case XFileSkinWeightsObject.TemplateName:
                        childObject = ParseSkinWeightsObject(dataReader, ob.Name);
                        mesh.SkinWeightsCollection.Add((XFileSkinWeightsObject)childObject);
                        break;
                    case XFileTextureCoords.TemplateName:
                        childObject = ParseMeshTextureCoords(dataReader, ob.Name);
                        mesh.MeshTextureCoords = (XFileTextureCoords)childObject;
                        break;
                    case XFileMeshVertexColors.TemplateName:
                        childObject = ParseMeshVertexColors(dataReader, ob.Name);
                        mesh.MeshVertexColors = (XFileMeshVertexColors)childObject;
                        break;
                    case XFileMeshNormals.TemplateName:
                        childObject = ParseMeshNormals(dataReader, ob.Name);
                        mesh.MeshNormals = (XFileMeshNormals)childObject;
                        break;
                }
                ob = ReadNextDataBlock(reader);
            }

            return mesh;
        }

        private XFileMeshFaceObject ParseMeshFace(TextReader reader)
        {
            return new XFileMeshFaceObject()
            {
                NFaceVertexIndices = ReadNextInt(reader).Value,
                //NOTE: Assuming triangular faces, we expect this value to always be '3'.
                FaceVertexIndices = new int[] {
                        ReadNextInt(reader).Value,
                        ReadNextInt(reader).Value,
                        ReadNextInt(reader).Value }
            };
        }

        private XFileSkinWeightsObject ParseSkinWeightsObject(TextReader reader, string name)
        {
            XFileSkinWeightsObject skinWeights = new XFileSkinWeightsObject();
            skinWeights.Name = name;
            skinWeights.TransformNodeName = ReadNextString(reader);
            skinWeights.NWeights = ReadNextInt(reader).Value;
            skinWeights.VertexIndices = new int[skinWeights.NWeights];
            int vertexIndicesOffset = 1;
            for (int i = 0; i < skinWeights.NWeights; i++)
                skinWeights.VertexIndices[i] = ReadNextInt(reader).Value;
            int weightsOffset = vertexIndicesOffset + skinWeights.NWeights;
            skinWeights.Weights = new float[skinWeights.NWeights];
            for (int i = 0; i < skinWeights.NWeights; i++)
                skinWeights.Weights[i] = ReadNextFloat(reader).Value;
            int matrixOffset = weightsOffset + skinWeights.NWeights;
            XFileMatrix matrix = new XFileMatrix();
            matrix.m = new float[16];
            for (int i = 0; i < 16; i++)
                matrix.m[i] = ReadNextFloat(reader).Value;
            skinWeights.MatrixOffset = matrix;
            return skinWeights;
        }

        private XFileTextureCoords ParseMeshTextureCoords(TextReader reader, string name)
        {
            XFileTextureCoords meshTextureCoords = new XFileTextureCoords();
            meshTextureCoords.Name = name;
            meshTextureCoords.NTextureCoords = ReadNextInt(reader).Value;
            meshTextureCoords.TextureCoords = new Vector2[meshTextureCoords.NTextureCoords];
            for (int i = 0; i < meshTextureCoords.NTextureCoords; i++)
            {
                meshTextureCoords.TextureCoords[i] = new Vector2
                {
                    X = ReadNextFloat(reader).Value,
                    Y = ReadNextFloat(reader).Value,
                };
            }
            return meshTextureCoords;
        }

        private XFileMeshVertexColors ParseMeshVertexColors(TextReader reader, string name)
        {
            XFileMeshVertexColors meshVertexColors = new XFileMeshVertexColors();
            meshVertexColors.Name = name;
            meshVertexColors.NVertexColors = ReadNextInt(reader).Value;
            meshVertexColors.VertexColors = new XFileIndexedColor[meshVertexColors.NVertexColors];
            for (int i = 0; i < meshVertexColors.NVertexColors; i++)
            {
                meshVertexColors.VertexColors[i] = new XFileIndexedColor
                {
                    Index = ReadNextInt(reader).Value,
                    ColorRGBA = ParseColorRGBA(reader),
                };
            }
            return meshVertexColors;
        }

        public XFileMeshNormals ParseMeshNormals(TextReader reader, string name)
        {
            XFileMeshNormals meshNormals = new XFileMeshNormals();
            meshNormals.Name = name;
            meshNormals.NNormals = ReadNextInt(reader).Value;
            meshNormals.Normals = new Vector3[meshNormals.NNormals];
            for (int i = 0; i < meshNormals.NNormals; i++)
            {
                meshNormals.Normals[i] = new Vector3
                {
                    X = ReadNextFloat(reader).Value,
                    Y = ReadNextFloat(reader).Value,
                    Z = ReadNextFloat(reader).Value,
                };
            }
            meshNormals.NFaceNormals = ReadNextInt(reader).Value;
            meshNormals.FaceNormals = new XFileMeshFaceObject[meshNormals.NFaceNormals];
            for (int i = 0; i < meshNormals.NFaceNormals; i++)
            {
                meshNormals.FaceNormals[i] = ParseMeshFace(reader);
            }
            return meshNormals;
        }

        private XFileDeclDataObject ParseDeclDataObject(TextReader reader, string name)
        {
            const int nComponentsPerVertexElement = 4;
            XFileDeclDataObject declData = new XFileDeclDataObject();
            declData.Name = name;
            declData.NVertexElements = ReadNextInt(reader).Value;
            int vertexElementsOffset = 1;
            declData.VertexElements = new XFileVertexElement[declData.NVertexElements];
            int nTotalVertexElementComponents = declData.NVertexElements * nComponentsPerVertexElement;
            for (int i = 0; i < declData.NVertexElements; i++)
            {
                int j = i * nComponentsPerVertexElement + vertexElementsOffset;
                declData.VertexElements[i] = new XFileVertexElement()
                {
                    Type = (VertexElementType)ReadNextInt(reader).Value,
                    Method = (VertexElementMethod)ReadNextInt(reader).Value,
                    Usage = (VertexElementUsage)ReadNextInt(reader).Value,
                    UsageIndex = ReadNextInt(reader).Value,
                };
            }
            declData.NData = ReadNextInt(reader).Value;
            int dataOffset = vertexElementsOffset + nTotalVertexElementComponents + 1;
            declData.Data = new UInt32[declData.NData];
            for (int i = 0; i < declData.NData; i++)
            {
                declData.Data[i] = ReadNextUInt(reader).Value;
            }
            return declData;
        }

        private XFileMeshMaterialListObject ParseMeshMaterialListObject(TextReader reader, string name)
        {
            XFileMeshMaterialListObject meshMaterialList = new XFileMeshMaterialListObject();
            meshMaterialList.Name = name;
            meshMaterialList.NMaterials = ReadNextInt(reader).Value;
            meshMaterialList.NFaceIndices = ReadNextInt(reader).Value;

            meshMaterialList.FaceIndices = new int[meshMaterialList.NFaceIndices];
            for (int i = 0; i < meshMaterialList.NFaceIndices; i++)
            {
                meshMaterialList.FaceIndices[i] = ReadNextInt(reader).Value;
            }
            if (meshMaterialList.NMaterials > 0)
            {
                if (!IsNextObjectReference(reader))
                {
                    meshMaterialList.MaterialRefs = new string[meshMaterialList.NMaterials];
                    for (int i = 0; i < meshMaterialList.NMaterials; i++)
                        meshMaterialList.MaterialRefs[i] = ReadNextReference(reader);
                }
                else
                {
                    meshMaterialList.Materials = new XFileMaterialObject[meshMaterialList.NMaterials];
                    for (int i = 0; i < meshMaterialList.NMaterials; i++)
                    {
                        XFileObjectBlock ob = ReadNextDataBlock(reader);
                        StringReader dataReader = new StringReader(ob.Data);
                        meshMaterialList.Materials[i] = ParseMaterialObject(dataReader, ob.Name);
                    }
                }
            }
            return meshMaterialList;
        }

        private XFileMaterialObject ParseMaterialObject(TextReader reader, string name)
        {
            XFileMaterialObject material = new XFileMaterialObject();
            material.Name = name;
            material.FaceColor = ParseColorRGBA(reader);
            material.Power = ReadNextFloat(reader).Value;
            material.SpecularColor = ParseColorRGB(reader);
            material.EmissiveColor = ParseColorRGB(reader);
            XFileObjectBlock ob = ReadNextDataBlock(reader);
            if (ob != null)
            {
                StringReader dataReader = new StringReader(ob.Data);
                material.TextureFilename = ReadNextString(dataReader);
            }
            return material;
        }

        private XFileMatrix ParseFrameTransformMatrix(TextReader reader)
        {
            XFileMatrix matrix = new XFileMatrix();
            matrix.m = new float[16];
            for (int i = 0; i < matrix.m.Length; i++)
                matrix.m[i] = ReadNextFloat(reader).Value;
            return matrix;
        }

        private XFileColorRGBA ParseColorRGBA(TextReader reader)
        {
            return new XFileColorRGBA()
            {
                Red = ReadNextFloat(reader).Value,
                Green = ReadNextFloat(reader).Value,
                Blue = ReadNextFloat(reader).Value,
                Alpha = ReadNextFloat(reader).Value
            };
        }

        private XFileColorRGB ParseColorRGB(TextReader reader)
        {
            return new XFileColorRGB()
            {
                Red = ReadNextFloat(reader).Value,
                Green = ReadNextFloat(reader).Value,
                Blue = ReadNextFloat(reader).Value
            };
        }

        private XFileHeader ReadHeader(TextReader reader)
        {
            XFileHeader header = new XFileHeader();

            char[] magicNumber = new char[4];
            reader.Read(magicNumber, 0, magicNumber.Length);
            header.MagicNumber = new string(magicNumber);

            char[] majorNumber = new char[2];
            reader.Read(majorNumber, 0, majorNumber.Length);
            header.MajorNumber = short.Parse(new string(majorNumber));

            char[] minorNumber = new char[2];
            reader.Read(minorNumber, 0, minorNumber.Length);
            header.MinorNumber = short.Parse(new string(minorNumber));

            char[] formatType = new char[4];
            reader.Read(formatType, 0, formatType.Length);
            header.FormatType = new string(formatType);

            char[] floatSize = new char[4];
            reader.Read(floatSize, 0, floatSize.Length);
            header.FloatSize = int.Parse(new string(floatSize));

            return header;
        }        

        private static string ReadNextReference(TextReader reader)
        {
            string referenceName = null;
            StringBuilder sb = new StringBuilder();
            XFileReaderState currentState = XFileReaderState.Start;
            while (true)
            {
                int input = reader.Read();
                if (input == -1)
                    break;
                switch (currentState)
                {
                    case XFileReaderState.Start:
                        if ((char)input == '}')
                            currentState = XFileReaderState.Complete;
                        else if ((char)input == '{')
                        {
                            sb.Clear(); //clear unwanted characters preceding the reference name.
                            currentState = XFileReaderState.Data;
                        }
                        break;
                    case XFileReaderState.Data:
                        if ((char)input == '}')
                        {
                            referenceName = sb.ToString().Substring(1).Trim();
                            sb.Clear();
                            currentState = XFileReaderState.Complete;
                        }
                        break;
                }
                if (currentState == XFileReaderState.Complete)
                    break;
                sb.Append((char)input);
            }
            return referenceName;
        }

        private static uint? ReadNextUInt(TextReader reader)
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                int input = reader.Read();
                if (input == -1)
                    break;
                //skip preceding white spaces, semicolons and commas.
                if ((sb.Length == 0) && ((char)input == ';' || input == ',' || char.IsWhiteSpace((char)input)))
                    continue;
                if (!char.IsDigit((char)input) && (char)input != '-')
                    break;
                sb.Append((char)input);
            }
            return sb.Length != 0 ? (uint?)uint.Parse(sb.ToString()) : null;
        }

        private static int? ReadNextInt(TextReader reader)
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                int input = reader.Read();
                if (input == -1)
                    break;
                //skip preceding white spaces, semicolons and commas.
                if ((sb.Length == 0) && ((char)input == ';' || input == ',' || char.IsWhiteSpace((char)input)))
                    continue;
                if (!char.IsDigit((char)input) && (char)input != '-')
                    break;
                sb.Append((char)input);
            }
            return sb.Length != 0 ? (int?)int.Parse(sb.ToString()) : null;
        }

        private static float? ReadNextFloat(TextReader reader)
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                int input = reader.Read();
                if (input == -1)
                    break;
                //skip preceding white spaces, semicolons and commas.
                if ((sb.Length == 0) && ((char)input == ';' || input == ',' || char.IsWhiteSpace((char)input)))
                    continue;
                if (!char.IsDigit((char)input) && (char)input != '-' && (char)input != '.')
                    break;
                sb.Append((char)input);
            }
            return sb.Length != 0 ? (float?)float.Parse(sb.ToString()) : null;
        }

        private static string ReadNextString(TextReader reader)
        {
            StringBuilder sb = new StringBuilder();
            XFileReaderState state = XFileReaderState.Start;
            string result = null;
            while (true)
            {
                int input = reader.Read();
                if (input == -1)
                    break;
                switch (state)
                {
                    case XFileReaderState.Start:
                        if ((char)input == '}')
                            state = XFileReaderState.Complete;
                        else if ((char)input == '"')
                        {
                            state = XFileReaderState.Data;
                            sb.Clear();
                        }
                        break;
                    case XFileReaderState.Data:
                        if ((char)input == '"')
                        {
                            result = sb.ToString().Substring(1);
                            state = XFileReaderState.Complete;
                        }
                        break;
                }
                if (state == XFileReaderState.Complete)
                    break;
                sb.Append((char)input);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        /// <remarks>Expects the next expression in the stream to be either a data block 
        /// or the end of the "containing" block.</remarks>
        private static XFileObjectBlock ReadNextDataBlock(TextReader reader)
        {
            XFileObjectBlock block = new XFileObjectBlock();
            int nUnmatchedBraces = 0;
            XFileReaderState currentState = XFileReaderState.Start;
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                int input = reader.Read();
                if (input == -1)
                    return null;
                else if ((char)input == '}' && nUnmatchedBraces == 0)
                    return null;
                switch (currentState)
                {
                    case XFileReaderState.Start:
                        if (char.IsLetterOrDigit((char)input))
                        {
                            sb.Clear();
                            currentState = XFileReaderState.Type;
                        }
                        break;
                    case XFileReaderState.Type:
                        if ((char)input == '{' || char.IsWhiteSpace((char)input))
                        {
                            block.Type = sb.ToString().Trim();
                            sb.Clear();
                            if ((char)input != '{')
                                currentState = XFileReaderState.Name;
                            else
                                currentState = XFileReaderState.Data;
                            if ((char)input == '{')
                                nUnmatchedBraces++;
                        }
                        break;
                    case XFileReaderState.Name:
                        if ((char)input == '{' || char.IsWhiteSpace((char)input))
                        {
                            block.Name = sb.ToString().Trim();
                            sb.Clear();
                            currentState = XFileReaderState.Data;
                            if ((char)input == '{')
                                nUnmatchedBraces++;
                        }
                        break;
                    case XFileReaderState.Data:
                        if ((char)input == '{')
                            nUnmatchedBraces++;
                        else if ((char)input == '}')
                        {
                            nUnmatchedBraces--;
                            if (nUnmatchedBraces == 0)
                            {
                                block.Data = sb.ToString().Trim().Substring(1).Trim(); //NOTE: The call to Substring(1) removes the opening brace.
                                sb.Clear();
                                currentState = XFileReaderState.Complete;
                            }
                        }
                        break;
                }
                if (currentState == XFileReaderState.Complete)
                    break;
                sb.Append((char)input);
            }
            return block;
        }

        private static bool IsNextObjectReference(TextReader reader)
        {
            while (true)
            {
                int input = reader.Peek();
                if (input == -1)
                    break;
                if (char.IsWhiteSpace((char)input))
                    reader.Read();
                else
                {
                    return ((char)input != '{') ? true : false;
                }
            }
            return false;
        }        

        private enum XFileReaderState
        {
            Start,
            Type,
            Name,
            Data,
            Complete
        }
    }

    public class XFileObjectBlock
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
    }

    public class XFileHeader
    {
        public string MagicNumber { get; set; }
        public short MajorNumber { get; set; }
        public short MinorNumber { get; set; }
        public string FormatType { get; set; }
        public int FloatSize { get; set; }
    }

    public class XFileDataObject
    {
        public string Name { get; set; }
    }

    public class XFileAnimationTicksPerSecondObject : XFileDataObject
    {
        public const string TemplateName = "AnimTicksPerSecond";
        public const string TemplateId = "9e415a43-7ba6-4a73-8743-b73d47e88476";
        public int TicksPerSecond { get; set; }
    }

    public class XFileMaterialObject : XFileDataObject
    {
        public const string TemplateName = "Material";
        public const string TemplateId = "3d82ab4d-62da-11cf-ab39-0020af71e433";
        public XFileColorRGBA FaceColor { get; set; }
        public float Power { get; set; }
        public XFileColorRGB SpecularColor { get; set; }
        public XFileColorRGB EmissiveColor { get; set; }
        public string TextureFilename { get; set; }
    }

    public class XFileFrameObject : XFileDataObject
    {
        public const string TemplateName = "Frame";
        public const string TemplateId = "3d82ab46-62da-11cf-ab39-0020af71e433";
        private List<XFileMeshObject> _meshes = new List<XFileMeshObject>();
        private List<XFileFrameObject> _childFrames = new List<XFileFrameObject>();

        public XFileMatrix FrameTransformMatrix { get; set; }
        public List<XFileMeshObject> Meshes { get { return _meshes; } }
        public List<XFileFrameObject> ChildFrames { get { return _childFrames; } }
    }

    public class XFileMeshObject : XFileDataObject
    {
        public const string TemplateName = "Mesh";
        public const string TemplateId = "3d82ab44-62da-11cf-ab39-0020af71e433";
        private List<XFileSkinWeightsObject> _skinWeightsCollection = new List<XFileSkinWeightsObject>();
        public int NVertices { get; set; }
        public XFileVector[] Vertices { get; set; }
        public int NFaces { get; set; }
        public XFileMeshFaceObject[] Faces { get; set; }
        public XFileMeshMaterialListObject MeshMaterialList { get; set; }
        public XFileDeclDataObject DeclData { get; set; }
        public List<XFileSkinWeightsObject> SkinWeightsCollection { get { return _skinWeightsCollection; } }
        public XFileTextureCoords MeshTextureCoords { get; set; }
        public XFileMeshVertexColors MeshVertexColors { get; set; }
        public XFileMeshNormals MeshNormals { get; set; }
    }

    public class XFileTextureCoords : XFileDataObject
    {
        public const string TemplateName = "MeshTextureCoords";
        public const string TemplateId = "f6f23f40-7686-11cf-8f52-0040333594a3";
        public int NTextureCoords { get; set; }
        public Vector2[] TextureCoords { get; set; }
    }

    public class XFileMeshVertexColors : XFileDataObject
    {
        public const string TemplateName = "MeshVertexColors";
        public const string TemplateId = "1630b821-7842-11cf-8f52-0040333594a3";
        public int NVertexColors { get; set; }
        public XFileIndexedColor[] VertexColors { get; set; }
    }

    public class XFileMeshNormals : XFileDataObject
    {
        public const string TemplateName = "MeshNormals";
        public const string TemplateId = "f6f23f43-7686-11cf-8f52-0040333594a3";
        public int NNormals { get; set; }
        public Vector3[] Normals { get; set; }
        public int NFaceNormals { get; set; }
        public XFileMeshFaceObject[] FaceNormals { get; set; }
    }

    public class XFileMeshFaceObject : XFileDataObject
    {
        public const string TemplateName = "MeshFace";
        public const string TemplateId = "3d82ab5f-62da-11cf-ab39-0020af71e433";
        public int NFaceVertexIndices { get; set; }
        public int[] FaceVertexIndices { get; set; }
    }

    public class XFileMeshMaterialListObject : XFileDataObject
    {
        public const string TemplateName = "MeshMaterialList";
        public const string TemplateId = "f6f23f42-7686-11cf-8f52-0040333594a3";
        public int NMaterials { get; set; }
        public int NFaceIndices { get; set; }
        public int[] FaceIndices { get; set; }
        public string[] MaterialRefs { get; set; }
        public XFileMaterialObject[] Materials { get; set; }
    }

    public class XFileDeclDataObject : XFileDataObject
    {
        public const string TemplateName = "DeclData";
        public const string TemplateId = "bf22e553-292c-4781-9fea-62bd554bdd93";
        public int NVertexElements { get; set; }
        public XFileVertexElement[] VertexElements { get; set; }
        public int NData { get; set; }
        public UInt32[] Data { get; set; }

        public Vector2[] GetTextureCoords(int usageInstanceIndex = 0)
        {
            int? dataOffset = GetVertexDataOffset(VertexElementUsage.TexCoord, usageInstanceIndex);
            int dataStride = GetVertexDataStride();
            if (dataOffset.HasValue)
            {
                int nTexCoords = NData / dataStride;
                Vector2[] texCoords = new Vector2[nTexCoords];
                for (int i = 0; i < nTexCoords; i++)
                {
                    int k = dataOffset.Value + (dataStride * i);
                    float u = BitConverter.ToSingle(BitConverter.GetBytes(Data[k]), 0);
                    float v = BitConverter.ToSingle(BitConverter.GetBytes(Data[k + 1]), 0);
                    texCoords[i] = new Vector2(u, v);
                }
                return texCoords;
            }
            else
                return null;
        }

        public Vector3[] GetNormals(int usageInstanceIndex = 0)
        {
            int? dataOffset = GetVertexDataOffset(VertexElementUsage.Normal, usageInstanceIndex);
            int dataStride = GetVertexDataStride();
            if (dataOffset.HasValue)
            {
                int nNormals = NData / dataStride;
                Vector3[] normals = new Vector3[nNormals];
                for (int i = 0; i < nNormals; i++)
                {
                    int k = dataOffset.Value + (dataStride * i);
                    float x = BitConverter.ToSingle(BitConverter.GetBytes(Data[k]), 0);
                    float y = BitConverter.ToSingle(BitConverter.GetBytes(Data[k + 1]), 0);
                    float z = BitConverter.ToSingle(BitConverter.GetBytes(Data[k + 2]), 0);
                    normals[i] = new Vector3(x, y, z);
                }
                return normals;
            }
            else
                return null;
        }

        private int GetVertexDataStride()
        {
            int stride = 0;
            for (int i = 0; i < NVertexElements; i++)
                stride += GetElementDWORDSize(VertexElements[i].Type);
            return stride;
        }

        private int? GetVertexDataOffset(VertexElementUsage usage, int usageInstanceIndex)
        {
            int usageInstancesFound = 0;
            int offset = 0;
            for (int i = 0; i < NVertexElements; i++)
            {
                if (VertexElements[i].Usage == usage)
                {
                    if (usageInstanceIndex == usageInstancesFound)
                        return offset;
                    else
                        usageInstancesFound++;
                }
                offset += GetElementDWORDSize(VertexElements[i].Type);
            }
            return null;
        }

        private static int GetElementDWORDSize(VertexElementType type)
        {
            switch (type)
            {
                case VertexElementType.Float1:
                    return 1;
                case VertexElementType.Float2:
                    return 2;
                case VertexElementType.Float3:
                    return 3;
                case VertexElementType.Float4:
                    return 4;
                default:
                    throw new NotImplementedException();
            }
        }      
    }

    public class XFileSkinWeightsObject : XFileDataObject
    {
        public const string TemplateName = "SkinWeights";
        public const string TemplateId = "6f0d123b-bad2-4167-a0d0-80224f25fabb";
        public string TransformNodeName { get; set; }
        public int NWeights { get; set; }
        public int[] VertexIndices { get; set; }
        public float[] Weights { get; set; }
        public XFileMatrix MatrixOffset { get; set; }
    }

    public class XFileAnimationObject : XFileDataObject
    {
        public const string TemplateName = "Animation";
        public const string TemplateId = "3d82ab4f-62da-11cf-ab39-0020af71e433";
        private List<XFileAnimationKeyObject> _keys = new List<XFileAnimationKeyObject>();
        public string FrameRef { get; set; }
        public XFileAnimationOptions? Options { get; set; }
        public List<XFileAnimationKeyObject> Keys { get { return _keys; } }      
    }

    public class XFileAnimationSetObject : XFileDataObject
    {
        public const string TemplateName = "AnimationSet";
        public const string TemplateId = "3d82ab50-62da-11cf-ab39-0020af71e433";
        private List<XFileAnimationObject> _animations = new List<XFileAnimationObject>();
        public List<XFileAnimationObject> Animations { get { return _animations; } }        
    }

    public class XFileAnimationKeyObject : XFileDataObject
    {
        public const string TemplateName = "AnimationKey";
        public const string TemplateId = "10dd46a8-775b-11cf-8f52-0040333594a3";
        public KeyType KeyType { get; set; }
        public int NKeys { get; set; }
        public XFileTimedFloatKey[] TimedFloatKeys { get; set; }
    }

    public enum PositionQuality
    {
        Spline = 0,
        Linear = 1
    }

    public enum KeyType
    {
        Rotation = 0,
        Scale = 1,
        Position = 2,
        Matrix = 3,
        Unknown = 4,
    }

    public enum VertexElementType
    {
        Float1 = 0,
        Float2 = 1,
        Float3 = 2,
        Float4 = 3,
        Color = 4,
        UByte4 = 5,
        Short2 = 6,
        Short4 = 7,
        UByte4N = 8,
        Short2N = 9,
        Short4N = 10,
        UShort2N = 11,
        UShort4N = 12,
        UDec3 = 13,
        Dec3N = 14,
        Float16_2 = 15,
        Float16_4 = 16,
        Unused = 17
    }

    public enum VertexElementMethod
    {
        Default = 0,
        PartialU = 1,
        PartialL = 2,
        CrossUV = 3,
        UV = 4,
        LookUp = 5,
        LookUpResampled = 6
    }

    public enum VertexElementUsage
    {
        Position = 0,
        BlendWeight = 1,
        BlendIndices = 2,
        Normal = 3,
        PSize = 4,
        TexCoord = 5,
        Tangent = 6,
        Binormal = 7,
        TessFactor = 8,
        PositionT = 9,
        Color = 10,
        Fog = 11,
        Depth = 12,
        Sample = 13
    }

    public struct XFileVertexElement
    {
        public VertexElementType Type { get; set; }
        public VertexElementMethod Method { get; set; }
        public VertexElementUsage Usage { get; set; }
        public int UsageIndex { get; set; }
    }

    public struct XFileTimedFloatKey
    {
        public int Time;
        public int NValues;
        public float[] Values;
    }
    
    public struct XFileAnimationOptions
    {
        public bool OpenClosed;
        public PositionQuality PositionQuality;
    }

    public struct XFileColorRGBA
    {
        public float Red;
        public float Green;
        public float Blue;
        public float Alpha;
    }

    public struct XFileColorRGB
    {
        public float Red;
        public float Green;
        public float Blue;
    }

    public struct XFileIndexedColor
    {
        public int Index;
        public XFileColorRGBA ColorRGBA;
    }

    public struct XFileMatrix
    {
        public float[] m;
    }

    public struct XFileVector
    {
        public float X;
        public float Y;
        public float Z;
    }

    public class DataObjectCollection : List<XFileDataObject>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">1-based instance index</param>
        /// <returns></returns>
        public T GetDataObject<T>(int instance) where T : XFileDataObject
        {
            if (instance <= 0)
                throw new ArgumentException("instance must be greater than zero", nameof(instance));

            var instances = GetDataObjects<T>();

            if (instances.Length < instance)
                return null;
            else
                return instances[instance - 1];
        }

        public T[] GetDataObjects<T>() where T : XFileDataObject
        {
            var results = new List<T>();
            foreach (XFileDataObject dataObj in this)
            {
                if (dataObj is T)
                {
                    results.Add((T)dataObj);
                }
            }
            return results.ToArray();
        }
    }
}
