using System.Collections.Generic;
using QuickGraph;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;


namespace Tejas.Jhu.NegativeCycleDetection.DataContracts
{
    public class NegativeCycleDetectionResult
    {
        public bool IsNegativeCycleDetected;
        public BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ScannedGraph { get; set; }
        public List<VertexProperties> FailedConstraints { get; set; }


        public NegativeCycleDetectionResult()
            : this(false, new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>(), new List<VertexProperties>())
        {}

        public NegativeCycleDetectionResult(bool isNegativeCycleDetected, BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> scannedGraph, List<VertexProperties> failedConstraints)
        {
            IsNegativeCycleDetected = isNegativeCycleDetected;
            ScannedGraph = scannedGraph;
            FailedConstraints = failedConstraints;
        }
    }
}