using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using QuickGraph;
using QuickGraph.Algorithms;
using Tejas.Jhu.GraphUtilities;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.IncrementalZSatChecking.DataContracts;
using Tejas.Jhu.NegativeCycleDetection;

namespace Tejas.Jhu.IncrementalZSatChecking
{
    public class IncrementalZSatChecking : IIncrementalZSatChecking
    {

        #region private class properties

        private static BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> OriginalConstraintGraph { get; set; }
        private static BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ChangedConstraintGraph { get; set; }        
        private static BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ReverseConstraintGraph{ get; set; }
        private IDictionary<VertexProperties, int> ChangedPotentialValues { get; set; }
        private TaggedEdge<VertexProperties, EdgeProperties> ConstraintEdge { get; set; }
        private IGraphHelper GraphHelper { get; set; }
        private IGraphTraversalAlgorithms GraphTraversalAlgorithms { get; set; }
        private TaggedEdge<VertexProperties, EdgeProperties> ReverseConstraintEdge { get; set; }
        private IDictionary<VertexProperties, int> SourceShortestPaths { get; set; }
        private IDictionary<VertexProperties, int> TargetShortestPaths { get; set; }
        private static bool IsConsistencyCheckSuccessful { get; set; }
        private VertexProperties FailedSource { get; set; }
        private VertexProperties FailedTarget { get; set; }
        #endregion

        #region Class constructor

        public IncrementalZSatChecking(IGraphHelper graphHelper, IGraphTraversalAlgorithms graphTraversalAlgorithms)
        {
            GraphHelper = graphHelper;
            GraphTraversalAlgorithms = graphTraversalAlgorithms;
        }

        #endregion
        #region public methods

        public IncrementalZSatResults CheckZSatisfiability(BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph,
            TaggedEdge<VertexProperties, EdgeProperties> constraintEdge, 
            IDictionary<VertexProperties, int> changedPotentialValues)
      
        {
            
            //intialize the members
            OriginalConstraintGraph = graph;
            //ChangedConstraintGraph = AlgorithmExtensions.Clone(graph,);
            ConstraintEdge = constraintEdge;
            ChangedPotentialValues = changedPotentialValues;
            ReverseConstraintGraph = new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();
            ChangedConstraintGraph = new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();
            IsConsistencyCheckSuccessful = true;
            //create the reverse graph            


            Parallel.ForEach(OriginalConstraintGraph.Edges, currentEdge =>
            {
                VertexProperties sourceVertex;
                VertexProperties targetVertex;
                if (changedPotentialValues.ContainsKey(currentEdge.Source))
                {
                    sourceVertex = new VertexProperties(currentEdge.Source.Name,
                        ChangedPotentialValues[currentEdge.Source], true, currentEdge.Source.IsNegative);
                }
                else
                {
                    sourceVertex = currentEdge.Source;
                }

                if (changedPotentialValues.ContainsKey(currentEdge.Target))
                {
                    targetVertex = new VertexProperties(currentEdge.Target.Name,
                        ChangedPotentialValues[currentEdge.Target], true, currentEdge.Target.IsNegative);
                }
                else
                {
                    targetVertex = currentEdge.Target;
                }

                lock (this)
                {
                    ReverseConstraintGraph.AddVertex(sourceVertex);
                    ReverseConstraintGraph.AddVertex(targetVertex);

                    ChangedConstraintGraph.AddVertex(sourceVertex);
                    ChangedConstraintGraph.AddVertex(targetVertex);

                    ////////--------------THIS IS WHAT IM CHANGING -----------------///////

                    EdgeProperties edgeTag= new EdgeProperties(0,currentEdge.Tag.Weight);
                    edgeTag.SetSlack(sourceVertex.DistanceLabel-targetVertex.DistanceLabel + currentEdge.Tag.Weight);
                    ////////--------------THIS IS WHAT IM CHANGING -----------------///////                    

                                                
                    ReverseConstraintGraph.AddEdge(new TaggedEdge<VertexProperties, EdgeProperties>(targetVertex,
                        sourceVertex, edgeTag));

                    ChangedConstraintGraph.AddEdge(new TaggedEdge<VertexProperties, EdgeProperties>(sourceVertex,
                        targetVertex, edgeTag));

                    

                }
            });

            ComputeChangedConstraintGraphSlack();
            
           
                        
            

            TaggedEdge<VertexProperties, EdgeProperties> constraintEdgeToPass = (from edge in ChangedConstraintGraph.Edges
                                                                                 where edge.Source.Equals(ConstraintEdge.Source) &&
                                                                                       edge.Target.Equals(ConstraintEdge.Target) &&
                                                                                       edge.Tag.Weight == ConstraintEdge.Tag.Weight
                                                                                 select edge).FirstOrDefault();


            if (constraintEdgeToPass == null) 
                return new IncrementalZSatResults(ChangedConstraintGraph,SourceShortestPaths,TargetShortestPaths,ConstraintEdge,FailedSource,FailedTarget,false);
            //IEnumerable<TaggedEdge<VertexProperties,EdgeProperties>> reversedEdges= graph.Edges.Reverse();

           //Run early termination
            //for edge x->y we want x->
            SourceShortestPaths=GraphTraversalAlgorithms.SingleSourceNonNegativeShortestPath_EarlyTermination(constraintEdgeToPass,
                ChangedConstraintGraph);
          //  ConstraintGraph.Edges.Reverse();

            
             ReverseConstraintEdge=new TaggedEdge<VertexProperties, EdgeProperties>(constraintEdgeToPass.Target,constraintEdgeToPass.Source,constraintEdgeToPass.Tag);

            //ReverseConstraintGraph.Edges.ToList().ForEach(edge => edge.Tag.SetSlack(edge.Target-));

                   


            TargetShortestPaths=GraphTraversalAlgorithms.SingleSourceNonNegativeShortestPath_EarlyTermination(ReverseConstraintEdge,
                ReverseConstraintGraph);

            //ConstraintGraph.Edges.Reverse();

            IEnumerable<IncrementalZSatQueryResults> relevantShortestPaths = from currentSourceKeyValue in SourceShortestPaths
            from currentTargetKeyValue in TargetShortestPaths
            where currentSourceKeyValue.Key.Name.Equals(currentTargetKeyValue.Key.Name) &&
                  currentSourceKeyValue.Key.IsNegative != currentTargetKeyValue.Key.IsNegative &&
                  currentSourceKeyValue.Value!=int.MaxValue && currentTargetKeyValue.Value!=int.MaxValue
            select new IncrementalZSatQueryResults(currentTargetKeyValue.Key, currentSourceKeyValue.Key,
                currentSourceKeyValue.Value + currentTargetKeyValue.Value - constraintEdgeToPass.Tag.Slack);

            var incrementalZSatQueryResultses = relevantShortestPaths as IList<IncrementalZSatQueryResults> ?? relevantShortestPaths.ToList();
            if(!incrementalZSatQueryResultses.Any())
                return new IncrementalZSatResults(ChangedConstraintGraph,SourceShortestPaths,TargetShortestPaths,constraintEdgeToPass,FailedSource,FailedTarget,true);
            

            Parallel.ForEach(incrementalZSatQueryResultses, (currentQueryObject, loopState) =>
            //foreach(IncrementalZSatQueryResults currentQueryObject in incrementalZSatQueryResultses)
            {
                var currentCounterQueryObject = (from counterQueryObject in incrementalZSatQueryResultses
                    where counterQueryObject.Source.Equals(currentQueryObject.Target) &&
                          counterQueryObject.Target.Equals(currentQueryObject.Source)
                    select counterQueryObject).FirstOrDefault();



                int sourceDistanceLabel;
                int targetDistanceLabel;
                IGraphTraversalAlgorithms graphTraversal=new GraphTraversalAlgorithms();
                sourceDistanceLabel = changedPotentialValues.ContainsKey(currentQueryObject.Source) ? changedPotentialValues[currentCounterQueryObject.Source] : currentQueryObject.Source.DistanceLabel;
                targetDistanceLabel = changedPotentialValues.ContainsKey(currentQueryObject.Target) ? changedPotentialValues[currentCounterQueryObject.Target] : currentQueryObject.Target.DistanceLabel;



                if (currentCounterQueryObject != null)
                {
                    if (currentQueryObject.Distance + currentCounterQueryObject.Distance == 0
                        && ((currentQueryObject.Source.DistanceLabel - currentQueryObject.Target.DistanceLabel) % 2 != 0))
                    {
                        IsConsistencyCheckSuccessful = false;
                        lock (this)
                        {
                            FailedSource = currentQueryObject.Source;
                            FailedTarget = currentQueryObject.Target;
                            loopState.Break();
                        }
                    }
                    else
                    {
                        IDictionary<VertexProperties, int> currentSourceShortestPath =
                            graphTraversal.SingleSourceNonNegativeShortestPath(currentQueryObject.Source,
                                OriginalConstraintGraph);
                        if (currentSourceShortestPath.ContainsKey(currentQueryObject.Target))
                        {
                            if (currentSourceShortestPath[currentQueryObject.Target] +
                                currentCounterQueryObject.Distance == 0 &&
                                ((currentQueryObject.Source.DistanceLabel - currentQueryObject.Target.DistanceLabel) % 2 != 0))
                            {
                                IsConsistencyCheckSuccessful = false;
                                lock (this)
                                {
                                    FailedSource = currentQueryObject.Source;
                                    FailedTarget = currentQueryObject.Target;
                                    loopState.Break();
                                }
                            }
                        }

                    }
                }
                else
                {
                    IDictionary<VertexProperties, int> currentCounterSourceShortestPath =
                        graphTraversal.SingleSourceNonNegativeShortestPath(currentQueryObject.Target,
                            ChangedConstraintGraph);
                    if (currentCounterSourceShortestPath.ContainsKey(currentQueryObject.Source))
                    {
                        if (currentCounterSourceShortestPath[currentQueryObject.Source] + currentQueryObject.Distance ==
                            0 && ((currentQueryObject.Source.DistanceLabel - currentQueryObject.Target.DistanceLabel)%2!=0))
                        {
                            
                            IsConsistencyCheckSuccessful = false;
                            lock (this)
                            {
                                FailedSource = currentQueryObject.Source;
                                FailedTarget = currentQueryObject.Target;
                                loopState.Break();
                            }
                        }
                    }

                }


            });


            if(!IsConsistencyCheckSuccessful)
                return new IncrementalZSatResults(ChangedConstraintGraph,SourceShortestPaths,
                    TargetShortestPaths,constraintEdgeToPass,FailedSource,FailedTarget,false);

            return new IncrementalZSatResults(ChangedConstraintGraph,SourceShortestPaths,
                TargetShortestPaths,constraintEdgeToPass,FailedSource,FailedTarget,true);

        }
        #endregion

        #region private helper methods


        private void ComputeChangedConstraintGraphSlack()
        {

            
                 foreach(TaggedEdge<VertexProperties,EdgeProperties> currentEdge in ChangedConstraintGraph.Edges)
            {
                if (currentEdge != null && !currentEdge.Source.Name.Equals(Constants.SourceVertexName))
                {

                    try
                    {
                        checked
                        {
                            currentEdge.Tag.SetSlack(currentEdge.Source.DistanceLabel - currentEdge.Target.DistanceLabel +
                                                     currentEdge.Tag.Weight);
                        }

                    }
                    catch (Exception e)
                    {
                        if (currentEdge.Source.DistanceLabel - currentEdge.Target.DistanceLabel + currentEdge.Tag.Weight <
                            0)
                            currentEdge.Tag.SetSlack(int.MaxValue);
                        else
                        {
                            currentEdge.Tag.SetSlack(int.MinValue);
                        }
                    }
                }
            }//);
            //};
        }
     
        private void ComputeOriginalConstraintGraphSlack()
        {


            foreach (TaggedEdge<VertexProperties, EdgeProperties> currentEdge in OriginalConstraintGraph.Edges)
            {
                if (currentEdge != null && !currentEdge.Source.Name.Equals(Constants.SourceVertexName))
                {

                    try
                    {
                        checked
                        {
                            currentEdge.Tag.SetSlack(currentEdge.Source.DistanceLabel - currentEdge.Target.DistanceLabel +
                                                     currentEdge.Tag.Weight);
                        }

                    }
                    catch (Exception e)
                    {
                        if (currentEdge.Source.DistanceLabel - currentEdge.Target.DistanceLabel + currentEdge.Tag.Weight <
                            0)
                            currentEdge.Tag.SetSlack(int.MaxValue);
                        else
                        {
                            currentEdge.Tag.SetSlack(int.MinValue);
                        }
                    }
                }
            }//);
            //};
        }

        #endregion
    }
}