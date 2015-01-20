using System;
using System.Collections.Generic;
using QuickGraph;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;

namespace Tejas.Jhu.ImplicationChecking
{
    public abstract class ImplicationChecker
    {

        #region Abstract Methods

        /// <summary>
        /// Abstract method to check the implication of constraints in an incremental fashion.
        /// Pass a graph alongwith a list of new constraints that need to be incrementally checked for implication.
        /// NOTE: Method only overridden/ implemented in the IncrementalImplicationChecker class
        /// </summary>
        /// <param name="graph">Existing graph to check for implication</param>
        /// <param name="constraintsList">List of constraints to be checked for implication</param>
        /// <returns>The constraints that are implied by the current constraint graph</returns>
        public virtual IList<string> CheckImplication(
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, 
            EdgeProperties>> constraintGraph,IDictionary<VertexProperties,int> sourceRelevantShortestPathList,
            IDictionary<VertexProperties,int> targetRelevantShortestPaths,
            TaggedEdge<VertexProperties,EdgeProperties> addedConstraintEdge,  
            IList<string> constraintsList)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Abstract method to check for the consistency of a list of constraints in a non-incremental fashion.
        /// Pass it a list of constraints to check. The method will convert it to a graph and then check for satisfiability.
        /// NOTE: Method only overridden/ implemented in the NonIncrementalImplicationChecker class
        /// </summary>
        /// <param name="constraintsList">List of all the constraints to check for impication</param>
        /// <returns>The list of constraints that are implied by the current constraint graph</returns>
        public virtual IList<string> CheckImplication(BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph, List<string> constraintsList)
        {
            throw new NotImplementedException();
        }

        #endregion
    


    }
}