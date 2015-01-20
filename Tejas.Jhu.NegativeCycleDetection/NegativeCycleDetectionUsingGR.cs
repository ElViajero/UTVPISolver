using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickGraph;
using QuickGraph.Algorithms.RandomWalks;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.NegativeCycleDetection.DataContracts;

namespace Tejas.Jhu.NegativeCycleDetection
{
    /// <summary>
    /// 
    /// </summary>
    public class NegativeCycleDetectionUsingGR : INegativeCycleDetection
    {
        #region Private Properties

        private  static BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> GraphWithSourceVertex { get; set;}
        private IGraphTraversalAlgorithms GraphTraversalAlgorithmsObj { get; set; }

        private IList<VertexProperties> VerticesToScan { get; set; }
        private IList<VertexProperties> LabeledVertices { get; set; }

        #endregion

        #region Class Constructors

        /// <summary>
        /// Class Constructors
        /// </summary>
        public NegativeCycleDetectionUsingGR(IGraphTraversalAlgorithms graphTraversalAlgorithmsObj)
        {
            VerticesToScan = new List<VertexProperties>();
            LabeledVertices = new List<VertexProperties>();
            GraphTraversalAlgorithmsObj = graphTraversalAlgorithmsObj;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The method takes in a graph and detects whether a negative cycle exists in the graph.
        /// If one does, the set of UTVPI constraints is not consistent and the method returns a minimal set 
        /// of graph edges that are responsible for the inconsistency.
        ///  If the graph does not contain negative cycles, we return the potential function of the 
        /// graph that gives the shortest path distance of each vertex from the source vertex.   
        /// </summary>
        /// <param name="graph">The QuickGraph that you want to analyze for negative cycles</param>
        /// <returns>INegativeCycleDetection - see object definition</returns>
        public NegativeCycleDetectionResult DetectNegativeCycles(
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph)
        {
            NegativeCycleDetectionResult results = new NegativeCycleDetectionResult();

            // Step 1:  Add a source vertex to the graph which is connected to all the vertices
            //          by a zero weight edge.
            //BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graphWithSourceVertex =
            GraphWithSourceVertex=
                AddSourceVertexToGraph(graph);

            //step 2: intialize distance labels and slack for all edges.
            InitializeDistanceLabels(GraphWithSourceVertex);

            //add the source vertex to the list of labelled vertices.
            LabeledVertices.Add(GraphWithSourceVertex.Vertices.FirstOrDefault(p => p.Name.Equals(Constants.SourceVertexName, StringComparison.InvariantCultureIgnoreCase) ));

            // Step 2: At the begining of each pass use list B to compute the vertices in List A

            while (LabeledVertices.Count != 0)
            {
                ComputeSlack(GraphWithSourceVertex);
                ComputeVerticesToScan(GraphWithSourceVertex);
                ComputeSlack(GraphWithSourceVertex);
                // Step 3: After computing A, order the vertices in A using DFS.
                if (VerticesToScan.Count > 0)
                {

                    try
                    {
                        DfsTraversalResult scanResult =
                            GraphTraversalAlgorithmsObj.CustomDFSTraversal(GraphWithSourceVertex, VerticesToScan);
                        results.IsNegativeCycleDetected = scanResult.NegativeCycleDetected;
                        VerticesToScan = scanResult.ListOfVertices;
                        if (scanResult.NegativeCycleDetected)
                        {

                            results.IsNegativeCycleDetected = true;
                            results.FailedConstraints = scanResult.ListOfVertices;
                            break;
                        }
                    }
                    catch (Exception excpn)
                    {
                        // Whatever you want to do with this exception
                        results.IsNegativeCycleDetected = true;
                    }

                    // step 4: Perform the Pass
                    PerformNegativeCycleDectectionPass(GraphWithSourceVertex);
                }
                if (LabeledVertices.Count == 0 &&
                    GraphWithSourceVertex.Vertices.Any(
                        p =>
                        !p.Visited &&
                        p.DistanceLabel == int.MaxValue &&
                        !p.Name.Equals(Constants.SourceVertexName, StringComparison.InvariantCultureIgnoreCase)))
                    
                {
                    LabeledVertices.Add(graph.Vertices.FirstOrDefault(p => p.Name.Equals(Constants.SourceVertexName, StringComparison.InvariantCultureIgnoreCase)));
                }

            }

            // Step 5: Remove the ROOT Vertex from the graph before returning it to the caller
            foreach (VertexProperties vertexPropertiese in GraphWithSourceVertex.Vertices.Where(vertexPropertiese => vertexPropertiese.Name.Equals(Constants.SourceVertexName)))
            {
                GraphWithSourceVertex.RemoveVertex(vertexPropertiese);
                break;
            }

            // Step 6: Prepare the result and return to caller
            results.ScannedGraph = GraphWithSourceVertex;

            return results;
        }

        #endregion

        #region Private Helper Methods

        private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> AddSourceVertexToGraph(BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph)
        {
            // Add the root vertex to the graph
            VertexProperties rootVertexProperties = new VertexProperties(Constants.SourceVertexName, 0, false, false);
            graph.AddVertex(rootVertexProperties);

            // Connect all the vertices in the graph to the root vertex i.e. add edges with zero weight
            
            //Parallel.ForEach(
            //    graph.Vertices.Where(vertex => !vertex.Name.Equals(Constants.SourceVertexName, StringComparison.InvariantCultureIgnoreCase)),
            //    currVertex => graph.AddEdge(new TaggedEdge<VertexProperties, EdgeProperties>(rootVertexProperties, currVertex, new EdgeProperties(int.MinValue, 0))));


            bool sourceConnected = false;
            Parallel.ForEach(graph.Vertices.Where(vertex => !vertex.Equals(rootVertexProperties)),
                currentVertex=>
                {
                    IEnumerable<TaggedEdge<VertexProperties, EdgeProperties>> outEdges;
                    IEnumerable<TaggedEdge<VertexProperties, EdgeProperties>> inEdges;
                    graph.TryGetInEdges(currentVertex, out inEdges);
                    graph.TryGetOutEdges(currentVertex, out outEdges);
                    if (!inEdges.Any() && !outEdges.Any())
                    {
                        currentVertex.SetDistanceLabel(0);
                        currentVertex.SetVisited(true);
                    }
                    else if (!inEdges.Any() && outEdges.Any())
                    {
                        graph.AddEdge(new TaggedEdge<VertexProperties, EdgeProperties>(rootVertexProperties,
                            currentVertex, new EdgeProperties(int.MinValue, 0)));
                        sourceConnected = true;
                    }
                });

            if (!sourceConnected)
            {
                VertexProperties connectingVertex =
                    graph.Vertices.FirstOrDefault(vertex => !vertex.Equals(rootVertexProperties));
                graph.AddEdge(new TaggedEdge<VertexProperties, EdgeProperties>(rootVertexProperties,
                            connectingVertex, new EdgeProperties(int.MinValue, 0)));
            }


            return graph;
        }

        private void InitializeDistanceLabels(BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph)
        {
            Parallel.ForEach(
                graph.Vertices.Where(vertex => !vertex.Name.Equals(Constants.SourceVertexName, StringComparison.InvariantCultureIgnoreCase)),
                currVertex => currVertex.SetDistanceLabel(int.MaxValue));
        }


        private void ComputeVerticesToScan(BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph)
        {
            //step 1: For every vertex in B that does not have atleast one outgoing edge with negative slack, delete the edge from B.
            Parallel.ForEach(LabeledVertices, labeledVertex =>
                {
                    /*
                    VertexProperties graphVertexProp =
                        graph.Vertices.FirstOrDefault(
                            gV => gV.Name.Equals(labeledVertex, StringComparison.InvariantCultureIgnoreCase));
                    */
                    if (labeledVertex != null)
                    {
                        IEnumerable<TaggedEdge<VertexProperties, EdgeProperties>> outEdges;
                        if (graph.TryGetOutEdges(labeledVertex, out outEdges))
                        {
                            IEnumerable<TaggedEdge<VertexProperties, EdgeProperties>> taggedEdges = outEdges as IList<TaggedEdge<VertexProperties, EdgeProperties>> ?? outEdges.ToList();
                            List<TaggedEdge<VertexProperties, EdgeProperties>> outEdgesCorrected = new List<TaggedEdge<VertexProperties, EdgeProperties>>(taggedEdges);
                            
                            // If the source vertex is Root then take only the first out edge from the root
                            // else take all out edges
                            if (labeledVertex.Name.Equals(Constants.SourceVertexName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                outEdgesCorrected.Clear();
                                outEdgesCorrected.Add(taggedEdges.FirstOrDefault(p => !p.Target.Visited));
                            }

                            foreach (TaggedEdge<VertexProperties, EdgeProperties> taggedEdge in outEdgesCorrected.Where(p => p != null))
                            {
                                taggedEdge.Source.SetVisited(true);
                                var lockObject =
                                    GraphWithSourceVertex.Vertices.FirstOrDefault(p => p.Equals(taggedEdge.Target));
                                lock (lockObject)
                                {
                                    ComputeSlack(GraphWithSourceVertex);

                                    taggedEdge.Tag.SetSlack(taggedEdge.Source.DistanceLabel -
                                                            taggedEdge.Target.DistanceLabel + taggedEdge.Tag.Weight);
                                    if (taggedEdge.Tag.Slack < 0)
                                    {
                                        lock (VerticesToScan)
                                        {
                                            if(!VerticesToScan.Contains(taggedEdge.Target))
                                            VerticesToScan.Add(taggedEdge.Target);
                                        }
                                        // source - target + weight = slack
                                        // lock (this)
                                        //{
                                        taggedEdge.Target.SetDistanceLabel(taggedEdge.Source.DistanceLabel +
                                                                           taggedEdge.Tag.Weight);

                                        lock (GraphWithSourceVertex)
                                        {
                                            //graph.Vertices.Where(p=> p.Equals(taggedEdge.Target)).ToList().ForEach(vertex => vertex.SetDistanceLabel(taggedEdge.Target.DistanceLabel));
                                            graph.Edges.Where(p => p.Target.Equals(taggedEdge.Target))
                                                .AsParallel()
                                                .ToList()
                                                .ForEach(
                                                    edge =>
                                                        edge.Target.SetDistanceLabel(taggedEdge.Target.DistanceLabel));
                                            graph.Edges.Where(p => p.Source.Equals(taggedEdge.Target))
                                                .AsParallel()
                                                .ToList()
                                                .ForEach(
                                                    edge =>
                                                        edge.Source.SetDistanceLabel(taggedEdge.Target.DistanceLabel));
                                        }
                                        taggedEdge.Tag.SetSlack(0);


                                    }
                                }
                            }

                        }
                        labeledVertex.SetVisited(true);
                        graph.Edges.Where(p => p.Target.Equals(labeledVertex)).AsParallel().ToList().ForEach(edge => edge.Target.SetVisited(true));
                        graph.Edges.Where(p => p.Source.Equals(labeledVertex)).AsParallel().ToList().ForEach(edge => edge.Source.SetVisited(true));
                    }                  
         
                });

            LabeledVertices.Clear();
        }
        
        private void PerformNegativeCycleDectectionPass(BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph)
        {

            foreach (VertexProperties currentVertex in VerticesToScan)
            {
             /*
                VertexProperties currentVertexProperty =
                    graph.Vertices.FirstOrDefault(p => p.Name.Equals(currentVertex, StringComparison.InvariantCultureIgnoreCase));
               */

               IEnumerable<TaggedEdge<VertexProperties, EdgeProperties>> outEdges;
                if (graph.TryGetOutEdges(currentVertex, out outEdges))
                {            
                    Parallel.ForEach(outEdges.ToList(), currentOutEdge =>
                    //foreach (var currentOutEdge in outEdges)                                                                  
                    {
                        var lockObject =
                            GraphWithSourceVertex.Vertices.FirstOrDefault(p => p.Equals(currentOutEdge.Target));
                        lock (lockObject)
                        {
                            currentOutEdge.Source.SetVisited(true);
                            //currentOutEdge.Tag.SetSlack(currentOutEdge.Source.DistanceLabel-currentOutEdge.Target.DistanceLabel + currentOutEdge.Tag.Slack);
                            lock (GraphWithSourceVertex)
                            {
                                ComputeSlack(GraphWithSourceVertex);
                            }
                            if (currentOutEdge.Tag.Slack < 0)
                            {

                                currentOutEdge.Target.SetDistanceLabel(currentOutEdge.Source.DistanceLabel +
                                                                       currentOutEdge.Tag.Weight);


                                //////////------------------------CHANGE-----------------/////////////////
                                lock (GraphWithSourceVertex)
                                {
                                    graph.Edges.Where(p => p.Target.Equals(currentOutEdge.Target))
                                        .AsParallel()
                                        .ToList()
                                        .ForEach(
                                            edge => edge.Target.SetDistanceLabel(currentOutEdge.Target.DistanceLabel));
                                    graph.Edges.Where(p => p.Source.Equals(currentOutEdge.Target))
                                        .AsParallel()
                                        .ToList()
                                        .ForEach(
                                            edge => edge.Source.SetDistanceLabel(currentOutEdge.Target.DistanceLabel));
                                }
                                //////////------------------------CHANGE-----------------/////////////////



                                // currentOutEdge.Tag.Slack = 0;
                                lock (LabeledVertices)
                                {
                                   
                                    LabeledVertices.Add(currentOutEdge.Target);
                                }
                            }
                        }
                    });
                }

                if (currentVertex != null)
                {
                    currentVertex.SetVisited(true);
                    graph.Edges.Where(p => p.Target.Equals(currentVertex)).AsParallel().ToList().ForEach(edge => edge.Target.SetVisited(true));
                    graph.Edges.Where(p => p.Source.Equals(currentVertex)).AsParallel().ToList().ForEach(edge => edge.Source.SetVisited(true));

                }
            }

            VerticesToScan.Clear();
            
        }

        private void ComputeSlack(BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph)
        {
            Parallel.ForEach(graph.Edges, currentEdge =>
                // foreach(TaggedEdge<VertexProperties,EdgeProperties> currentEdge in graph.Edges)
            {
                if (currentEdge != null && !currentEdge.Source.Name.Equals(Constants.SourceVertexName))
                {

                    //otherwise the int cannot be increased and wraps around to min value. Something we don't want.
                    //if (currentEdge.Source.DistanceLabel == int.MaxValue && (currentEdge.Tag.Weight-currentEdge.Target.DistanceLabel)>0)
                    //    currentEdge.Tag.SetSlack(currentEdge.Tag.Weight);

                    ////otherwise int cannot be decreased further and wraps around to max value. Again, something we don't want.                        
                    //else if (currentEdge.Source.DistanceLabel == 0 &&
                    //         currentEdge.Target.DistanceLabel == int.MaxValue && currentEdge.Tag.Weight <= 0)
                    //    currentEdge.Tag.Slack = int.MinValue;

                    //else

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
            });
            //};
        }

        #endregion
    }
}

