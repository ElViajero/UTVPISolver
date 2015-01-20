using System.Collections.Generic;
using QuickGraph;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.NegativeCycleDetection.DataContracts;

namespace Tejas.Jhu.NegativeCycleDetection
{
    public interface IGraphTraversalAlgorithms
    {
        DfsTraversalResult CustomDFSTraversal(
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph,
            IList<VertexProperties> VerticesToScan);

        IDictionary<VertexProperties, List<List<VertexProperties>>> ComputeStronglyConnectedComponents(
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph);

        IDictionary<VertexProperties, int> SingleSourceNonNegativeShortestPath(
            VertexProperties sourceVertex,
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph);
        IDictionary<VertexProperties, DjikstrasEdgeTrackObject> SingleSourceNonNegativeShortestPath_EdgeTracking(
            VertexProperties sourceVertex,
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph);
        IDictionary<VertexProperties, int> SingleSourceNonNegativeShortestPath_EarlyTermination(
            TaggedEdge<VertexProperties,EdgeProperties> constraintEdge ,
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph);

    }
}
