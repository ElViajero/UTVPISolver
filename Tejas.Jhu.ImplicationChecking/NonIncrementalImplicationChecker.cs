using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using QuickGraph;
using QuickGraph.Algorithms;
using Tejas.Jhu.GraphUtilities;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.NegativeCycleDetection;

namespace Tejas.Jhu.ImplicationChecking
{
    public class NonIncrementalImplicationChecker:ImplicationChecker
    {
        #region private class properties

        private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstraintGraph {get; set; }
        private IDictionary<VertexProperties, IDictionary<VertexProperties, int>> ShortesPathsDictionary { get; set; }
        private IGraphTraversalAlgorithms GraphTraversalAlgorithmsObject { get; set; }
        private IGraphHelper GraphHelperObject { get; set; }
        private  IList<string> ImpliedConstraintList { get; set; }
        #endregion

        #region constructor

        public NonIncrementalImplicationChecker(GraphTraversalAlgorithms graphTraversalAlgorithmsObject, GraphHelper graphHelperObject )
        {
            GraphTraversalAlgorithmsObject = graphTraversalAlgorithmsObject;
            GraphHelperObject = graphHelperObject;
            ImpliedConstraintList= new List<string>();
            ShortesPathsDictionary = new Dictionary<VertexProperties, IDictionary<VertexProperties, int>>();
        }

        #endregion

        public override IList<string> CheckImplication(BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph, List<string> constraintsList)
        {

            ConstraintGraph = constraintGraph;            
            Parallel.ForEach(constraintsList, currentConstraint =>
            {
                List<string> currentConstraintList = new List<string>();
                VertexProperties sourceVertex;
                VertexProperties targetVertex;
                currentConstraintList.Add(currentConstraint);
                List<TaggedEdge<VertexProperties, EdgeProperties>> edgeList =
                    GraphHelperObject.ConvertConstriantsToEdges(currentConstraintList);
                
                bool shortestPathDictionaryContainsVertex = false;

                //check if shortest paths are known for either of the starting vertices

                //if (edgeList.Where(currentEdge => ShortesPathsDictionary.ContainsKey(currentEdge.Source)).Any(currentEdge => ShortesPathsDictionary[currentEdge.Source].ContainsKey(currentEdge.Target)))                                                                                                                             
                foreach (TaggedEdge<VertexProperties, EdgeProperties> currentEdge in edgeList.Where(currentEdge => ShortesPathsDictionary.ContainsKey(currentEdge.Source)))
                {
                    sourceVertex = (from vertex in constraintGraph.Vertices
                                        where vertex.Equals(currentEdge.Source)
                                        select vertex).FirstOrDefault();
                    //we need the correct distance labels to perform the check.
                    targetVertex = (from vertex in constraintGraph.Vertices
                                        where vertex.Equals(edgeList[0].Target)
                                        select vertex).FirstOrDefault();
                    if (sourceVertex == null || targetVertex == null)
                        continue;
                    //(currentEdge.Target.DistanceLabel - currentEdge.Source.DistanceLabel + ShortesPathsDictionary[currentEdge.Source][currentEdge.Target] )<= currentEdge.Tag.Weight)
                    if (targetVertex.DistanceLabel - sourceVertex.DistanceLabel +
                        ShortesPathsDictionary[sourceVertex][targetVertex] > currentEdge.Tag.Weight)
                        break;
                    shortestPathDictionaryContainsVertex = true;
                    lock (ImpliedConstraintList)
                    {
                        ImpliedConstraintList.Add(currentConstraint);
                    }
                }

                if (shortestPathDictionaryContainsVertex) return;
                
                //check the overridden equals method in vertexProperties to know why
                sourceVertex=(from vertex in constraintGraph.Vertices
                where vertex.Equals(edgeList[0].Source)
                select vertex).FirstOrDefault();
                //we need the correct distance labels to perform the check.
                targetVertex = (from vertex in constraintGraph.Vertices
                                    where vertex.Equals(edgeList[0].Target)
                                    select vertex).FirstOrDefault();

                if(targetVertex==null || sourceVertex == null) return;
                

                lock (ShortesPathsDictionary)
                {
                    if(!ShortesPathsDictionary.ContainsKey(sourceVertex))
                    ShortesPathsDictionary.Add(sourceVertex,
                        GraphTraversalAlgorithmsObject.SingleSourceNonNegativeShortestPath(sourceVertex,
                            ConstraintGraph));
                }

                if (!ShortesPathsDictionary[sourceVertex].ContainsKey(targetVertex) ||
                    (targetVertex.DistanceLabel- sourceVertex.DistanceLabel+ ShortesPathsDictionary[sourceVertex][targetVertex]) > edgeList[0].Tag.Weight) return;
                lock (ImpliedConstraintList)
                {
                    ImpliedConstraintList.Add(currentConstraint);
                }
            });

            return ImpliedConstraintList;
        }
    }
    
}