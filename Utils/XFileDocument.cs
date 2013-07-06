using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Utils
{       
    public abstract class XFileDocument
    {        
        private List<XFileDataObject> dataObjects = new List<XFileDataObject>();

        public XFileHeader Header { get; protected set; }

        public List<XFileDataObject> DataObjects { get { return dataObjects; } }
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
                ReadNextDataObject(reader, out nextDataObject);
                if (nextDataObject != null)
                    DataObjects.Add(nextDataObject);
            }
        }

        private void ReadNextDataObject(TextReader reader, out XFileDataObject dataObject)
        {            
            dataObject = null;
            //string dataObjectPattern = @"(?<!template\s*)\b(?<data>(?<type>[^{}\s;]+)\s+(?:(?<!template\s*)\b(?<name>[^{}\s]+)\s+)?\{((?:[^{}]|(?<open>\{)|(?<-open>\}))+(?(open)(?!)))\})";
            //Regex regEx = new Regex(dataObjectPattern);
            //Match match = regEx.Match(content, startPosition);
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
                    //case XFileAnimationObject.TemplateName:
                    //    dataObject = ParseAnimationObject(match.Groups["data"].Value, match.Groups["name"].Value);
                    //    //frame.Animations.Add((XFileAnimationObject)childObject);
                    //    break;
                    case XFileAnimationTicksPerSecondObject.TemplateName:
                        dataObject = ParseAnimationTicksPerSecond(dataReader, ob.Name);
                        break;
                    case XFileAnimationSetObject.TemplateName:
                        dataObject = ParseAnimationSetObject(dataReader, ob.Name);
                        //frame.AnimationSets.Add((XFileAnimationSetObject)childObject);
                        break;
                }                
            }
        }                    

        public XFileAnimationTicksPerSecondObject ParseAnimationTicksPerSecond(TextReader reader, string name)
        {
            //string valuesPattern = @"(?![^\{]+\{)\d+(?:\.\d+)?";
            //Match match = Regex.Match(content, valuesPattern);
            XFileAnimationTicksPerSecondObject result = new XFileAnimationTicksPerSecondObject();
            result.TicksPerSecond = ReadNextInt(reader).Value;
            return result;
        }

        public XFileFrameObject ParseFrameObject(TextReader reader, string name)
        {
            XFileFrameObject frame = new XFileFrameObject();
            frame.Name = name;
            //string childObjectPattern = @"(?<=(?:\{\s*)|(\}\s*))(?<data>(?<type>[^{}\s;]+)\s+(?:(?<name>[^{}\s]+)\s+)?\{((?:[^{}]|(?<open>\{)|(?<-open>\}))+(?(open)(?!)))\})";
            //Regex regEx = new Regex(childObjectPattern);
            //MatchCollection collection = regEx.Matches(frameContent);
            //foreach (Match match in collection)
            XFileObjectBlock ob = ReadNextDataBlock(reader);
            while(ob != null)
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

            //string childObjectPattern = @"(?<=(?:\{\s*)|(\}\s*))(?<data>(?<type>[^{}\s;]+)\s+(?:(?<name>[^{}\s]+)\s+)?\{((?:[^{}]|(?<open>\{)|(?<-open>\}))+(?(open)(?!)))\})";
            //Regex regEx = new Regex(childObjectPattern);
            //MatchCollection collection = regEx.Matches(animationSetContent);
            XFileObjectBlock ob = ReadNextDataBlock(reader);
            //foreach (Match match in collection)
            while(ob != null)
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
            //string childObjectPatern = @"(?<=(?:\{\s*)|(\}\s*))(?<data>(?<type>[^{}\s;]+)\s+(?:(?<name>[^{}\s]+)\s+)?\{((?:[^{}]|(?<open>\{)|(?<-open>\}))+(?(open)(?!)))\})";
            //string frameReferencePattern = @"(?<! Animation\s*){\s*(?<frame>[^\s]*)\s*}";
            //Match frameMatch = Regex.Match(animationContent, frameReferencePattern);
            animation.FrameRef = ReadNextReference(reader); // frameMatch.Groups["frame"].Value;
            //MatchCollection collection = Regex.Matches(animationContent, childObjectPatern);
            XFileObjectBlock ob = ReadNextDataBlock(reader);
            //foreach (Match match in collection)
            while(ob != null)
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
           // string valuesPattern = @"(?![^\{]+\{)\d+(?:\.\d+)?";
           // MatchCollection matches = Regex.Matches(animationKeyContent, valuesPattern);
            animationKey.KeyType = (KeyType)Math.Min(ReadNextInt(reader).Value, 3); //I've capped this to deal with an apparent bug in an exporter, which writes '4' for matrix keys.
            animationKey.NKeys = ReadNextInt(reader).Value;
            const int tfkOffset = 2;
            animationKey.TimedFloatKeys = new XFileTimedFloatKey[animationKey.NKeys];
            int nAnimationKeyComponents = 0;
            switch (animationKey.KeyType)
            {
                case KeyType.Matrix:
                    nAnimationKeyComponents = 18; //1 (time) + 1 (nvalues) + 16 (matrix)
                    break;
                case KeyType.Position:
                    nAnimationKeyComponents = 5;  //1 (time) + 1 (nvalues) + 3 (translation vector)
                    break;
                case KeyType.Scale:
                    nAnimationKeyComponents = 5; //1 (time) + 1 (nvalues) + 3 (scale vector)
                    break;
                case KeyType.Rotation:
                    nAnimationKeyComponents = 6; //1 (time) + 1 (nvalues) + 4 (rotation quaternion)
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
            const int nVertexComponents = 3;
            //************************************************
            //NOTE: We always assume faces are triangular    *
            //************************************************     
            const int nFaceComponents = 4;
            XFileMeshObject mesh = new XFileMeshObject();
            mesh.Name = name;
            //string childObjectPattern = @"(?<=(?:\{\s*)|(\}\s*)|(;;\s*))(?<data>(?<type>[^{}\s;]+)\s+(?:(?<name>[^{}\s]+)\s+)?\{((?:[^{}]|(?<open>\{)|(?<-open>\}))+(?(open)(?!)))\})";
            //string childLessMeshContent = Regex.Replace(meshContent, childObjectPattern, string.Empty);
            //string valuesPattern = @"(?![^\{]+\{)\d+(?:\.\d+)?";
            //MatchCollection matches = Regex.Matches(childLessMeshContent, valuesPattern);
            mesh.NVertices = ReadNextInt(reader).Value; //int.Parse(matches[0].Value);
            int verticesOffset = 1;
            mesh.Vertices = new XFileVector[mesh.NVertices];
            for (int i = 0; i < mesh.NVertices; i++)
            {
                int j = verticesOffset + i * nVertexComponents;
                mesh.Vertices[i] = new XFileVector()
                {
                    X = ReadNextFloat(reader).Value,
                    Y = ReadNextFloat(reader).Value,
                    Z = ReadNextFloat(reader).Value
                };
            }
            mesh.NFaces = ReadNextInt(reader).Value;
            int facesOffset = verticesOffset + (mesh.NVertices * nVertexComponents) + 1;
            mesh.Faces = new XFileMeshFaceObject[mesh.NFaces];

            for (int i = 0; i < mesh.NFaces; i++)
            {
                int j = i * nFaceComponents + facesOffset;
                //NOTE: Assuming triangular faces, we expect this value to always be '3'.
                int nFaceIndices = ReadNextInt(reader).Value;
                mesh.Faces[i] = new XFileMeshFaceObject()
                {
                    NFaceVertexIndices = nFaceIndices,
                    FaceVertexIndices = new int[] { 
                        ReadNextInt(reader).Value,
                        ReadNextInt(reader).Value,
                        ReadNextInt(reader).Value }
                };
            }

            //MatchCollection collection = Regex.Matches(meshContent, childObjectPattern);
            XFileObjectBlock ob = ReadNextDataBlock(reader);
            //foreach (Match match in collection)
            while(ob != null)
            {
                XFileDataObject childObject = null;
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
                }
                ob = ReadNextDataBlock(reader);
            }

            return mesh;
        }

        private XFileSkinWeightsObject ParseSkinWeightsObject(TextReader reader, string name)
        {
            XFileSkinWeightsObject skinWeights = new XFileSkinWeightsObject();
            skinWeights.Name = name;
            //string boneReferencePattern = @"""(?<bone>[^""]+)""";
            //string valuesPattern = @"(?![^\{]+\{)\d+(?:\.\d+)?";
            //string childLessSkinWeightsContent = Regex.Replace(skinWeightsContent, boneReferencePattern, string.Empty);
            skinWeights.TransformNodeName = ReadNextString(reader); //Regex.Match(skinWeightsContent, boneReferencePattern).Groups["bone"].Value;
            //MatchCollection matches = Regex.Matches(childLessSkinWeightsContent, valuesPattern);
            skinWeights.NWeights = ReadNextInt(reader).Value; //int.Parse(matches[0].Value);
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

        private XFileDeclDataObject ParseDeclDataObject(TextReader reader, string name)
        {
            const int nComponentsPerVertexElement = 4;
            XFileDeclDataObject declData = new XFileDeclDataObject();
            declData.Name = name;
            //string valuesPattern = @"(?![^\{]+\{)\d+(?:\.\d+)?";
            //MatchCollection matches = Regex.Matches(declDataContent, valuesPattern);
            declData.NVertexElements = ReadNextInt(reader).Value; //int.Parse(matches[0].Value);
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
                    UsageIndex = (VertexElementUsage)ReadNextInt(reader).Value
                };
            }
            declData.NData = ReadNextInt(reader).Value;
            int dataOffset = vertexElementsOffset + nTotalVertexElementComponents + 1;
            declData.Data = new long[declData.NData];
            for (int i = 0; i < declData.NData; i++)
            {
                declData.Data[i] = ReadNextLong(reader).Value;
            }
            return declData;
        }

        private XFileMeshMaterialListObject ParseMeshMaterialListObject(TextReader reader, string name)
        {
            XFileMeshMaterialListObject meshMaterialList = new XFileMeshMaterialListObject();
            meshMaterialList.Name = name;
            //string materialReferencesPattern = @"(?<! MeshMaterialList\s*){\s*(?<material>[^\s]*\s*)}";
            //string inlineMaterialsPattern = @"(?<!MeshMaterialList\s*)Material\s*{\s*(?<material>[^}]*\s*)}";
            //string valuesPattern = @"(?![^\{]+\{)\d+(?:\.\d+)?";
            //string childLessContent = Regex.Replace(Regex.Replace(meshMaterialListContent, materialReferencesPattern, string.Empty), inlineMaterialsPattern, string.Empty);
            //MatchCollection matches = Regex.Matches(childLessContent, valuesPattern);
            meshMaterialList.NMaterials = ReadNextInt(reader).Value;
            meshMaterialList.NFaceIndices = ReadNextInt(reader).Value;
            int faceIndicesOffset = 2;
            meshMaterialList.FaceIndices = new int[meshMaterialList.NFaceIndices];
            for (int i = 0; i < meshMaterialList.NFaceIndices; i++)
            {
                meshMaterialList.FaceIndices[i] = ReadNextInt(reader).Value;
            }
            if (meshMaterialList.NMaterials > 0)
            {                
                //MatchCollection collection = Regex.Matches(meshMaterialListContent, materialReferencesPattern);
                //NOTE: We're assuming that either all child materials are references or that all child materials
                //are inline.
                //if (collection.Count != 0)
                if (!IsNextObjectReference(reader))
                {
                    meshMaterialList.MaterialRefs = new string[meshMaterialList.NMaterials];
                    for (int i = 0; i < meshMaterialList.NMaterials; i++)
                        meshMaterialList.MaterialRefs[i] = ReadNextReference(reader); //collection[i].Groups["material"].Value;
                }
                else
                {
                    //collection = Regex.Matches(meshMaterialListContent, inlineMaterialsPattern);                    
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
            //string valuesPattern = @"(?![^\{]+\{)\d+(?:\.\d+)?"; //@"(\d+.\d+)\s*";   
            //string textureFileNameExpression = @"TextureFilename\s*{\s*""(?<texturename>[^""]+)""\s*;\s*}";
            //string childLessMaterialContent = Regex.Replace(materialContent, textureFileNameExpression, string.Empty);
            //MatchCollection matches = Regex.Matches(childLessMaterialContent, valuesPattern);
            material.FaceColor = ParseColorRGBA(reader);
            material.Power = ReadNextFloat(reader).Value;
            material.SpecularColor = ParseColorRGB(reader);
            material.EmissiveColor = ParseColorRGB(reader);
            //Match match = Regex.Match(materialContent, textureFileNameExpression);
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
            //string valuesPattern = @"(?![^\{]+\{)\d+(?:\.\d+)?";
            //MatchCollection matches = Regex.Matches(matrixContent, valuesPattern);
            XFileMatrix matrix = new XFileMatrix();
            matrix.m = new float[16];
            for (int i = 0; i < matrix.m.Length; i++)
                matrix.m[i] = ReadNextFloat(reader).Value;
            return matrix;
        }        

        //private string ReadHeader(TextReader reader)
        //{
        //    char[] result = new char[16];
        //    reader.Read(result, 0, 16);
        //    return new string(result);
        //}           

        private XFileColorRGBA ParseColorRGBA(TextReader reader)
        {
            //string valuesPattern = @"(?<value>(?![^\{]+\{)\d+(?:\.\d+)?);";
            //MatchCollection collection = Regex.Matches(rgbaContent, valuesPattern);
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
            //string valuesPattern = @"(?<value>(?![^\{]+\{)\d+(?:\.\d+)?);";
            //MatchCollection collection = Regex.Matches(rgbContent, valuesPattern);
            return new XFileColorRGB()
            {
                Red = ReadNextFloat(reader).Value,
                Green = ReadNextFloat(reader).Value,
                Blue = ReadNextFloat(reader).Value
            };
        }
        
        private XFileHeader ReadHeader(TextReader reader)
        {
            //string headerContent = ReadHeader(reader);

            XFileHeader header = new XFileHeader();
            //StringReader reader = new StringReader(headerContent);

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

        private static int? ReadNextInt(TextReader reader)
        {
            long? result = ReadNextLong(reader);
            return result.HasValue ? (int?)result.Value : null;
        }

        private static long? ReadNextLong(TextReader reader)
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
                if (!char.IsDigit((char)input))
                    break;
                sb.Append((char)input);
            }
            return sb.Length != 0 ? (long?)long.Parse(sb.ToString()) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>         
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
                if (!char.IsDigit((char)input) && (char)input != '.')
                    break;
                sb.Append((char)input);
            }
            return sb.Length != 0 ? (float?)float.Parse(sb.ToString()) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>      
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
                switch(state)
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
                switch(currentState)
                {
                    case XFileReaderState.Start:
                        if(char.IsLetterOrDigit((char)input))
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
                                block.Data = sb.ToString().Substring(1).Trim(); //NOTE: The substring removes the opening brace.
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

    //public class XFileTextDocument : XFileDocument
    //{
    //    public void Load(string textFileContent)
    //    {
    //        //TODO: Strip comments from file content here.

    //        string headerContent = ReadHeader(textFileContent);
    //        Header = ParseHeader(headerContent);

    //        XFileDataObject nextDataObject = null;
    //        int readPosition = 0;
    //        while (readPosition < textFileContent.Length)
    //        {
    //            readPosition = ReadNextDataObject(textFileContent, readPosition, out nextDataObject);
    //            if (nextDataObject != null)
    //                DataObjects.Add(nextDataObject);
    //        }
    //    }

    //    private int ReadNextDataObject(string content, int startPosition, out XFileDataObject dataObject)
    //    {
    //        dataObject = null;           
    //        string dataObjectPattern = @"(?<!template\s*)\b(?<data>(?<type>[^{}\s;]+)\s+(?:(?<!template\s*)\b(?<name>[^{}\s]+)\s+)?\{((?:[^{}]|(?<open>\{)|(?<-open>\}))+(?(open)(?!)))\})";
    //        Regex regEx = new Regex(dataObjectPattern);
    //        Match match = regEx.Match(content, startPosition);
    //        if (match.Success)
    //        {
    //            switch (match.Groups["type"].Value)
    //            {
    //                case XFileMaterialObject.TemplateName:
    //                    dataObject = ParseMaterialObject(match.Groups["data"].Value, match.Groups["name"].Value);
    //                    break;
    //                case XFileFrameObject.TemplateName:
    //                    dataObject = ParseFrameObject(match.Groups["data"].Value, match.Groups["name"].Value);
    //                    break;
    //                case XFileMeshObject.TemplateName:
    //                    dataObject = ParseMeshObject(match.Groups["data"].Value, match.Groups["name"].Value);
    //                    break;
    //                //case XFileAnimationObject.TemplateName:
    //                //    dataObject = ParseAnimationObject(match.Groups["data"].Value, match.Groups["name"].Value);
    //                //    //frame.Animations.Add((XFileAnimationObject)childObject);
    //                //    break;
    //                case XFileAnimationTicksPerSecondObject.TemplateName:
    //                    dataObject = ParseAnimationTicksPerSecond(match.Groups["data"].Value, match.Groups["name"].Value);
    //                    break;
    //                case XFileAnimationSetObject.TemplateName:
    //                    dataObject = ParseAnimationSetObject(match.Groups["data"].Value, match.Groups["name"].Value);
    //                    //frame.AnimationSets.Add((XFileAnimationSetObject)childObject);
    //                    break;
    //            }               
    //            return match.Index + match.Length;
    //        }
    //        else
    //            return content.Length;
    //    }

    //    public XFileAnimationTicksPerSecondObject ParseAnimationTicksPerSecond(string content, string name)
    //    {
    //        string valuesPattern = @"(?![^\{]+\{)\d+(?:\.\d+)?";
    //        Match match = Regex.Match(content, valuesPattern);
    //        XFileAnimationTicksPerSecondObject result = new XFileAnimationTicksPerSecondObject();
    //        result.TicksPerSecond = int.Parse(match.Value);
    //        return result;
    //    }

    //    public XFileFrameObject ParseFrameObject(string frameContent, string name)
    //    {
    //        XFileFrameObject frame = new XFileFrameObject();
    //        frame.Name = name;
    //        string childObjectPattern = @"(?<=(?:\{\s*)|(\}\s*))(?<data>(?<type>[^{}\s;]+)\s+(?:(?<name>[^{}\s]+)\s+)?\{((?:[^{}]|(?<open>\{)|(?<-open>\}))+(?(open)(?!)))\})";
    //        Regex regEx = new Regex(childObjectPattern);
    //        MatchCollection collection = regEx.Matches(frameContent);            
    //        foreach (Match match in collection)
    //        {
    //            XFileDataObject childObject = null;
    //            switch (match.Groups["type"].Value)
    //            {
    //                case "FrameTransformMatrix":
    //                    frame.FrameTransformMatrix = ParseFrameTransformMatrix(match.Groups["data"].Value, match.Groups["name"].Value);                        
    //                    break;
    //                case XFileFrameObject.TemplateName:
    //                    childObject = ParseFrameObject(match.Groups["data"].Value, match.Groups["name"].Value);
    //                    frame.ChildFrames.Add((XFileFrameObject)childObject);
    //                    break;
    //                case XFileMeshObject.TemplateName:
    //                    childObject = ParseMeshObject(match.Groups["data"].Value, match.Groups["name"].Value);
    //                    frame.Meshes.Add((XFileMeshObject)childObject);
    //                    break;                    
    //            }
    //        }            
    //        return frame;
    //    }

    //    private XFileDataObject ParseAnimationSetObject(string animationSetContent, string name)
    //    {
    //        XFileAnimationSetObject animationSet = new XFileAnimationSetObject();
    //        animationSet.Name = name;

    //        string childObjectPattern = @"(?<=(?:\{\s*)|(\}\s*))(?<data>(?<type>[^{}\s;]+)\s+(?:(?<name>[^{}\s]+)\s+)?\{((?:[^{}]|(?<open>\{)|(?<-open>\}))+(?(open)(?!)))\})";
    //        Regex regEx = new Regex(childObjectPattern);
    //        MatchCollection collection = regEx.Matches(animationSetContent);
    //        foreach (Match match in collection)
    //        {
    //            XFileAnimationObject childObject = null;
    //            switch (match.Groups["type"].Value)
    //            {                    
    //                case XFileAnimationObject.TemplateName:
    //                    childObject = ParseAnimationObject(match.Groups["data"].Value, match.Groups["name"].Value);
    //                    animationSet.Animations.Add(childObject);
    //                    break;
    //            }
    //        }

    //        return animationSet;
    //    }

    //    private XFileAnimationObject ParseAnimationObject(string animationContent, string name)
    //    {
    //        XFileAnimationObject animation = new XFileAnimationObject();
    //        animation.Name = name;
    //        string childObjectPatern = @"(?<=(?:\{\s*)|(\}\s*))(?<data>(?<type>[^{}\s;]+)\s+(?:(?<name>[^{}\s]+)\s+)?\{((?:[^{}]|(?<open>\{)|(?<-open>\}))+(?(open)(?!)))\})";             
    //        string frameReferencePattern = @"(?<! Animation\s*){\s*(?<frame>[^\s]*)\s*}";
    //        Match frameMatch = Regex.Match(animationContent, frameReferencePattern);
    //        animation.FrameRef = frameMatch.Groups["frame"].Value;
    //        MatchCollection collection = Regex.Matches(animationContent, childObjectPatern);
    //        foreach (Match match in collection)
    //        {
    //            XFileDataObject childObject = null;
    //            switch (match.Groups["type"].Value)
    //            {
    //                case XFileAnimationKeyObject.TemplateName:
    //                    childObject = ParseAnimationKeyObject(match.Groups["data"].Value, match.Groups["name"].Value);
    //                    animation.Keys.Add((XFileAnimationKeyObject)childObject);
    //                    break;
    //            }
    //        }
    //        return animation;
    //    }

    //    private XFileAnimationKeyObject ParseAnimationKeyObject(string animationKeyContent, string name)
    //    {
    //        XFileAnimationKeyObject animationKey = new XFileAnimationKeyObject();
    //        animationKey.Name = name;
    //        string valuesPattern = @"(?![^\{]+\{)\d+(?:\.\d+)?";
    //        MatchCollection matches = Regex.Matches(animationKeyContent, valuesPattern);
    //        animationKey.KeyType = (KeyType)Math.Min(int.Parse(matches[0].Value), 3); //I've capped this to deal with an apparent bug in an exporter, which writes '4' for matrix keys.
    //        animationKey.NKeys = int.Parse(matches[1].Value);
    //        const int tfkOffset = 2;
    //        animationKey.TimedFloatKeys = new XFileTimedFloatKey[animationKey.NKeys];            
    //        int nAnimationKeyComponents = 0;
    //        switch (animationKey.KeyType)
    //        {
    //            case KeyType.Matrix:
    //                nAnimationKeyComponents = 18; //1 (time) + 1 (nvalues) + 16 (matrix)
    //                break;
    //            case KeyType.Position:
    //                nAnimationKeyComponents = 5;  //1 (time) + 1 (nvalues) + 3 (translation vector)
    //                break;
    //            case KeyType.Scale:
    //                nAnimationKeyComponents = 5; //1 (time) + 1 (nvalues) + 3 (scale vector)
    //                break;
    //            case KeyType.Rotation:
    //                nAnimationKeyComponents = 6; //1 (time) + 1 (nvalues) + 4 (rotation quaternion)
    //                break;
    //        }
    //        for (int i = 0; i < animationKey.NKeys; i++)
    //        {
    //            int j = i * nAnimationKeyComponents + tfkOffset;
    //            XFileTimedFloatKey timedFloatKey = new XFileTimedFloatKey();
    //            timedFloatKey.Time = int.Parse(matches[j].Value);
    //            timedFloatKey.NValues = int.Parse(matches[j + 1].Value);
    //            timedFloatKey.Values = new float[timedFloatKey.NValues];
    //            const int valuesOffset = 2;
    //            for (int k = 0; k < timedFloatKey.NValues; k++)
    //            {
    //                int m = j + k + valuesOffset;
    //                timedFloatKey.Values[k] = float.Parse(matches[m].Value);
    //            }
    //            animationKey.TimedFloatKeys[i] = timedFloatKey;
    //        }           
    //        return animationKey;
    //    }

    //    private XFileMeshObject ParseMeshObject(string meshContent, string name)
    //    {
    //        const int nVertexComponents = 3;
    //        //************************************************
    //        //NOTE: We always assume faces are triangular    *
    //        //************************************************     
    //        const int nFaceComponents = 4;
    //        XFileMeshObject mesh = new XFileMeshObject();
    //        mesh.Name = name;
    //        string childObjectPattern = @"(?<=(?:\{\s*)|(\}\s*)|(;;\s*))(?<data>(?<type>[^{}\s;]+)\s+(?:(?<name>[^{}\s]+)\s+)?\{((?:[^{}]|(?<open>\{)|(?<-open>\}))+(?(open)(?!)))\})";
    //        string childLessMeshContent = Regex.Replace(meshContent, childObjectPattern, string.Empty);
    //        string valuesPattern = @"(?![^\{]+\{)\d+(?:\.\d+)?";            
    //        MatchCollection matches = Regex.Matches(childLessMeshContent, valuesPattern);
    //        mesh.NVertices = int.Parse(matches[0].Value);
    //        int verticesOffset = 1;           
    //        mesh.Vertices = new XFileVector[mesh.NVertices];
    //        for (int i = 0; i < mesh.NVertices; i++)
    //        {
    //            int j = verticesOffset + i * nVertexComponents;
    //            mesh.Vertices[i] = new XFileVector()
    //            {
    //                X = float.Parse(matches[j].Value),
    //                Y = float.Parse(matches[j + 1].Value),
    //                Z = float.Parse(matches[j + 2].Value)
    //            };
    //        }
    //        mesh.NFaces = int.Parse(matches[verticesOffset + (mesh.NVertices * nVertexComponents)].Value);
    //        int facesOffset = verticesOffset + (mesh.NVertices * nVertexComponents) + 1;
    //        mesh.Faces = new XFileMeshFaceObject[mesh.NFaces];
       
    //        for (int i = 0; i < mesh.NFaces; i ++)
    //        {
    //            int j = i * nFaceComponents + facesOffset;
    //            //NOTE: Assuming triangular faces, we expect this value to always be '3'.
    //            int nFaceIndices = int.Parse(matches[j].Value);
    //            mesh.Faces[i] = new XFileMeshFaceObject()
    //            {
    //                NFaceVertexIndices = nFaceIndices,
    //                FaceVertexIndices = new int[] { 
    //                    int.Parse(matches[j + 1].Value),
    //                    int.Parse(matches[j + 2].Value),
    //                    int.Parse(matches[j + 3].Value) }
    //            };
    //        }
            
    //        MatchCollection collection = Regex.Matches(meshContent, childObjectPattern);            
    //        foreach (Match match in collection)
    //        {
    //            XFileDataObject childObject = null;
    //            switch (match.Groups["type"].Value)
    //            {
    //                case XFileMeshMaterialListObject.TemplateName:
    //                    childObject = ParseMeshMaterialListObject(match.Groups["data"].Value, match.Groups["name"].Value);
    //                    mesh.MeshMaterialList = (XFileMeshMaterialListObject)childObject;
    //                    break;
    //                case XFileDeclDataObject.TemplateName:
    //                    childObject = ParseDeclDataObject(match.Groups["data"].Value, match.Groups["name"].Value);
    //                    mesh.DeclData = (XFileDeclDataObject)childObject;
    //                    break;
    //                case XFileSkinWeightsObject.TemplateName:
    //                    childObject = ParseSkinWeightsObject(match.Groups["data"].Value, match.Groups["name"].Value);
    //                    mesh.SkinWeightsCollection.Add((XFileSkinWeightsObject)childObject);
    //                    break;
    //            }
    //        }

    //        return mesh;
    //    }

    //    private XFileSkinWeightsObject ParseSkinWeightsObject(string skinWeightsContent, string name)
    //    {
    //        XFileSkinWeightsObject skinWeights = new XFileSkinWeightsObject();
    //        skinWeights.Name = name;
    //        string boneReferencePattern = @"""(?<bone>[^""]+)""";
    //        string valuesPattern = @"(?![^\{]+\{)\d+(?:\.\d+)?";
    //        string childLessSkinWeightsContent = Regex.Replace(skinWeightsContent, boneReferencePattern, string.Empty);
    //        skinWeights.TransformNodeName = Regex.Match(skinWeightsContent, boneReferencePattern).Groups["bone"].Value;
    //        MatchCollection matches = Regex.Matches(childLessSkinWeightsContent, valuesPattern);
    //        skinWeights.NWeights = int.Parse(matches[0].Value);
    //        skinWeights.VertexIndices = new int[skinWeights.NWeights];
    //        int vertexIndicesOffset = 1;
    //        for (int i = 0; i < skinWeights.NWeights; i++)
    //            skinWeights.VertexIndices[i] = int.Parse(matches[i + vertexIndicesOffset].Value);
    //        int weightsOffset = vertexIndicesOffset + skinWeights.NWeights;
    //        skinWeights.Weights = new float[skinWeights.NWeights];
    //        for (int i = 0; i < skinWeights.NWeights; i++)
    //            skinWeights.Weights[i] = float.Parse(matches[i + weightsOffset].Value);
    //        int matrixOffset = weightsOffset + skinWeights.NWeights;
    //        XFileMatrix matrix = new XFileMatrix();
    //        matrix.m = new float[16];
    //        for (int i = 0; i < 16; i++)
    //            matrix.m[i] = float.Parse(matches[i + matrixOffset].Value);
    //        skinWeights.MatrixOffset = matrix;
    //        return skinWeights;
    //    }

    //    private XFileDeclDataObject ParseDeclDataObject(string declDataContent, string name)
    //    {
    //        const int nComponentsPerVertexElement = 4;
    //        XFileDeclDataObject declData = new XFileDeclDataObject();
    //        declData.Name = name;
    //        string valuesPattern = @"(?![^\{]+\{)\d+(?:\.\d+)?";
    //        MatchCollection matches = Regex.Matches(declDataContent, valuesPattern);
    //        declData.NVertexElements = int.Parse(matches[0].Value);
    //        int vertexElementsOffset = 1;
    //        declData.VertexElements = new XFileVertexElement[declData.NVertexElements];            
    //        int nTotalVertexElementComponents = declData.NVertexElements * nComponentsPerVertexElement;
    //        for (int i = 0; i < declData.NVertexElements; i++)
    //        {
    //            int j = i * nComponentsPerVertexElement + vertexElementsOffset;
    //            declData.VertexElements[i] = new XFileVertexElement()
    //            {
    //                Type = (VertexElementType)int.Parse(matches[j].Value),
    //                Method = (VertexElementMethod)int.Parse(matches[j + 1].Value),
    //                Usage = (VertexElementUsage)int.Parse(matches[j + 2].Value),
    //                UsageIndex = (VertexElementUsage)int.Parse(matches[j + 3].Value)
    //            };
    //        }
    //        declData.NData = int.Parse(matches[vertexElementsOffset + nTotalVertexElementComponents].Value);
    //        int dataOffset = vertexElementsOffset + nTotalVertexElementComponents + 1;
    //        declData.Data = new long[declData.NData];
    //        for( int i = 0; i < declData.NData; i++)
    //        {
    //            declData.Data[i] = long.Parse(matches[i + dataOffset].Value);
    //        }
    //        return declData;
    //    }

    //    private XFileMeshMaterialListObject ParseMeshMaterialListObject(string meshMaterialListContent, string name)
    //    {
    //        XFileMeshMaterialListObject meshMaterialList = new XFileMeshMaterialListObject();
    //        meshMaterialList.Name = name;
    //        string materialReferencesPattern = @"(?<! MeshMaterialList\s*){\s*(?<material>[^\s]*\s*)}";
    //        string inlineMaterialsPattern = @"(?<!MeshMaterialList\s*)Material\s*{\s*(?<material>[^}]*\s*)}";
    //        string valuesPattern = @"(?![^\{]+\{)\d+(?:\.\d+)?";
    //        string childLessContent = Regex.Replace(Regex.Replace(meshMaterialListContent, materialReferencesPattern, string.Empty), inlineMaterialsPattern, string.Empty);
    //        MatchCollection matches = Regex.Matches(childLessContent, valuesPattern);
    //        meshMaterialList.NMaterials = int.Parse(matches[0].Value);
    //        meshMaterialList.NFaceIndices = int.Parse(matches[1].Value);
    //        int faceIndicesOffset = 2;
    //        meshMaterialList.FaceIndices = new int[meshMaterialList.NFaceIndices];
    //        for( int i = 0; i < meshMaterialList.NFaceIndices; i++ )
    //        {
    //            meshMaterialList.FaceIndices[i] = int.Parse(matches[faceIndicesOffset + i].Value);
    //        }
    //        if(meshMaterialList.NMaterials > 0)
    //        {                
    //            MatchCollection collection = Regex.Matches(meshMaterialListContent, materialReferencesPattern);
    //            //NOTE: We're assuming that either all child materials are references or that all child materials
    //            //are inline.
    //            if (collection.Count != 0)
    //            {
    //                meshMaterialList.MaterialRefs = new string[meshMaterialList.NMaterials];
    //                for (int i = 0; i < meshMaterialList.NMaterials; i++)
    //                    meshMaterialList.MaterialRefs[i] = collection[i].Groups["material"].Value;
    //            }
    //            else
    //            {
    //                collection = Regex.Matches(meshMaterialListContent, inlineMaterialsPattern);
    //                meshMaterialList.Materials = new XFileMaterialObject[meshMaterialList.NMaterials];
    //                for (int i = 0; i < meshMaterialList.NMaterials; i++)
    //                    meshMaterialList.Materials[i] = ParseMaterialObject(collection[i].Groups["material"].Value, null);
    //            }
    //        }
    //        return meshMaterialList;
    //    }

    //    private XFileMaterialObject ParseMaterialObject(string materialContent, string name)
    //    {
    //        XFileMaterialObject material = new XFileMaterialObject();
    //        material.Name = name;
    //        string valuesPattern = @"(?![^\{]+\{)\d+(?:\.\d+)?"; //@"(\d+.\d+)\s*";   
    //        string textureFileNameExpression = @"TextureFilename\s*{\s*""(?<texturename>[^""]+)""\s*;\s*}";
    //        string childLessMaterialContent = Regex.Replace(materialContent, textureFileNameExpression, string.Empty);
    //        MatchCollection matches = Regex.Matches(childLessMaterialContent, valuesPattern);
    //        material.FaceColor = ParseColorRGBA(string.Format("{0};{1};{2};{3};", matches[0].Groups[0].Value, matches[1].Groups[0].Value, matches[2].Groups[0].Value, matches[3].Groups[0].Value));
    //        material.Power = float.Parse(matches[4].Groups[0].Value);
    //        material.SpecularColor = ParseColorRGB(string.Format("{0};{1};{2};", matches[5].Groups[0].Value, matches[6].Groups[0].Value, matches[7].Groups[0].Value));
    //        material.EmissiveColor = ParseColorRGB(string.Format("{0};{1};{2};", matches[8].Groups[0].Value, matches[9].Groups[0].Value, matches[10].Groups[0].Value));
            
    //        Match match = Regex.Match(materialContent, textureFileNameExpression);
    //        if( match.Success)            
    //            material.TextureFilename = match.Groups["TextureFilename"].Value;

    //        return material;
    //    }

    //    private XFileMatrix ParseFrameTransformMatrix(string matrixContent, string name)
    //    {
    //        string valuesPattern = @"(?![^\{]+\{)\d+(?:\.\d+)?";
    //        MatchCollection matches = Regex.Matches(matrixContent, valuesPattern);
    //        XFileMatrix matrix = new XFileMatrix();
    //        matrix.m = new float[16];
    //        for (int i = 0; i < matrix.m.Length; i++)
    //            matrix.m[i] = float.Parse(matches[i].Value);
    //        return matrix;
    //    }

    //    private XFileColorRGBA ParseColorRGBA(string rgbaContent)
    //    {          
    //        string valuesPattern = @"(?<value>(?![^\{]+\{)\d+(?:\.\d+)?);";
    //        MatchCollection collection = Regex.Matches(rgbaContent, valuesPattern);
    //        return new XFileColorRGBA()
    //        {
    //            Red = float.Parse(collection[0].Groups["value"].Value),
    //            Green = float.Parse(collection[1].Groups["value"].Value),
    //            Blue = float.Parse(collection[2].Groups["value"].Value),
    //            Alpha = float.Parse(collection[3].Groups["value"].Value)
    //        };
    //    }

    //    private XFileColorRGB ParseColorRGB(string rgbContent)
    //    {            
    //        string valuesPattern = @"(?<value>(?![^\{]+\{)\d+(?:\.\d+)?);";
    //        MatchCollection collection = Regex.Matches(rgbContent, valuesPattern);
    //        return new XFileColorRGB()
    //        {
    //            Red = float.Parse(collection[0].Groups["value"].Value),
    //            Green = float.Parse(collection[1].Groups["value"].Value),
    //            Blue = float.Parse(collection[2].Groups["value"].Value)               
    //        };
    //    }

    //    private string ReadHeader(string textFileContent)
    //    {
    //        string result = null;
    //        Match match = Regex.Match(textFileContent, @"xof");         
    //        if (match.Index >= 0)
    //            result = textFileContent.Substring(match.Index, 16);
    //        return result;
    //    }

    //    private XFileHeader ParseHeader(string headerContent)
    //    {
    //        XFileHeader header = new XFileHeader();
    //        StringReader reader = new StringReader(headerContent);

    //        char[] magicNumber = new char[4];
    //        reader.Read(magicNumber, 0, magicNumber.Length);
    //        header.MagicNumber = new string(magicNumber);

    //        char[] majorNumber = new char[2];
    //        reader.Read(majorNumber, 0, majorNumber.Length);
    //        header.MajorNumber = short.Parse(new string(majorNumber));

    //        char[] minorNumber = new char[2];
    //        reader.Read(minorNumber, 0, minorNumber.Length);
    //        header.MinorNumber = short.Parse(new string(minorNumber));

    //        char[] formatType = new char[4];
    //        reader.Read(formatType, 0, formatType.Length);
    //        header.FormatType = new string(formatType);

    //        char[] floatSize = new char[4];
    //        reader.Read(floatSize, 0, floatSize.Length);
    //        header.FloatSize = int.Parse(new string(floatSize));

    //        return header;
    //    }
    //}

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
        public int TicksPerSecond { get; set; }
    }

    public class XFileMaterialObject : XFileDataObject
    {
        public const string TemplateName = "Material";
        public XFileColorRGBA FaceColor { get; set; }
        public float Power { get; set; }
        public XFileColorRGB SpecularColor { get; set; }
        public XFileColorRGB EmissiveColor { get; set; }
        public string TextureFilename { get; set; }
    }

    public class XFileFrameObject : XFileDataObject
    {
        public const string TemplateName = "Frame";
        private List<XFileMeshObject> _meshes = new List<XFileMeshObject>();
        private List<XFileFrameObject> _childFrames = new List<XFileFrameObject>();
        //private List<XFileAnimationObject> _animations = new List<XFileAnimationObject>();
        //private List<XFileAnimationSetObject> _animationSets = new List<XFileAnimationSetObject>();

        public XFileMatrix FrameTransformMatrix { get; set; }        
        public List<XFileMeshObject> Meshes { get { return _meshes; } }
        public List<XFileFrameObject> ChildFrames { get { return _childFrames; } }
    //    public List<XFileAnimationObject> Animations { get { return _animations; } }
    //    public List<XFileAnimationSetObject> AnimationSets { get { return _animationSets; } }
    }

    public class XFileMeshObject : XFileDataObject
    {
        public const string TemplateName = "Mesh";
        private List<XFileSkinWeightsObject> _skinWeightsCollection = new List<XFileSkinWeightsObject>();
        public int NVertices { get; set; }
        public XFileVector[] Vertices { get; set; }
        public int NFaces { get; set; }
        public XFileMeshFaceObject[] Faces { get; set; }
        public XFileMeshMaterialListObject MeshMaterialList { get; set; }
        public XFileDeclDataObject DeclData { get; set; }
        public List<XFileSkinWeightsObject> SkinWeightsCollection { get { return _skinWeightsCollection; } }
    }

    public class XFileMeshFaceObject : XFileDataObject
    {
        public int NFaceVertexIndices { get; set; }
        public int[] FaceVertexIndices { get; set; }
    }

    public class XFileMeshMaterialListObject : XFileDataObject
    {
        public const string TemplateName = "MeshMaterialList";
        public int NMaterials { get; set; }
        public int NFaceIndices { get; set; }
        public int[] FaceIndices { get; set; }
        public string[] MaterialRefs { get; set; }
        public XFileMaterialObject[] Materials { get; set; }
    }

    public class XFileDeclDataObject : XFileDataObject
    {
        public const string TemplateName = "DeclData";
        public int NVertexElements { get; set; }
        public XFileVertexElement[] VertexElements { get; set; }
        public int NData { get; set; }
        public long[] Data { get; set; }
        public T[] GetVertexDataStream<T>(int dataStreamIndex)
        {
            //NOTE: While we make ensure the dataStreamIndex is in range,
            //we, implicitly, ensure that NVertexElements is not equalt to 0.
            if (dataStreamIndex >= NVertexElements || dataStreamIndex < 0)
                throw new ArgumentOutOfRangeException("Stream not available.");    
         
            T[] results = new T[NData / NVertexElements];
            for (int i = 0; i < results.Length; i++)
            {
                int j = i * NVertexElements + dataStreamIndex;
                results[i] = (T)Convert.ChangeType(Data[j], typeof(T));
            }

            return results;
        }
        public T[] GetVertexDataStream<T>(VertexElementUsage usage, int usageInstanceIndex)
        {
            int usageInstancesFound = 0;
            for (int i = 0; i < NVertexElements; i++)
            {
                if (VertexElements[i].Usage == usage)
                {
                    if (usageInstanceIndex == usageInstancesFound)
                        return GetVertexDataStream<T>(i);
                    else
                        usageInstancesFound++;
                }
            }
            return null;
        }
    }

    public class XFileSkinWeightsObject : XFileDataObject
    {
        public const string TemplateName = "SkinWeights";
        public string TransformNodeName { get; set; }
        public int NWeights { get; set; }
        public int[] VertexIndices { get; set; }
        public float[] Weights { get; set; }
        public XFileMatrix MatrixOffset { get; set; }
    }

    public class XFileAnimationObject : XFileDataObject
    {
        public const string TemplateName = "Animation";
        private List<XFileAnimationKeyObject> _keys = new List<XFileAnimationKeyObject>();
        public string FrameRef { get; set; }
        public XFileAnimationOptions? Options { get; set; }
        public List<XFileAnimationKeyObject> Keys { get { return _keys; } }
        //public KeyType AnimationType { get { return _keys.Count > 0 ? _keys[0].KeyType : KeyType.Unknown; } }
    }

    public class XFileAnimationSetObject : XFileDataObject
    {
        public const string TemplateName = "AnimationSet";
        private List<XFileAnimationObject> _animations = new List<XFileAnimationObject>();
        public List<XFileAnimationObject> Animations { get { return _animations; } }        
    }

    public class XFileAnimationKeyObject : XFileDataObject
    {
        public const string TemplateName = "AnimationKey";
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
        public VertexElementUsage UsageIndex { get; set; }
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
}
