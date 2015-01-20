namespace Tejas.Jhu.GraphUtilities.GraphBusinessObjects
{
    public class VertexDistanceLabel
    {
        public string VertexName;
        public int DistanceFromSource;

        public VertexDistanceLabel(string vertexName, int distanceFromSource)
        {
            VertexName = vertexName;
            DistanceFromSource = distanceFromSource;
        }
    }
}