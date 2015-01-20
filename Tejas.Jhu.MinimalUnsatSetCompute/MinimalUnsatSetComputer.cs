using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickGraph;
using Tejas.Jhu.ConsistencyChecking.DataContracts;
using Tejas.Jhu.GraphUtilities;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.NegativeCycleDetection;
using Wintellect.PowerCollections;

namespace Tejas.Jhu.MinimalUnsatSetCompute
{
    public class MinimalUnsatSetComputer
    {
        
        private IList<string> FailedConstraintList { get; set; }
        private OrderedSet<string> MinimalConstraintSet { get; set; }
        private IGraphHelper GraphHelper { get; set; }
        private IGraphTraversalAlgorithms GraphTraversalAlgorithms { get; set; }


        public MinimalUnsatSetComputer(IGraphTraversalAlgorithms graphTraversalAlgorithms, IGraphHelper graphHelper)
        {
            GraphHelper = graphHelper;
            GraphTraversalAlgorithms = graphTraversalAlgorithms;
        }


        public IList<string> ComputeMinimalUnsatSet(BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph, VertexProperties sourceVertex,
            VertexProperties targetVertex,IList<string> constraintsList )
        {
        
           
             IDictionary<VertexProperties, List<List<VertexProperties>>> stronglyConnectedComponentList = GraphTraversalAlgorithms.ComputeStronglyConnectedComponents(constraintGraph);



            var resultList = (from sccComponentListOne in stronglyConnectedComponentList.Values
                              from scc in sccComponentListOne
                              from vertexOne in scc
                              from vertexTwo in scc
                              where vertexOne.Equals(sourceVertex) &&
                                    vertexTwo.Equals(targetVertex)&&
                                    vertexOne.IsNegative != vertexTwo.IsNegative &&
                                    (vertexOne.DistanceLabel - vertexTwo.DistanceLabel) % 2 != 0
                              orderby scc.Count ascending
                              select new SccQueryResult(scc, vertexOne, vertexTwo));
           


            
            //check if Z UNSAt
            var sccQueryResults = resultList as IList<SccQueryResult> ?? resultList.ToList();
            if (sccQueryResults.Any())
            {
                
                var result = sccQueryResults[0];
                IEnumerable<SccQueryResult> finalSccList = (from scc in sccQueryResults
                    where scc.SccComponentList.Count == result.SccComponentList.Count &&
                          scc.SccVertexOne.Equals(result.SccVertexOne) &&
                          scc.SccVertexTwo.Equals(result.SccVertexTwo)
                    select scc);

                MinimalConstraintSet = new OrderedSet<string>();
                MinimalConstraintSet.AddMany(constraintsList);
                var queryResults = finalSccList as IList<SccQueryResult> ?? finalSccList.ToList();
                if (queryResults.Any())
                {
                    Parallel.ForEach(queryResults, currentSccList =>
                    {
                        var temp =
                            new OrderedSet<string>(GraphHelper.RetrieveConstraintsFromCycle(-1, -1,
                                currentSccList.SccComponentList, constraintGraph));
                        MinimalConstraintSet = MinimalConstraintSet.Intersection(temp);
                    });
                }
             
            }

            return MinimalConstraintSet.ToList();
        }
    }  
    }
