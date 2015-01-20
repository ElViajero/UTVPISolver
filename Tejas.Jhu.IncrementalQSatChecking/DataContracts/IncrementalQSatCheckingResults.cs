using System.Collections.Generic;
using QuickGraph;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;

namespace Tejas.Jhu.IncrementalQSatChecking.DataContracts
{
    public class IncrementalQSatCheckingResults
    {
        public BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstraintGraph;
        public IDictionary<VertexProperties, int> ChangedPotentialValues;
        public IList<VertexProperties> FailedConstraintVerticesList;
        public TaggedEdge<VertexProperties, EdgeProperties> ConstraintEdge; 
        public bool IsConsistencyCheckSuccessful;

        public IncrementalQSatCheckingResults(
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph,
            IDictionary<VertexProperties, int> changedPotentialValues,
            IList<VertexProperties> failedConstraintVerticesList,TaggedEdge<VertexProperties,EdgeProperties> constraintEdge,bool isConsistencyCheckSuccessful)
        {
            ConstraintGraph = constraintGraph;
            ChangedPotentialValues = changedPotentialValues;
            FailedConstraintVerticesList = failedConstraintVerticesList;
            ConstraintEdge = constraintEdge;
            IsConsistencyCheckSuccessful = isConsistencyCheckSuccessful;
        }
    }
}