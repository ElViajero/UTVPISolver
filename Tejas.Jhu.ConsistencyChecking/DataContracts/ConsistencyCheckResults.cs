using System;
using System.Collections.Generic;
using QuickGraph;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;

namespace Tejas.Jhu.ConsistencyChecking.DataContracts
{
    public class ConsistencyCheckResults : IEquatable<ConsistencyCheckResults>
    {
        public bool IsConsistencyCheckSuccessful { get; private set; }
        public BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> Graph { get; private set; }
        public List<string> FailedConstraints { get; private set; }


        public IDictionary<VertexProperties, int> SourceRelevantShortestPathsList { get; set; }
        public IDictionary<VertexProperties, int> TargetRelevantShortestPathsList { get; set; }
        public  TaggedEdge<VertexProperties, EdgeProperties> ConstraintEdge { get; set; }

        public ConsistencyCheckResults(bool isConsistencyCheckSuccessful, BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph, List<string> failedConstraints)
        {
            IsConsistencyCheckSuccessful = isConsistencyCheckSuccessful;
            Graph = graph;
            FailedConstraints = failedConstraints;
        }

        public ConsistencyCheckResults(bool isConsistencyCheckSuccessful, BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph,
            List<string> failedConstraints, IDictionary<VertexProperties, int> sourceRelevantShortestPathsList, IDictionary<VertexProperties, int> targetRelevantShortestPathsList,
             TaggedEdge<VertexProperties, EdgeProperties> constraintEdge)
        {
            IsConsistencyCheckSuccessful = isConsistencyCheckSuccessful;
            Graph = graph;
            FailedConstraints = failedConstraints;
            SourceRelevantShortestPathsList = sourceRelevantShortestPathsList;
            TargetRelevantShortestPathsList = targetRelevantShortestPathsList;
            ConstraintEdge = constraintEdge;
        }


        public override string ToString()
        {
            return string.Format("IsConsistencyCheckSuccessful: {0}, Graph: {1}, FailedConstraints: {2}", IsConsistencyCheckSuccessful, Graph, FailedConstraints);
        }

        public bool Equals(ConsistencyCheckResults other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsConsistencyCheckSuccessful.Equals(other.IsConsistencyCheckSuccessful) && Equals(Graph, other.Graph) && Equals(FailedConstraints, other.FailedConstraints);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ConsistencyCheckResults) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = IsConsistencyCheckSuccessful.GetHashCode();
                hashCode = (hashCode*397) ^ (Graph != null ? Graph.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (FailedConstraints != null ? FailedConstraints.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}