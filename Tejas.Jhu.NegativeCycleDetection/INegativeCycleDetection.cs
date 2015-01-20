using QuickGraph;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.NegativeCycleDetection.DataContracts;

namespace Tejas.Jhu.NegativeCycleDetection
{
    public interface INegativeCycleDetection
    {
        NegativeCycleDetectionResult DetectNegativeCycles(BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph);
    }
}