using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using QuickGraph;
using QuickGraph.Predicates;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.IncrementalQSatChecking.DataContracts;
using Tejas.Jhu.NegativeCycleDetection.DataContracts;
using Wintellect.PowerCollections;

namespace Tejas.Jhu.IncrementalQSatChecking
{
    public class IncrementalQSatChecking : IIncrementalQSatChecking
    {

        #region private class properties

        //the distance field of this object 
        private static OrderedSet<PseudoSlackProperties> PseudoSlackList { get; set; }
        private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstraintGraph {get; set; }
        private TaggedEdge<VertexProperties, EdgeProperties> ConstraintEdge { get; set; }
        private static IDictionary<VertexProperties, int> NewPotentialValues { get; set; }
        private static IDictionary<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> TrackedEdgeList { get; set;}
        List<VertexProperties> FailedConstraintVerticesList { get; set; }
        #endregion

        public IncrementalQSatCheckingResults CheckQSatisfiability(BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph, TaggedEdge<VertexProperties, EdgeProperties> constraintEdge)
        {
            //initialize
            ConstraintGraph = graph;
            ConstraintEdge = constraintEdge;
            var source = graph.Vertices.FirstOrDefault(p => p.Equals(ConstraintEdge.Source));
            var target = graph.Vertices.FirstOrDefault(p => p.Equals(ConstraintEdge.Target));
            if (source == null)
            {
                ConstraintEdge.Source.SetDistanceLabel(0);
                graph.AddVertex(ConstraintEdge.Source);
            }
            if (target == null)
            {
               ConstraintEdge.Target.SetDistanceLabel(constraintEdge.Source.DistanceLabel+ConstraintEdge.Tag.Weight);
                graph.AddVertex(ConstraintEdge.Target);
            }

            ComputeCorrectDistanceLabels();
            NewPotentialValues=new Dictionary<VertexProperties, int>();
            TrackedEdgeList=new Dictionary<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();
            FailedConstraintVerticesList = new List<VertexProperties>();
            TrackedEdgeList.Add(ConstraintEdge.Target,ConstraintEdge);

             //set the first entry in pseudo-slack
            PseudoSlackList=new OrderedSet<PseudoSlackProperties>
            {
                new PseudoSlackProperties(ConstraintEdge.Target,
                    new PseudoSlackValue(
                        ComputePseudoSlack(ConstraintEdge.Source.DistanceLabel, ConstraintEdge.Target.DistanceLabel,
                            ConstraintEdge.Tag.Weight), 0))
            };

            //intialize the rest of the priority queue
            InitializePseudoSlack();


            //now that we have the priority queue initialized, we can start the loop.
            var pseudoSlackProperties = PseudoSlackList.FirstOrDefault(p => p.Vertex.Equals(ConstraintEdge.Source));
            if (pseudoSlackProperties == null)
            {
                PseudoSlackList.Add(new PseudoSlackProperties(ConstraintEdge.Source, new PseudoSlackValue(0, int.MaxValue)));
            }
            pseudoSlackProperties = PseudoSlackList.FirstOrDefault(p => p.Vertex.Equals(ConstraintEdge.Source));
            while ((PseudoSlackList[0].Value.PseudoSlack < 0 && pseudoSlackProperties.Value.PseudoSlack == 0))
            {
                //do something
                //PseudoSlackValue minPseudoSlack = PseudoSlack[0].Value;
                                    
                VertexProperties currentVertex =
                    ConstraintGraph.Vertices.AsParallel().FirstOrDefault(p => p.Equals(PseudoSlackList[0].Vertex));
                if(!NewPotentialValues.ContainsKey(currentVertex))
                NewPotentialValues.Add(currentVertex,(currentVertex.DistanceLabel + PseudoSlackList[0].Value.PseudoSlack));
                else
                {
                    NewPotentialValues[currentVertex] = currentVertex.DistanceLabel +
                                                        PseudoSlackList[0].Value.PseudoSlack;
                }

                int numberOfEdgesToVertex = PseudoSlackList[0].Value.NumberOfEdges;
                PseudoSlackList[0].Value.PseudoSlack = 0;

                IEnumerable<TaggedEdge<VertexProperties, EdgeProperties>> outEdges;
                if (ConstraintGraph.TryGetOutEdges(currentVertex, out outEdges))
                {
                    try
                    {
                        Parallel.ForEach(outEdges, currentOutEdge =>
                        {
                            if (!NewPotentialValues.ContainsKey(currentOutEdge.Target))
                            {
                                //compute slack..
                                int newSlack=ComputePseudoSlack(NewPotentialValues[currentVertex], currentOutEdge.Target.DistanceLabel,
                                    currentOutEdge.Tag.Weight);
                                PseudoSlackProperties pseudoSlackTarget;
                                lock (this)
                                {
                                    pseudoSlackTarget =
                                        PseudoSlackList.AsParallel().FirstOrDefault(p => p.Vertex.Equals(currentOutEdge.Target));
                                }
                                //   int x =PseudoSlackList.IndexOf(pseudoSlackTarget);
                                if (newSlack < pseudoSlackTarget.Value.PseudoSlack ||
                                    (newSlack == pseudoSlackTarget.Value.PseudoSlack &&
                                     pseudoSlackTarget.Value.NumberOfEdges > (numberOfEdgesToVertex + 1)))
                                {
                                    lock (PseudoSlackList)
                                    {
                                        //int index = PseudoSlackList.IndexOf(pseudoSlackTarget);
                                        PseudoSlackList.Remove(pseudoSlackTarget);
                                    }
                                    lock (pseudoSlackTarget)
                                    {
                                        pseudoSlackTarget.Value.PseudoSlack = newSlack;
                                        pseudoSlackTarget.Value.NumberOfEdges = numberOfEdgesToVertex + 1;
                                    }
                                    lock (TrackedEdgeList)
                                    {
                                        if (!TrackedEdgeList.ContainsKey(currentOutEdge.Target))
                                        {

                                            TrackedEdgeList.Add(currentOutEdge.Target, currentOutEdge);

                                        }
                                        else
                                        {
                                            TrackedEdgeList[currentOutEdge.Target] = currentOutEdge;
                                        }
                                    }
                                    lock (PseudoSlackList)
                                    {
                                        PseudoSlackList.Add(pseudoSlackTarget);
                                    }
                                }
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            //check for UNSAT
            if (pseudoSlackProperties.Value.PseudoSlack < 0)
            {                   
                VertexProperties startVertex = ConstraintEdge.Source;
                //FailedConstraintVerticesList.Add(ConstraintEdge.Source);
                //FailedConstraintVerticesList.Add(constraintEdge.Target);
                while (!startVertex.Equals(ConstraintEdge.Target))
                {
                    var currentTrackedEdge = TrackedEdgeList[startVertex];
                    var counterEdge =graph.Edges.AsParallel()
                        .FirstOrDefault(
                            p => p.Source.Name.Equals(startVertex.Name, StringComparison.InvariantCultureIgnoreCase) &&
                                 p.Source.IsNegative != startVertex.IsNegative &&
                                 p.Target.Name.Equals(currentTrackedEdge.Source.Name,
                                     StringComparison.InvariantCultureIgnoreCase) &&
                                 p.Target.IsNegative != currentTrackedEdge.Source.IsNegative);
                    
                    //get the two vertices from pseudoslacklist
                    var counterSource = PseudoSlackList.AsParallel().FirstOrDefault(p => p.Vertex.Equals(counterEdge.Source));
                    var counterTarget = PseudoSlackList.AsParallel().FirstOrDefault(p => p.Vertex.Equals(counterEdge.Target));
                    int counterSlack = ComputePseudoSlack(counterEdge.Source.DistanceLabel,
                        counterEdge.Target.DistanceLabel, counterEdge.Tag.Weight);
                    if (counterSlack == 0 &&
                        counterSource.Value.NumberOfEdges != int.MaxValue &&
                        counterTarget.Value.NumberOfEdges != int.MaxValue &&
                        counterTarget.Value.NumberOfEdges == (counterSource.Value.NumberOfEdges + 1))
                    {
                        if(!TrackedEdgeList.ContainsKey(counterEdge.Target))
                            TrackedEdgeList.Add(counterEdge.Target,counterEdge);
                        else
                        TrackedEdgeList[counterTarget.Vertex] = counterEdge;
                    }
                    FailedConstraintVerticesList.Add(startVertex);
                    startVertex = currentTrackedEdge.Source;
                }
                FailedConstraintVerticesList.Add(ConstraintEdge.Target);
                FailedConstraintVerticesList.Reverse();
                return new IncrementalQSatCheckingResults(ConstraintGraph,NewPotentialValues,FailedConstraintVerticesList,ConstraintEdge,false);
            }

            ConstraintGraph.AddEdge(ConstraintEdge);
            ConstraintGraph.AddVertex(ConstraintEdge.Source);
            ConstraintGraph.AddVertex(ConstraintEdge.Target);

            return new IncrementalQSatCheckingResults(ConstraintGraph, NewPotentialValues, FailedConstraintVerticesList,ConstraintEdge, true);

        }

        

        #region private helper methods

        private void ComputeCorrectDistanceLabels()
        {
                VertexProperties sourceVertex = ConstraintGraph.Vertices.FirstOrDefault(p => p.Equals(ConstraintEdge.Source));
                if (sourceVertex != null)
                   ConstraintEdge.Source.SetDistanceLabel(sourceVertex.DistanceLabel);
                VertexProperties targetVertex = ConstraintGraph.Vertices.FirstOrDefault(p => p.Equals(ConstraintEdge.Target));
                if (targetVertex != null)
                    ConstraintEdge.Target.SetDistanceLabel(targetVertex.DistanceLabel);
            
        }

        private int ComputePseudoSlack(int sourceDistanceLabel, int targetDistanceLabel, int edgeWeight)
        {
            int slack;
            try
            {
                checked
                {
                    slack = sourceDistanceLabel - targetDistanceLabel + edgeWeight;
                }

            }
            catch (Exception e)
            {
                if (sourceDistanceLabel-targetDistanceLabel + edgeWeight <
                    0)
                    slack=(int.MaxValue);
                else
                {
                    slack=(int.MinValue);
                }
            }
            return slack;
        }

        private void InitializePseudoSlack()
        {
          // Parallel.ForEach(ConstraintGraph.Vertices.Where(p => !p.Equals(ConstraintEdge.Target)&& p!=null),
            foreach (VertexProperties currentVertex in ConstraintGraph.Vertices)
            {
                if (!currentVertex.Equals(ConstraintEdge.Target))
                    PseudoSlackList.Add(new PseudoSlackProperties(currentVertex, new PseudoSlackValue(0, int.MaxValue)));
            }
        
            //    currentVertex => PseudoSlackList.Add(new PseudoSlackProperties(currentVertex, new PseudoSlackValue(0, int.MaxValue))));
        }


        #endregion

    }
}