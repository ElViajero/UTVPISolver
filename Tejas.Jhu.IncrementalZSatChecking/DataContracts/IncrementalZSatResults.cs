using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using QuickGraph;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;

namespace Tejas.Jhu.IncrementalZSatChecking.DataContracts
{
    public class IncrementalZSatResults
    {
        public BidirectionalGraph<VertexProperties,TaggedEdge<VertexProperties, EdgeProperties>> ConstraintGraph { get; set; }
        public IDictionary<VertexProperties, int> SourceRelevantShortestPathsList { get; set; }
        public IDictionary<VertexProperties, int> TargetRelevantShortestPathsList { get; set; }
        public TaggedEdge<VertexProperties, EdgeProperties> ConstraintEdge { get; set; }
        public bool IsConsistencyCheckSuccessful { get; set; }
        public VertexProperties FailedSource { get; set; }
        public VertexProperties FailedTarget { get; set; }

        public IncrementalZSatResults(
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph,
            IDictionary<VertexProperties, int> sourceRelevantShortestPathsList,
            IDictionary<VertexProperties, int> targetRelevantShortestPathsList,
            TaggedEdge<VertexProperties, EdgeProperties> constraintEdge,VertexProperties failedSource, VertexProperties failedTarget,
            bool isConsistencyCheckSuccessful)
        {
            ConstraintGraph = constraintGraph;
            SourceRelevantShortestPathsList = sourceRelevantShortestPathsList;
            TargetRelevantShortestPathsList = targetRelevantShortestPathsList;
            ConstraintEdge = constraintEdge;
            FailedSource = failedSource;
            FailedTarget = failedTarget;
            IsConsistencyCheckSuccessful = isConsistencyCheckSuccessful;

        }


    }
}