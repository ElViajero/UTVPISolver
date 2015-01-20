using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;

namespace Tejas.Jhu.NegativeCycleDetection.DataContracts
{
    public class EarlyTerminationDjikstraQueryResult
    {
         public int Caliber { get; set; }
        public DjikstraEarlyTerminationVertexProperties SourceDjikstrasVertex { get; set; }
        public DjikstraEarlyTerminationVertexProperties TargetDjikstrasVertex { get; set; }
        public EdgeProperties Edge { get; set; }

        public EarlyTerminationDjikstraQueryResult(int caliber, DjikstraEarlyTerminationVertexProperties sourceDjikstrasVertex, DjikstraEarlyTerminationVertexProperties targetDjikstrasVertex, EdgeProperties edge)
        {
            Caliber = caliber;
            SourceDjikstrasVertex = sourceDjikstrasVertex;
            TargetDjikstrasVertex = targetDjikstrasVertex;
            Edge = edge;
        } 
    }
}