using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickGraph;
using Tejas.Jhu.GraphUtilities;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.NegativeCycleDetection;

namespace Tejas.Jhu.ImplicationChecking
{
    public class IncrementalImplicationChecker : ImplicationChecker
    {

        #region private class properties

        private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstraintGraph {
            get; set; }

        private IGraphTraversalAlgorithms GraphTraversalAlgorithmsObject { get; set; }
        private IGraphHelper GraphHelperObject { get; set; }
        private IList<string> ImpliedConstraintList { get; set; }

        #endregion

        #region class constructor

        public IncrementalImplicationChecker(IGraphTraversalAlgorithms graphTraversalAlgorithmsObject,
            IGraphHelper graphHelperObject)
        {
            GraphTraversalAlgorithmsObject = graphTraversalAlgorithmsObject;
            GraphHelperObject = graphHelperObject;
            ImpliedConstraintList = new List<string>();
        }

        #endregion


        public override IList<string> CheckImplication(
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph,
            IDictionary<VertexProperties, int> sourceRelevantShortestPathList,
            IDictionary<VertexProperties, int> targetRelevantShortestPaths,
            TaggedEdge<VertexProperties,EdgeProperties> addedConstraintEdge,
            IList<string> constraintsList)
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
                

                //check if shortest paths are known for either of the starting vertices

                //if (edgeList.Where(currentEdge => ShortesPathsDictionary.ContainsKey(currentEdge.Source)).Any(currentEdge => ShortesPathsDictionary[currentEdge.Source].ContainsKey(currentEdge.Target)))                                                                                                                             
                foreach (TaggedEdge<VertexProperties, EdgeProperties> currentEdge in edgeList)
                {
                    sourceVertex = (from vertex in ConstraintGraph.Vertices
                        where vertex.Equals(currentEdge.Source)
                        select vertex).FirstOrDefault();
                    //we need the correct distance labels to perform the check.
                    targetVertex = (from vertex in ConstraintGraph.Vertices
                        where vertex.Equals(currentEdge.Target)
                        select vertex).FirstOrDefault();
                    
                    if (sourceVertex == null || targetVertex == null ||
                        !(sourceRelevantShortestPathList.ContainsKey(targetVertex) && targetRelevantShortestPaths.ContainsKey(sourceVertex)))
                        continue;

                    if (targetVertex.DistanceLabel - sourceVertex.DistanceLabel +
                        (sourceRelevantShortestPathList[targetVertex] + targetRelevantShortestPaths[sourceVertex] -
                         addedConstraintEdge.Tag.Slack) <= currentEdge.Tag.Weight)
                    {
                        lock (ImpliedConstraintList)
                        {
                            ImpliedConstraintList.Add(currentConstraint);
                        }
                    }
                    
                }

            });
            return ImpliedConstraintList;
        }
    }
}