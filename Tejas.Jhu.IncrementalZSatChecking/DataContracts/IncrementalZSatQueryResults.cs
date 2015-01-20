using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;

namespace Tejas.Jhu.IncrementalZSatChecking.DataContracts
{
    public class IncrementalZSatQueryResults
    {
        public VertexProperties Source { get; set; }
        public VertexProperties Target { get; set; }
        public int Distance;

        public IncrementalZSatQueryResults(VertexProperties source, VertexProperties target, int distance)
        {
            Source = source;
            Target = target;
            Distance = distance;
        }
    }
}