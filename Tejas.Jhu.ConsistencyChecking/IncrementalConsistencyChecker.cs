using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using QuickGraph;
using Tejas.Jhu.ConsistencyChecking.DataContracts;
using Tejas.Jhu.GraphUtilities;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.IncrementalQSatChecking;
using Tejas.Jhu.IncrementalQSatChecking.DataContracts;
using Tejas.Jhu.IncrementalZSatChecking;
using Tejas.Jhu.IncrementalZSatChecking.DataContracts;
using Tejas.Jhu.NegativeCycleDetection;


namespace Tejas.Jhu.ConsistencyChecking
{
    public class IncrementalConsistencyChecker : ConsistencyChecker
    {
        #region Private Class Properties
        
        private IGraphHelper GraphHelperObject { get; set; }    
        private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstraintGraph { get; set; }
        private IGraphTraversalAlgorithms GraphTraversalAlgorithms { get; set; }
        private IIncrementalQSatChecking IncrementalQSatChecking { get; set; }
        private IIncrementalZSatChecking IncrementalZSatChecking { get; set; }
        private IncrementalQSatCheckingResults QsatCheckResults { get; set; }
        private IncrementalZSatResults ZsatCheckResults { get; set; }                
        private List<string> FailedConstraints { get; set; }     
        
        #endregion

        #region Class Constructors

        public IncrementalConsistencyChecker(IGraphHelper graphHelper,
            IGraphTraversalAlgorithms graphTraversalAlgorithms, IIncrementalQSatChecking incrementalQSatChecking,IIncrementalZSatChecking incrementalZSatChecking)
        {
            GraphHelperObject = graphHelper;
            GraphTraversalAlgorithms = graphTraversalAlgorithms;
            IncrementalQSatChecking = incrementalQSatChecking;
            IncrementalZSatChecking = incrementalZSatChecking;
        }


        #endregion

        #region Abstract Method Override

        /// <summary>
        /// Abstract method to check the consistency in an incremental fashion.
        /// Pass a graph alongwith a list of new constraints that need to be incrementally added to the graph to check for satisfiability.
        /// </summary>
        /// <param name="graph">Existing graph to check for consistency</param>
        /// <param name="constraintsList">List of constraints to be added to the graph incrementally</param>
        /// <returns>Whether the consistency check passed or failed. If passed, then returns a graph also otherwise a list of failed constraints.</returns>
        public override ConsistencyCheckResults CheckConsistency(
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph,
           string constraint)
        {

            ConstraintGraph = graph;

            
                IList<string> singleConstraintList =new List<string>();
                singleConstraintList.Add(constraint);
                var constraintEdges = GraphHelperObject.ConvertConstriantsToEdges(singleConstraintList);

                foreach (TaggedEdge<VertexProperties, EdgeProperties> currentEdge in constraintEdges)
                {
                    QsatCheckResults = IncrementalQSatChecking.CheckQSatisfiability(ConstraintGraph, currentEdge);

                    if (!QsatCheckResults.IsConsistencyCheckSuccessful)
                    {
                        FailedConstraints=new List<string>();
                        FailedConstraints=GraphHelperObject.RetrieveConstraintsFromCycle(-1, -1,
                            QsatCheckResults.FailedConstraintVerticesList, ConstraintGraph);

                        //got the add the current constraint and delete the constraint between the first and last vertices... STILL NOT DONE.
                        if(FailedConstraints.Count>0)
                            FailedConstraints.RemoveAt(FailedConstraints.Count-1);
                        FailedConstraints.Add(constraint);
                        return new ConsistencyCheckResults(false,ConstraintGraph,FailedConstraints);
                    }
                    ConstraintGraph = QsatCheckResults.ConstraintGraph;
                }

                ZsatCheckResults=IncrementalZSatChecking.CheckZSatisfiability(ConstraintGraph, constraintEdges[0],
                    QsatCheckResults.ChangedPotentialValues);
                if (!ZsatCheckResults.IsConsistencyCheckSuccessful)
                {                 
                    //FailedConstraints=   
                    return new ConsistencyCheckResults(false, ConstraintGraph, FailedConstraints);
                }
                
                ConstraintGraph = ZsatCheckResults.ConstraintGraph;
            
            

            return new ConsistencyCheckResults(true,ConstraintGraph,FailedConstraints,ZsatCheckResults.SourceRelevantShortestPathsList,
                ZsatCheckResults.TargetRelevantShortestPathsList,ZsatCheckResults.ConstraintEdge);
        }



        #endregion

        #region Private Helper Methods



        #endregion
    }
}