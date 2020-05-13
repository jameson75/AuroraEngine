using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Ionic.Zlib;

namespace CipherPark.AngelJacket.Core.Content
{
    public class TMXDocument
    {        
        public TMXMap Map { get; set; }

        private TMXDocument() { }

        public static TMXDocument Load(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TMXMap));
            using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                TMXDocument document = new TMXDocument()
                {
                    Map = (TMXMap)serializer.Deserialize(stream),
                };
                document.DecodeLayers();
                return document;                
            }
        }

        public static TMXDocument Parse(string content)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TMXMap));
            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                TMXDocument document = new TMXDocument
                {
                    Map = (TMXMap)serializer.Deserialize(stream),
                };
                document.DecodeLayers();
                return document;
            }
        }

        private void DecodeLayers()
        {            
            foreach(var layer in this.Map?.Layers)
            {
                if(layer.Data != null)
                {
                    int[] decodedUncompressedData = null;
                    if (layer.Data.Encoding == "base64")
                    {                        
                        byte[] decodedData = Convert.FromBase64String(layer.Data.InnerText);
                        if (layer.Data.Compression != null)
                        {
                            if (layer.Data.Compression == "gzip")
                            {
                                using (var memStream = new MemoryStream(decodedData))
                                using (var gZipStream = new System.IO.Compression.GZipStream(memStream, System.IO.Compression.CompressionMode.Decompress))
                                {
                                    decodedUncompressedData = gZipStream.ReadIntArray();                                                                        
                                }
                            }
                            else if (layer.Data.Compression == "zlib")
                            {
                                using (var memStream = new MemoryStream(decodedData))
                                using (var zLibStream = new ZlibStream(memStream, CompressionMode.Decompress))
                                {
                                    decodedUncompressedData = zLibStream.ReadIntArray();
                                }
                            }
                            else
                                throw new InvalidOperationException("Unrecognized compression format.");
                        }
                        else
                            decodedUncompressedData = BitConverterHelper.ToInt32Array(decodedData);
                    }
                    else if(layer.Data.Encoding == "csv")
                    {
                        decodedUncompressedData = layer.Data.InnerText.Split(',').Select(x => int.Parse(x)).ToArray();
                    }
                    else if (layer.Data.Encoding != null)
                        throw new InvalidOperationException("Unsupported encoding.");

                    layer.Data.Values = decodedUncompressedData;             
                }
            }
        }
    }

    [XmlRoot("map")]
    public class TMXMap
    {
        [XmlAttribute("version")]
        public string Version { get; set; }
        [XmlAttribute("tiledversion")]
        public string TiledVersion { get; set; }
        [XmlAttribute("orentation")]
        public string Orientation { get; set; }
        [XmlAttribute("renderorder")]
        public string RenderOrder { get; set; }
        [XmlAttribute("width")]
        public int Width { get; set; }
        [XmlAttribute("height")]
        public int Height { get; set; }
        [XmlAttribute("tilewidth")]
        public int TileWidth { get; set; }
        [XmlAttribute("tileheight")]
        public int TileHeight { get; set; }
        [XmlAttribute("hexsidelength")]
        public int HexSideLength { get; set; }
        [XmlAttribute("staggeraxis")]
        public string StaggerAxis { get; set; }
        [XmlAttribute("staggerindex")]
        public string StaggerIndex { get; set; }
        [XmlAttribute("backgroundcolor")]
        public string BackgroundColor { get; set; }
        [XmlAttribute("nextlayerid")]
        public string NextLayerId { get; set; }
        [XmlAttribute("nextobjectid")]
        public string NextObjectId { get; set; }

        [XmlElement("properties")]
        public TMXProperties Properties { get; set; }
        [XmlElement("tileset")]
        public TMXTileSet[] TileSets { get; set; }
        [XmlElement("layer")]
        public TMXLayer[] Layers { get; set; }
        [XmlElement("objectgroup")]
        public TMXObjectGroup ObjectGroup { get; set; }
        [XmlElement("imagelayer")]
        public TMXImageLayer[] ImageLayers { get; set; }
        [XmlElement("group")]
        public TMXGroup[] Groups { get; set; }
    }

    public class TMXTileSet
    {
        [XmlAttribute("firstgid")]
        public string FirstGID { get; set; }
        [XmlAttribute("source")]
        public string Source { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("tilewidth")]
        public int TileWidth { get; set; }
        [XmlAttribute("tileheight")]
        public int TileHeight { get; set; }
        [XmlAttribute("spacing")]
        public int Spacing { get; set; }
        [XmlAttribute("margin")]
        public int Margin { get; set; }
        [XmlAttribute("tilecount")]
        public int TileCount { get; set; }
        [XmlAttribute("columns")]
        public int Columns { get; set; }

        [XmlElement("tileoffset")]
        public TMXTileOffset TileOffset { get; set; }
        [XmlElement("grid")]
        public TMXGrid Grid { get; set; }
        [XmlElement("properties")]
        public TMXProperties Properties { get; set; }
        [XmlElement("image")]
        public TMXImage Image { get; set; }
        [XmlElement("terraintypes")]
        public TMXTerrainTypes TerrainTypes { get; set; }
        [XmlElement("tile")]
        public TMXTile Tile { get; set; }
        [XmlElement("wangsets")]
        public TMXWangSets WangSets { get; set; }
    }

    public class TMXTileOffset
    {
        [XmlAttribute("x")]
        public int X { get; set; }
        [XmlAttribute("y")]
        public int Y { get; set; }
    }  

    public class TMXGrid
    {
        [XmlAttribute("orientation")]
        public string Orientation { get; set; }
        [XmlAttribute("width")]
        public int Width { get; set; }
        [XmlAttribute("height")]
        public int Height { get; set; }
    }

    public class TMXImage
    {
        [XmlAttribute("format")]
        public string Format { get; set; }
        [XmlAttribute("source")]
        public string Source { get; set; }
        [XmlAttribute("trans")]
        public string Trans { get; set; }
        [XmlAttribute("width")]
        public int Width { get; set; }
        [XmlAttribute("height")]
        public int Height { get; set; }
        [XmlElement("data")]
        public TMXData Data { get; set; }
    }

    public class TMXTerrainTypes
    {
        [XmlElement("terrain")]
        public TMXTerrian[] Terrain { get; set; }
    }

    public class TMXTerrian
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("tile")]
        public string Tile { get; set; }
    }

    public class TMXTile
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("type")]
        public string Type { get; set; }
        [XmlAttribute("terrain")]
        public string Terrain { get; set; }
        [XmlAttribute("probability")]
        public double Probability { get; set; }

        [XmlElement("properties")]
        public TMXProperties Properties { get; set; }
        [XmlElement("image")]
        public TMXImage Image { get; set; }
        [XmlElement("objectgroup")]
        public TMXObjectGroup ObjectGroup { get; set; }
        [XmlElement("animation")]
        public TMXAnimation Animation { get; set; }
    }

    public class TMXAnimation
    {
        [XmlElement("frame")]
        public TMXFrame[] Frames { get; set; }
    }

    public class TMXFrame
    {
        [XmlAttribute("tileid")]
        public string TileId { get; set; }
        [XmlAttribute("duration")]
        public long Duration { get; set; }
    }

    public class TMXWangSets
    {
        [XmlElement("wangset")]
        public TMXWangSet[] WangSet { get; set; }
    }

    public class TMXWangSet
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("tile")]
        public string Tile { get; set; }
    }    

    public class TMXWangCornerColor
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("color")]
        public string Color { get; set; }
        [XmlAttribute("tile")]
        public string Tile { get; set; }
        [XmlAttribute("probability")]
        public double Probability { get; set; }
    }

    public class TMXWangEdgeColor
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("color")]
        public string Color { get; set; }
        [XmlAttribute("tile")]
        public string Tile { get; set; }
        [XmlAttribute("probability")]
        public double Probability { get; set; }
    }

    public class TMXWangTile
    {
        [XmlAttribute("tileid")]
        public string TileId { get; set; }
        [XmlAttribute("wangid")]
        public string WangId { get; set; }
    }

    public class TMXLayer
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("x")]
        public int X { get; set; }
        [XmlAttribute("y")]
        public int Y { get; set; }
        [XmlAttribute("width")]
        public int Width { get; set; }
        [XmlAttribute("height")]
        public int Height { get; set; }
        [XmlAttribute("opacity")]
        public float Opacity { get; set; }
        [XmlAttribute("visible")]
        public int Visible { get; set; }
        [XmlAttribute("offsetx")]
        public int OffsetX { get; set; }
        [XmlAttribute("offsety")]
        public int OffsetY { get; set; }

        [XmlElement("properties")]
        public TMXProperties Properties { get; set; }
        [XmlElement("data")]
        public TMXData Data { get; set; }
    }

    public class TMXData
    {
        [XmlAttribute("encoding")]
        public string Encoding { get; set; }
        [XmlAttribute("compression")]
        public string Compression { get; set; }       
        [XmlText]
        public string InnerText { get; set; }
        [XmlElement("tile")]
        public TMXTile[] Tiles { get; set; }
        [XmlElement("chunk")]
        public TMXChunk Chunk { get; set; }
        [XmlIgnore]
        public int[] Values { get; set; }
    }

    public class TMXChunk
    {
        [XmlAttribute("x")]
        public int X { get; set; }
        [XmlAttribute("y")]
        public int Y { get; set; }
        [XmlAttribute("width")]
        public int Width { get; set; }
        [XmlAttribute("height")]
        public int Height { get; set; }
        public string InnerText { get; set; }
        [XmlElement("tile")]
        public TMXTile[] Tiles { get; set; }
    }   

    public class TMXSingleTile
    {
        [XmlAttribute("gid")]
        public string GID { get; set; }
    }

    public class TMXObjectGroup
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("color")]
        public string Color { get; set; }
        [XmlAttribute("x")]
        public int X { get; set; }
        [XmlAttribute("y")]
        public int Y { get; set; }
        [XmlAttribute("width")]
        public int Width { get; set; }
        [XmlAttribute("height")]
        public int Height { get; set; }
        [XmlAttribute("opacity")]
        public float Opacity { get; set; }
        [XmlAttribute("visible")]
        public int Visible { get; set; }
        [XmlAttribute("offsetx")]
        public int OffsetX { get; set; }
        [XmlAttribute("offsety")]
        public int OffsetY { get; set; }
        [XmlAttribute("draworder")]
        public string DrawOrder { get; set; }
    }

    public class TMXObject
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("type")]
        public string Type { get; set; }
        [XmlAttribute("x")]
        public int X { get; set; }
        [XmlAttribute("y")]
        public int Y { get; set; }
        [XmlAttribute("width")]
        public int Width { get; set; }
        [XmlAttribute("height")]
        public int Height { get; set; }
        [XmlAttribute("rotation")]
        public float Rotation { get; set; }
        [XmlAttribute("gid")]
        public string GID { get; set; }
        [XmlAttribute("visible")]
        public int Visible { get; set; }
        [XmlAttribute("template")]
        public string TemplateFile { get; set; }
    }

    public class TMXEllipse { }

    public class TMXPoint { }

    public class TMXPolygon
    {
        [XmlAttribute("points")]
        public string Points { get; set; }
    }

    public class TMXPolyline
    {
        [XmlAttribute("points")]
        public string Points { get; set; }
    }

    public class TMXText
    {
        [XmlAttribute("fontfamily")]
        public string FontFamily { get; set; }
        [XmlAttribute("pixelsize")]
        public int PixelSize { get; set; }
        [XmlAttribute("wrap")]
        public string Wrap { get; set; }
        [XmlAttribute("color")]
        public string Color { get; set; }
        [XmlAttribute("bold")]
        public byte Bold { get; set; }
        [XmlAttribute("italic")]
        public byte Italic { get; set; }
        [XmlAttribute("underline")]
        public byte Underline { get; set; }
        [XmlAttribute("strikeout")]
        public byte Strikeout { get; set; }
        [XmlAttribute("kerning")]
        public byte Kerning { get; set; }
        [XmlAttribute("halign")]
        public string HAlign { get; set; }
        [XmlAttribute("valign")]
        public string VAlign { get; set; }
    }

    public class TMXImageLayer
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }       
        [XmlAttribute("x")]
        public int X { get; set; }
        [XmlAttribute("y")]
        public int Y { get; set; }       
        [XmlAttribute("opacity")]
        public float Opacity { get; set; }
        [XmlAttribute("visible")]
        public int Visible { get; set; }
        [XmlAttribute("offsetx")]
        public int OffsetX { get; set; }
        [XmlAttribute("offsety")]
        public int OffsetY { get; set; }    
    }

    public class TMXGroup
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }            
        [XmlAttribute("opacity")]
        public float Opacity { get; set; }
        [XmlAttribute("visible")]
        public int Visible { get; set; }
        [XmlAttribute("offsetx")]
        public int OffsetX { get; set; }
        [XmlAttribute("offsety")]
        public int OffsetY { get; set; }
    }

    public class TMXProperties
    {
        [XmlElement("property")]
        TMXProperty[] Collection { get; set; }
    }

    public class TMXProperty
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("type")]
        public string Type { get; set; }
        [XmlAttribute("value")]
        public string Value { get; set; }
    }

    public class TMXTemplate
    {
        [XmlElement("tileset")]
        public TMXTileSet TileSet { get; set; }
        [XmlElement("object")]
        public TMXObject Object { get; set; }
    }

    public static class CompressionStreamExtensions
    {
        public static int[] ReadIntArray(this Stream stream)
        {
            int offset = 0;
            int totalCount = 0;
            const int BufferSize = 1024;
            byte[] buffer = new byte[BufferSize];           
            while (true)
            {
                int bytesRead = stream.Read(buffer, offset, BufferSize);
                if (bytesRead == 0)                
                    break;                                             
                offset += bytesRead;
                totalCount += bytesRead;
                Array.Resize(ref buffer, buffer.Length + BufferSize);
            }
            Array.Resize(ref buffer, totalCount);
            return BitConverterHelper.ToInt32Array(buffer);
        }        
    }

    public static class BitConverterHelper
    {
        public static int[] ToInt32Array(byte[] array)
        {
            int[] result = new int[array.Length / sizeof(System.Int32)];
            for (int i = 0; i < result.Length; i++)
                result[i] = BitConverter.ToInt32(array, i * sizeof(System.Int32));
            return result;
        }
    }
}
