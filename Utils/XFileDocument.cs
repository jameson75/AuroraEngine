using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace CipherPark.AngelJacket.Core.Utils
{
    public class XFileDocument
    {
        public List<XFileDataObjects> dataObjects = new List<XFileDataObjects>();
        
        public XFileHeader Header { get; private set; }
        
        public XFileDataObjects dataObjects = 
        public void Load(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            SeekForwardToHeader(reader);
            Header = ReadHeader(reader);

        }

        private void SeekForwardToHeader(StreamReader reader)
        {
            
        }

        private XFileHeader ReadHeader(StreamReader reader)
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
    }


    public class XFileHeader
    {
        public string MagicNumber { get; set; }
        public short MajorNumber { get; set; }
        public short MinorNumber { get; set; }
        public string FormatType { get; set; }
        public int FloatSize { get; set; }
    }
}
