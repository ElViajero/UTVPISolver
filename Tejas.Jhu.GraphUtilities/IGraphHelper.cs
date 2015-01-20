using System.Collections.Generic;
using QuickGraph;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;

namespace Tejas.Jhu.GraphUtilities
{
    public interface IGraphHelper
    {
        /// <summary>
        /// The method converts a list of constraints (given as input in the form of a list) to a bi-directional graph.
        /// The graph can then be used for negative cycle detection etc...
        /// </summary>
        /// <param name="constraintsList">List of constraints</param>
        /// <returns>A bi-directional graph representation of the constraints</returns>
        BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConvertConstraintsToGraph(IList<string> constraintsList);

        List<string> RetrieveConstraintsFromCycle(int startIndex, int endIndex,IList<VertexProperties> vertexList,
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph ) ;

        List<TaggedEdge<VertexProperties, EdgeProperties>> ConvertConstriantsToEdges(IList<string> constraintList);

    }
}