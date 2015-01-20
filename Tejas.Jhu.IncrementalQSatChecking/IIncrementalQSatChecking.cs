using System.Security.Cryptography.X509Certificates;
using QuickGraph;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.IncrementalQSatChecking.DataContracts;

namespace Tejas.Jhu.IncrementalQSatChecking
{
    public interface IIncrementalQSatChecking
    {
        IncrementalQSatCheckingResults CheckQSatisfiability(
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph,
            TaggedEdge<VertexProperties, EdgeProperties> constraintEdge);

    }
}