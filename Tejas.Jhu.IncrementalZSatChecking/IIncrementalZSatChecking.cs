using System.Collections;
using System.Collections.Generic;
using QuickGraph;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.IncrementalZSatChecking.DataContracts;

namespace Tejas.Jhu.IncrementalZSatChecking
{
    public interface IIncrementalZSatChecking
    {
       IncrementalZSatResults CheckZSatisfiability(
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph,
            TaggedEdge<VertexProperties, EdgeProperties> constraintEdge,
            IDictionary<VertexProperties, int> changedPotentialValues);
    }
}