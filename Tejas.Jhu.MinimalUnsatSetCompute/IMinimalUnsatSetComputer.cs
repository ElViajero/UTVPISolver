using System.Collections.Generic;
using QuickGraph;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;

namespace Tejas.Jhu.MinimalUnsatSetCompute
{
    public interface IMinimalUnsatSetComputer
    {
         IList<string> ComputeMinimalUnsatSet(
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph,
            VertexProperties sourceVertex,
            VertexProperties targetVertex, IList<string> constraintsList);
    }
}