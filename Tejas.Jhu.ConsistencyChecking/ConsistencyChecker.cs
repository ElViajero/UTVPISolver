using System;
using System.Collections.Generic;
using QuickGraph;
using Tejas.Jhu.ConsistencyChecking.DataContracts;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.IncrementalZSatChecking.DataContracts;

namespace Tejas.Jhu.ConsistencyChecking
{
    public abstract class ConsistencyChecker
    {
        #region Abstract Methods

        /// <summary>
        /// Abstract method to check the consistency in an incremental fashion.
        /// Pass a graph alongwith a list of new constraints that need to be incrementally added to the graph to check for satisfiability.
        /// NOTE: Method only overridden/ implemented in the IncrementalConsistencyChecker class
        /// </summary>
        /// <param name="graph">Existing graph to check for consistency</param>
        /// <param name="constraintsList">List of constraints to be added to the graph incrementally</param>
        /// <returns>Whether the consistency check passed or failed. If passed, then returns a graph also otherwise a list of failed constraints.</returns>
        public virtual ConsistencyCheckResults CheckConsistency(BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph, string constraint)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Abstract method to check for the consistency of a list of constraints in a non-incremental fashion.
        /// Pass it a list of constraints to check. The method will convert it to a graph and then check for satisfiability.
        /// NOTE: Method only overridden/ implemented in the NonIncrementalConsistencyChecker class
        /// </summary>
        /// <param name="constraintsList">List of all the constraints to check for consistency</param>
        /// <returns>Whether the consistency check passed or failed. If passed, then returns a graph also otherwise a list of failed constraints.</returns>
        public virtual ConsistencyCheckResults CheckConsistency(IList<string> constraintsList)
        {
            throw new NotImplementedException();            
        }

        #endregion
    }
}