using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickGraph;
using QuickGraph.Predicates;
using Tejas.Jhu.ConsistencyChecking.DataContracts;
using Tejas.Jhu.GraphUtilities;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.NegativeCycleDetection;
using Tejas.Jhu.NegativeCycleDetection.DataContracts;
using Wintellect.PowerCollections;

namespace Tejas.Jhu.ConsistencyChecking
{
    public class NonIncrementalConsistencyChecker : ConsistencyChecker
    {
        #region Private Class Properties

        private IGraphHelper GraphHelperObject { get; set; }    
        private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstraintGraph { get; set; }

        private INegativeCycleDetection NcdAlgorithm { get; set; }

        private NegativeCycleDetectionResult NegativeCycleResult { get; set; }
        private ConsistencyCheckResults ResultsOfConsistencyCheck;
        private IGraphTraversalAlgorithms SccComputer;
        private OrderedSet<string> MinimalConstraintSet;
        #endregion

        //you need to fill this in !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        #region Class Constructors

        public NonIncrementalConsistencyChecker(INegativeCycleDetection ncdAlgorithm, IGraphTraversalAlgorithms sccComputer,IGraphHelper graphHelperObject)
        {
            NcdAlgorithm = ncdAlgorithm;
            SccComputer = sccComputer;
            GraphHelperObject = graphHelperObject;
        }

        #endregion

        #region Virtual Method Overrides

        /// <summary>
        /// Abstract method to check for the consistency of a list of constraints in a non-incremental fashion.
        /// Pass it a list of constraints to check. The method will convert it to a graph and then check for satisfiability.
        /// </summary>
        /// <param name="constraintsList">List of all the constraints to check for consistency</param>
        /// <returns>Whether the consistency check passed or failed. If passed, then returns a graph also otherwise a list of failed constraints.</returns>
        public override ConsistencyCheckResults CheckConsistency(IList<string> constraintsList)
        {

            //convert constraints to graph
            ConstraintGraph=GraphHelperObject.ConvertConstraintsToGraph(constraintsList);
            // Call Negative Cycle Detection on Constraint
            NegativeCycleResult = NcdAlgorithm.DetectNegativeCycles(ConstraintGraph);
            //check for negative cycle
            if (NegativeCycleResult.IsNegativeCycleDetected)
            {
                //populate the result object and return.
                ResultsOfConsistencyCheck = new ConsistencyCheckResults
                    (false,ConstraintGraph,GraphHelperObject.RetrieveConstraintsFromCycle(-1, -1, NegativeCycleResult.FailedConstraints, ConstraintGraph));
                return ResultsOfConsistencyCheck;
            }
            //Compute SCCs
            IDictionary<VertexProperties, List<List<VertexProperties>>> stronglyConnectedComponentList = SccComputer.ComputeStronglyConnectedComponents(NegativeCycleResult.ScannedGraph);

            //SccQueryResult result =
            //    (from sccComponentListOne in stronglyConnectedComponentList.Values
            //     join sccComponentListTwo in stronglyConnectedComponentList.Values on
            //         sccComponentListOne equals sccComponentListTwo
            //     from sccComponentOne in sccComponentListOne
            //     from sccOne in sccComponentOne
            //     from sccComponentTwo in sccComponentListTwo
            //     from sccTwo in sccComponentTwo
            //     where
            //         sccOne.Name.Equals(sccTwo.Name) &&
            //         sccOne.IsNegative != sccTwo.IsNegative &&
            //         (sccOne.DistanceLabel - sccTwo.DistanceLabel) % 2 != 0                      
            //     select new SccQueryResult(sccComponentOne, sccOne, sccTwo))
            //        .FirstOrDefault();



            //var resultList =
            //    (from sccComponentListOne in stronglyConnectedComponentList.Values
            //     join sccComponentListTwo in stronglyConnectedComponentList.Values on
            //         sccComponentListOne equals sccComponentListTwo
            //     from sccComponentOne in sccComponentListOne
            //     from sccOne in sccComponentOne
            //     from sccComponentTwo in sccComponentListTwo
            //     from sccTwo in sccComponentTwo
            //     where
            //         sccOne.Name.Equals(sccTwo.Name) &&
            //         sccOne.IsNegative != sccTwo.IsNegative &&
            //         (sccOne.DistanceLabel - sccTwo.DistanceLabel) % 2 != 0
            //     select new SccQueryResult(sccComponentOne, sccOne, sccTwo));


            var resultList = (from sccComponentListOne in stronglyConnectedComponentList.Values
                              from scc in sccComponentListOne
                              from vertexOne in scc
                              from vertexTwo in scc
                              where vertexOne.Name.Equals(vertexTwo.Name) &&
                                    vertexOne.IsNegative != vertexTwo.IsNegative &&
                                    (vertexOne.DistanceLabel - vertexTwo.DistanceLabel) % 2 != 0
                              orderby scc.Count ascending
                              select new SccQueryResult(scc, vertexOne, vertexTwo));
           


            
            //check if Z UNSAt
            var sccQueryResults = resultList as IList<SccQueryResult> ?? resultList.ToList();
            if (sccQueryResults.Any())
            {
                // find constraint set responsible for Z UNSAT and return


                /*   int startIndex = result.Key.IndexOf(result.Value);
                IEnumerable<int> endIndexList=                    
                from vertex in result.Key
                where vertex.Name.Equals(result.Value.Name, StringComparison.InvariantCultureIgnoreCase) &&
                      vertex.IsNegative != result.Value.IsNegative
                select result.Key.IndexOf(vertex);*/

                //if (result.SccComponentList.IndexOf(result.SccVertexOne) >
                //  result.SccComponentList.IndexOf(result.SccVertexTwo))
                //{
                //ResultsOfConsistencyCheck = new ConsistencyCheckResults
                //    (false, ConstraintGraph,
                //        GraphHelperObject.RetrieveConstraintsFromCycle(
                //            result.SccComponentList.IndexOf(result.SccVertexOne),
                //            result.SccComponentList.IndexOf(result.SccVertexTwo), result.SccComponentList, ConstraintGraph));



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
                            new OrderedSet<string>(GraphHelperObject.RetrieveConstraintsFromCycle(-1, -1,
                                currentSccList.SccComponentList, ConstraintGraph));
                        MinimalConstraintSet = MinimalConstraintSet.Intersection(temp);
                    });
                }

                ResultsOfConsistencyCheck = new ConsistencyCheckResults(false, ConstraintGraph,
                    MinimalConstraintSet.ToList());
                return ResultsOfConsistencyCheck;
            }
            //  return ResultsOfConsistencyCheck;
                //}

                //else
                //{
                    //ResultsOfConsistencyCheck = new ConsistencyCheckResults
                    //    (false, ConstraintGraph, 
                    //    GraphHelperObject.RetrieveConstraintsFromCycle(
                    //    result.SccComponentList.IndexOf(result.SccVertexTwo), 
                    //    result.SccComponentList.IndexOf(result.SccVertexOne), result.SccComponentList, ConstraintGraph));

                    
                    
                //}
            

            ResultsOfConsistencyCheck = new ConsistencyCheckResults(true,ConstraintGraph,new List<string>());
            return ResultsOfConsistencyCheck;

        }

        #endregion

        #region Private Helper Methods


        #endregion
    }
}