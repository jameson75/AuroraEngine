namespace Aurora.Sample.Editor.Scene
{
    public class ReferenceGrid
    {
        public ReferenceGrid(float xLength, int xSectors, int xSegments, float yLength, int ySectors, int ySegments)
        {
            XLength = xLength;
            XSectors = xSectors;
            XSegmemts = xSegments;
            YLength = yLength;
            YSectors = ySectors;
            YSegments = ySegments;
        }

        public float XLength { get; set; }              
        
        public int XSectors { get; set; }
        
        public int XSegmemts { get; set; }
        
        public float YLength { get; set; }
        
        public int YSegments { get; set; }
        
        public int YSectors { get; set; }
    }
}
