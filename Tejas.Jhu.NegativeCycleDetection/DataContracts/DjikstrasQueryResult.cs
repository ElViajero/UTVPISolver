using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;

namespace Tejas.Jhu.NegativeCycleDetection.DataContracts
{
    public class DjikstrasQueryResult
    {
        public int Caliber { get; set; }
        public DjikstrasVertexProperties SourceDjikstrasVertex { get; set; }
        public DjikstrasVertexProperties TargetDjikstrasVertex { get; set; }
        public EdgeProperties Edge { get; set; }

        public DjikstrasQueryResult(int caliber, DjikstrasVertexProperties sourceDjikstrasVertex, DjikstrasVertexProperties targetDjikstrasVertex, EdgeProperties edge)
        {
            Caliber = caliber;
            SourceDjikstrasVertex = sourceDjikstrasVertex;
            TargetDjikstrasVertex = targetDjikstrasVertex;
            Edge = edge;
        }
    }
}