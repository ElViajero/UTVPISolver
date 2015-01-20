using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using QuickGraph;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.NegativeCycleDetection.DataContracts;
using Wintellect.PowerCollections;


namespace Tejas.Jhu.NegativeCycleDetection
{

    public class GraphTraversalAlgorithms : IGraphTraversalAlgorithms
    {
        #region Private Class Properties

        private bool NegativeCycleFound { get; set; }
        private IDictionary<VertexProperties, int> DfsScannedVertices { get; set; }
        private IDictionary<VertexProperties, int> CurrentScanVertices { get; set; }
        private IDictionary<VertexProperties, int> VertexCaliberDictionary { get; set; }
        private IDictionary<VertexProperties, int> DjikstrasShortestPaths { get; set; }
        private IDictionary<VertexProperties, DjikstrasEdgeTrackObject> DjikstrasShortestPathsEdgeTracking { get; set; }
        private IDictionary<VertexProperties, List<List<VertexProperties>>> StronglyConnectedComponents { get; set; }
        private List<VertexProperties> NegativeCycleVertices { get; set; }
        private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> Graph { get; set; }
        private  OrderedSet<DjikstrasVertexProperties> ExactVerticesQueue { get; set; }           // THIS WAS STATIC
        private  OrderedSet<DjikstrasVertexProperties> StandardDjikstraQueue { get; set; }
        private  IList<DjikstrasQueryResult> TargetDjikstrasQueryObjects { get; set; }
        

        //properties for early termination

        private static OrderedSet<DjikstraEarlyTerminationVertexProperties> EarlyTerminationExactVerticesQueue { get; set; }
        private static OrderedSet<DjikstraEarlyTerminationVertexProperties> EarlyTerminationDjikstraQueue { get; set; }
        private static IList<EarlyTerminationDjikstraQueryResult> EarlyTerminationTargetDjikstrasQueryObjects { get; set; }
        private static TaggedEdge<VertexProperties,EdgeProperties> ConstraintEdge { get; set; }
        //private static IList<DjikstrasQueryResult> targetDjikstrasQueryObjects { get; set; }


        #endregion

        #region Public Methods

        /// <summary>
        /// CustomDFSTraversal
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="VerticesToScan"></param>
        /// <returns></returns>
        public DfsTraversalResult CustomDFSTraversal(
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph,
            IList<VertexProperties> VerticesToScan)
        {
            /* keep a dictionary of <string, int> which stores the vertex name along with it's index in the stack.
             * Also, keep a list of vertices as we find them for the topological sort.
             * 
             *1. Start with the source.
             *2. Iterate through outgoing edges to find an edge with slack <=0.
             *3. Check if vertex already exists in map. If yes, perform steps 4-7. If no, skip to step 8.
             *4. If so, we have a cycle. 
             *5. Retrieve the index of the target vertex and iterate from that index to the end of the stack checking each slack edge in the graph. - Potentially Parallelizable since we are checking slack of individual edges. However, we need to know index i and i+1 to compute edge between the vertices.
             *6. If we find that one of the edges has negative slack, TERMINATE and report negative cycle.
             *7. If the cycle is zero slack ( and therefore zero weight), go back to step 2 to find another edge to continue the search.
             *8. Lastly, if the vertex was not on the map, add it to the stack and to the map. Note the index and add it to the map. 
             *9. Increment the index by one
             *10. Repeat from step 2 with the target as the new source.
             * */

            DfsScannedVertices = new Dictionary<VertexProperties, int>();
            CurrentScanVertices = new Dictionary<VertexProperties, int>();
            NegativeCycleFound = false;
            Graph = graph;

            VertexProperties rootVertex = (from vertex in graph.Vertices
                where
                    vertex.Name.Equals(Constants.SourceVertexName,
                        StringComparison.InvariantCultureIgnoreCase)
                select vertex).FirstOrDefault();

            DfsRecursive(rootVertex, 0, -1);

            DfsTraversalResult result;
            if (NegativeCycleFound)
            {
                result = new DfsTraversalResult(NegativeCycleVertices, NegativeCycleFound);
            }
            else
            {
                List<VertexProperties> listOfOrderedVertices = (from vertex in DfsScannedVertices
                    join existingVertices in VerticesToScan on vertex.Key//.Name
                                                                equals existingVertices//.Name
                    orderby vertex.Value
                    select vertex.Key).ToList();

                result = new DfsTraversalResult(listOfOrderedVertices, NegativeCycleFound);
            }

            return result;
        }

        /// <summary>
        /// ComputeStronglyConnectedComponents
        /// </summary>
        /// <param name="graph"></param>
        public IDictionary<VertexProperties, List<List<VertexProperties>>> ComputeStronglyConnectedComponents(
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph)
        {
            /* Input: A graph with potential function and slack edges correctly laid out by an NCD.
             * Output :The IDict of SCCs
             * Take the graph and perform DFS on zero slack edges.
             * For each vertex along the path add the vertex to an IDict < SCC root vertex, List<SCC vertices>>
             * WHen a vertex points to a previous vertex, we have an SCC. 
             * So add every vertex from the root vertex to the current vertex to the list whose key in the IDict is the root vertex.
             * 
             * WHen this procedure terminates we have all SCCs in the zero slack graph.
             */

            Graph = graph;
            DfsScannedVertices = new Dictionary<VertexProperties, int>();
            CurrentScanVertices = new Dictionary<VertexProperties, int>();
            StronglyConnectedComponents = new Dictionary<VertexProperties, List<List<VertexProperties>>>();

            ParallelQuery<VertexProperties> startingVertices = from vertex in graph.Vertices.AsParallel()
                where vertex.DistanceLabel == 0
                select vertex;

            foreach (VertexProperties vertexProperties in startingVertices)
            {
                if (!DfsScannedVertices.ContainsKey(vertexProperties))
                {
                    DfsScannedVertices.Add(vertexProperties, 0);
                    CurrentScanVertices.Add(vertexProperties, 0);
                    StronglyConnectedComponents.Add(vertexProperties, new List<List<VertexProperties>>());
                }
                SccRecursive(vertexProperties, 0);
                CurrentScanVertices.Remove(vertexProperties);
            }

            return StronglyConnectedComponents;
        }

        public IDictionary<VertexProperties, int> SingleSourceNonNegativeShortestPath(
            VertexProperties sourceVertex,
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph)
        {

            StandardDjikstraQueue = new OrderedSet<DjikstrasVertexProperties>();
            ExactVerticesQueue = new OrderedSet<DjikstrasVertexProperties>();
            DjikstrasShortestPaths = new Dictionary<VertexProperties, int>();
            Graph = graph;
            TargetDjikstrasQueryObjects =new List<DjikstrasQueryResult>();
            ComputeCaliberList(sourceVertex);
            ExactVerticesQueue.Add(new DjikstrasVertexProperties(sourceVertex,null,0));
            InitializeStandardDjikstraQueue(sourceVertex);
            //if (!earlyTermination)
            //{
                while (StandardDjikstraQueue.ToList().Count > 0 || ExactVerticesQueue.ToList().Count > 0)
                {
                    TargetDjikstrasQueryObjects.Clear();
                    ComputeSingleSourceNonNegativeShortestPath(sourceVertex);
                    
                    StandardDjikstraQueue = StandardDjikstraQueue.Difference(ExactVerticesQueue);

                }
            //}
            return DjikstrasShortestPaths;
        }

        public IDictionary<VertexProperties, DjikstrasEdgeTrackObject> SingleSourceNonNegativeShortestPath_EdgeTracking(VertexProperties sourceVertex, 
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph)            
        {

            StandardDjikstraQueue = new OrderedSet<DjikstrasVertexProperties>();
            ExactVerticesQueue = new OrderedSet<DjikstrasVertexProperties>();
            DjikstrasShortestPathsEdgeTracking = new Dictionary<VertexProperties, DjikstrasEdgeTrackObject>();
            Graph = graph;
            TargetDjikstrasQueryObjects = new List<DjikstrasQueryResult>();
            ComputeCaliberList(sourceVertex);
            ExactVerticesQueue.Add(new DjikstrasVertexProperties(sourceVertex, null, 0,0));
            InitializeStandardDjikstraQueue(sourceVertex);
            //if (!earlyTermination)
            //{
            while (StandardDjikstraQueue.ToList().Count > 0 || ExactVerticesQueue.ToList().Count > 0)
            {
                TargetDjikstrasQueryObjects.Clear();
                ComputeSingleSourceNonNegativeShortestPath_EdgeTracking(sourceVertex);
                
                StandardDjikstraQueue = StandardDjikstraQueue.Difference(ExactVerticesQueue);

            }
            //}

           
            return DjikstrasShortestPathsEdgeTracking;


            throw new NotImplementedException();
        }

        public IDictionary<VertexProperties, int> SingleSourceNonNegativeShortestPath_EarlyTermination(TaggedEdge<VertexProperties,EdgeProperties> constraintEdge , BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph)
        {

            EarlyTerminationDjikstraQueue = new OrderedSet<DjikstraEarlyTerminationVertexProperties>();
            EarlyTerminationExactVerticesQueue = new OrderedSet<DjikstraEarlyTerminationVertexProperties>();
            DjikstrasShortestPaths = new Dictionary<VertexProperties, int>();
            Graph = graph;
            ConstraintEdge = constraintEdge;
            EarlyTerminationTargetDjikstrasQueryObjects = new List<EarlyTerminationDjikstraQueryResult>();
            ComputeCaliberList(constraintEdge.Source);
            EarlyTerminationExactVerticesQueue.Add(new DjikstraEarlyTerminationVertexProperties(new DjikstrasVertexProperties(constraintEdge.Source, null, 0),false));
            InitializeEarlyTerminationDjikstraQueue(constraintEdge.Target);
            EarlyTerminationExactVerticesQueue.Add(
                new DjikstraEarlyTerminationVertexProperties(
                    new DjikstrasVertexProperties(constraintEdge.Target,constraintEdge.Source, constraintEdge.Tag.Slack), true));
            //if (!earlyTermination)
            //{
           // Any(p => p.IsRelevant == true);
            while (EarlyTerminationDjikstraQueue[0].IsRelevant || EarlyTerminationExactVerticesQueue.Any())
            {
                EarlyTerminationTargetDjikstrasQueryObjects = new List<EarlyTerminationDjikstraQueryResult>();
                ComputeSingleSourceNonNegativeShortestPathEarlyTermination();

                EarlyTerminationDjikstraQueue = EarlyTerminationDjikstraQueue.Difference(EarlyTerminationExactVerticesQueue);

            }
            //}
            if(DjikstrasShortestPaths.ContainsKey(ConstraintEdge.Source))
                DjikstrasShortestPaths.Remove(ConstraintEdge.Source);
            return DjikstrasShortestPaths;
        }




        



        #endregion

        #region Private Helper Methods

        /// <summary>
        /// DfsRecursive - method to recurisvely traverse the graph depth first
        /// </summary>
        /// <param name="sourceVertex"></param>
        /// <param name="depth"></param>
        /// <param name="indexOfMostRecentNegativeSlack"></param>
        private void DfsRecursive(VertexProperties sourceVertex, int depth, int indexOfMostRecentNegativeSlack)
        {
            IEnumerable<TaggedEdge<VertexProperties, EdgeProperties>> edges;
            bool isOutEdgeAvailable = Graph.TryGetOutEdges(sourceVertex, out edges);

            if (isOutEdgeAvailable)
            {
                depth++;
                foreach (TaggedEdge<VertexProperties, EdgeProperties> taggedEdge in edges.Where(p => p.Tag.Slack <= 0))
                {
                    VertexProperties newSource = taggedEdge.Target;

                    if (taggedEdge.Tag.Slack < 0)
                    {
                        indexOfMostRecentNegativeSlack = depth;
                    }

                    if (CurrentScanVertices.ContainsKey(newSource))
                    {
                        // Contains a cycle. Check if negative cycle
                        var indexFromList = DfsScannedVertices[newSource];
                        if (indexFromList <= indexOfMostRecentNegativeSlack)
                        {

                            NegativeCycleVertices = (from vertex in CurrentScanVertices
                                where vertex.Value >= indexFromList
                                select vertex.Key).ToList();

                            NegativeCycleFound = true;
                            return;
                            //throw new Exception("Found negative cycle in the graph");                            
                        }

                        // scan the next outEdge
                        continue;
                    }

                    // Add the vertex to the dictionary/ map
                    if (!DfsScannedVertices.ContainsKey(newSource))
                    {
                        DfsScannedVertices.Add(newSource, depth);
                    }

                    if (!CurrentScanVertices.ContainsKey(newSource))
                    {
                        CurrentScanVertices.Add(newSource, depth);
                    }

                    // Call recursive to traverse depth of graph
                    DfsRecursive(newSource, depth, indexOfMostRecentNegativeSlack);

                    if (NegativeCycleFound)
                    {
                        return;
                    }

                    // Remove the current scanned vertex from the dictionary before moving on to the next vertex
                    CurrentScanVertices.Remove(newSource);
                }
            }
            else
            {
                // No more out edges. Just return
                return;
            }
        }
        /// <summary>
        /// SccRecursive
        /// </summary>
        /// <param name="sourceVertex"></param>
        /// <param name="depth"></param>
        private void SccRecursive(VertexProperties sourceVertex, int depth)
        {
            IEnumerable<TaggedEdge<VertexProperties, EdgeProperties>> edges;
            if (Graph.TryGetOutEdges(sourceVertex, out edges))
            {
                depth++;
                foreach (
                    VertexProperties newSource in
                        edges.Where(p => p.Tag.Slack == 0).Select(taggedEdge => taggedEdge.Target))
                {
                    if (CurrentScanVertices.ContainsKey(newSource))
                    {
                        // Contains a cycle (SCC).
                        int indexFromList = DfsScannedVertices[newSource];
                        StronglyConnectedComponents[newSource].Add((from vertex in CurrentScanVertices
                            where vertex.Value >= indexFromList
                            select vertex.Key).ToList());
                    }
                    else
                    {
                        if (!DfsScannedVertices.ContainsKey(newSource))
                        {
                            DfsScannedVertices.Add(newSource, depth);
                        }

                        if (!CurrentScanVertices.ContainsKey(newSource))
                        {
                            CurrentScanVertices.Add(newSource, depth);
                        }

                        if (!StronglyConnectedComponents.ContainsKey(newSource))
                        {
                            StronglyConnectedComponents.Add(newSource, new List<List<VertexProperties>>());
                        }

                        // Call recursive to traverse depth of graph
                        SccRecursive(newSource, depth);
                        CurrentScanVertices.Remove(newSource);
                    }
                }
            }
        }

        private void ComputeCaliberList(VertexProperties sourceVertex)

        {

            //test


            
            VertexCaliberDictionary = new Dictionary<VertexProperties, int>();
            //Parallel.ForEach(Graph.Vertices, currentVertex =>
            foreach (VertexProperties currentVertex in Graph.Vertices)



            {
                int vertexCaliber = int.MaxValue;
                IEnumerable<TaggedEdge<VertexProperties, EdgeProperties>> inEdges = null;
                bool x = Graph.TryGetInEdges(currentVertex, out inEdges);
                if(inEdges.ToList().Count>0)                
                {
                    vertexCaliber = (from currentEdge in inEdges
                    select currentEdge.Tag.Slack).Min();

                }
                if (!VertexCaliberDictionary.ContainsKey(currentVertex))
                {

                    VertexCaliberDictionary.Add(currentVertex, vertexCaliber);
                }
                //});
            }
        }

        private void ComputeSingleSourceNonNegativeShortestPath(VertexProperties sourceVertex)//VertexProperties sourceVertex)
        {
            if (ExactVerticesQueue.Count > 0)
            {

                //this is getting overwritten each time....
                //targetDjikstrasQueryObjects = new List<DjikstrasQueryResult>();
                Parallel.ForEach(ExactVerticesQueue.ToList(), currentDjikstrasVertex =>
                {
                    IEnumerable<TaggedEdge<VertexProperties, EdgeProperties>> outEdges;
                    bool areOutEdgesPresent=false;
                    areOutEdgesPresent=Graph.TryGetOutEdges(currentDjikstrasVertex.Vertex, out outEdges);
                    if (areOutEdgesPresent && outEdges.ToList().Count > 0)
                    {
                        List<DjikstrasQueryResult> targetDjikstrasQueryObjectsTemp;
                        lock (StandardDjikstraQueue)
                        {
                             targetDjikstrasQueryObjectsTemp =
                                (from edge in outEdges
                                    from djikstrasvertex in StandardDjikstraQueue
                                    from targetVertexCaliber in VertexCaliberDictionary
                                    where edge.Target.Equals(djikstrasvertex.Vertex)
                                          && targetVertexCaliber.Key.Equals(edge.Target)
                                          && !edge.Target.Equals(sourceVertex)
                                    select
                                        new DjikstrasQueryResult(targetVertexCaliber.Value, currentDjikstrasVertex,
                                            djikstrasvertex, edge.Tag)).ToList();
                        }
                        //now enumerate over the query results and make changes as necessary
                        lock (TargetDjikstrasQueryObjects)
                        {
                            TargetDjikstrasQueryObjects =
                                TargetDjikstrasQueryObjects.Concat(targetDjikstrasQueryObjectsTemp).ToList();
                        }

                    }

                    lock(DjikstrasShortestPaths)
                    {
                        if(!DjikstrasShortestPaths.ContainsKey(currentDjikstrasVertex.Vertex))
                        DjikstrasShortestPaths.Add(currentDjikstrasVertex.Vertex,
                            currentDjikstrasVertex.DjikstrasDistanceLabel);
                    }
                });

                

                ExactVerticesQueue.Clear();
                if (TargetDjikstrasQueryObjects.Count==0)
                    return;

                Parallel.ForEach(TargetDjikstrasQueryObjects, currentDjikstraQueryObject =>
                {

                    if (currentDjikstraQueryObject.SourceDjikstrasVertex.DjikstrasDistanceLabel +
                        currentDjikstraQueryObject.Edge.Slack
                        <= currentDjikstraQueryObject.TargetDjikstrasVertex.DjikstrasDistanceLabel)
                        
                    {



                        int newDistanceLabel =
                            currentDjikstraQueryObject.SourceDjikstrasVertex.DjikstrasDistanceLabel +
                            currentDjikstraQueryObject.Edge.Slack;
                        DjikstrasVertexProperties currentVertexInStandardQueue;
                        lock (StandardDjikstraQueue)
                        {
                             currentVertexInStandardQueue =
                                (from djikstrasVertex in StandardDjikstraQueue.ToList()
                                    where
                                        djikstrasVertex.Vertex.Equals(
                                            currentDjikstraQueryObject.TargetDjikstrasVertex.Vertex)
                                    select djikstrasVertex).FirstOrDefault();
                        }
                        //synchronize the update to avoid inconsistent values.
                        if (currentVertexInStandardQueue != null &&
                            !ExactVerticesQueue.ToList().Contains(currentVertexInStandardQueue)) //some other thread might have removed currentVertexInStandarQueue
                        {
                            /////////--------------------------------THIS IS WHAT IM CHANGING------------------------------//////////
                            lock (StandardDjikstraQueue)
                            {
                            ////    int x= StandardDjikstraQueue.IndexOf(currentVertexInStandardQueue);
                                StandardDjikstraQueue.Remove(currentVertexInStandardQueue);
                            }
                            /////////--------------------------------THIS IS WHAT IM CHANGING------------------------------//////////

                            lock (currentVertexInStandardQueue)
                            {
                                

                                currentVertexInStandardQueue.SetDjikstrasDistanceLabel(newDistanceLabel);
                                currentVertexInStandardQueue.SetParentVertex(
                                    currentDjikstraQueryObject.SourceDjikstrasVertex.Vertex);

                                lock (StandardDjikstraQueue)
                                {
                                    StandardDjikstraQueue.Add(currentVertexInStandardQueue);
                                }

                                //.TargetDjikstrasVertex.SetDjikstrasDistanceLabel(newDistanceLabel);
                                //currentDjikstraQueryObject.TargetDjikstrasVertex.SetParentVertex(currentDjikstraQueryObject.SourceDjikstrasVertex.Vertex);

                                if (currentDjikstraQueryObject.Caliber +
                                    currentDjikstraQueryObject.SourceDjikstrasVertex.DjikstrasDistanceLabel >=
                                    newDistanceLabel)
                                {
                                    //lock (StandardDjikstraQueue)
                                    //{
                                      //  StandardDjikstraQueue.Remove(currentVertexInStandardQueue);
                                    //}
                                    lock (ExactVerticesQueue)
                                    {
                                        ExactVerticesQueue.Add(currentVertexInStandardQueue);
                                    }
                                        
                                    
                                    
                                    
                                    //currentVertexInStandardQueue = null;
                                }
                            }
                        }
                    }
                });
                
            }
        

        //add the new lowest distance label item to the queue
            else
            {
                ExactVerticesQueue.Add(StandardDjikstraQueue.GetFirst());
                StandardDjikstraQueue.RemoveFirst();
            }
 

        }














        private void ComputeSingleSourceNonNegativeShortestPathEarlyTermination()
        {

            if (EarlyTerminationExactVerticesQueue.Count > 0)
            {

                //this is getting overwritten each time....
                //targetDjikstrasQueryObjects = new List<DjikstrasQueryResult>();
                Parallel.ForEach(EarlyTerminationExactVerticesQueue.ToList(), currentDjikstrasVertex =>
                //foreach(DjikstraEarlyTerminationVertexProperties currentDjikstrasVertex in EarlyTerminationExactVerticesQueue)
                {
                    IEnumerable<TaggedEdge<VertexProperties, EdgeProperties>> outEdges;
                    bool areOutEdgesPresent = false;
                    areOutEdgesPresent = Graph.TryGetOutEdges(currentDjikstrasVertex.Vertex.Vertex, out outEdges);
                    if (areOutEdgesPresent && outEdges.Any())
                    {

                        var targetDjikstrasQueryObjectsTemp =
                            (from edge in outEdges
                             from djikstrasvertex in EarlyTerminationDjikstraQueue
                             from targetVertexCaliber in VertexCaliberDictionary
                             where edge.Target.Equals(djikstrasvertex.Vertex.Vertex)
                                   && targetVertexCaliber.Key.Equals(edge.Target)
                                   && !edge.Equals(ConstraintEdge)
                             select
                                 new EarlyTerminationDjikstraQueryResult(targetVertexCaliber.Value, currentDjikstrasVertex,
                                     djikstrasvertex, edge.Tag)).ToList();

                        //now enumerate over the query results and make changes as necessary
                        lock (EarlyTerminationTargetDjikstrasQueryObjects)
                        {
                            EarlyTerminationTargetDjikstrasQueryObjects =
                                EarlyTerminationTargetDjikstrasQueryObjects.Concat(targetDjikstrasQueryObjectsTemp).ToList();
                        }

                    }

                    lock (DjikstrasShortestPaths)
                    {
                        if (!DjikstrasShortestPaths.ContainsKey(currentDjikstrasVertex.Vertex.Vertex))
                            DjikstrasShortestPaths.Add(currentDjikstrasVertex.Vertex.Vertex,
                                currentDjikstrasVertex.Vertex.DjikstrasDistanceLabel);
                    }
                });



                EarlyTerminationExactVerticesQueue.Clear();
                if (EarlyTerminationTargetDjikstrasQueryObjects.Count == 0)
                    return;

                Parallel.ForEach(EarlyTerminationTargetDjikstrasQueryObjects, currentDjikstraQueryObject =>

                {

                    VertexProperties lockObject;
                    lock (EarlyTerminationDjikstraQueue)
                    {
                        lockObject =
                            (from djikstrasVertex in EarlyTerminationDjikstraQueue.ToList()
                                where
                                    djikstrasVertex.Vertex.Vertex.Equals(
                                        currentDjikstraQueryObject.TargetDjikstrasVertex.Vertex.Vertex)
                                select djikstrasVertex.Vertex.Vertex).FirstOrDefault();
                    }
                    //must already be exact or not worth looking at...
                    if (lockObject == null)
                        return;
                    lock (lockObject)
                    {

                        if (currentDjikstraQueryObject.SourceDjikstrasVertex.Vertex.DjikstrasDistanceLabel +
                            currentDjikstraQueryObject.Edge.Slack
                            <= currentDjikstraQueryObject.TargetDjikstrasVertex.Vertex.DjikstrasDistanceLabel)
                        {



                            int newDistanceLabel =
                                currentDjikstraQueryObject.SourceDjikstrasVertex.Vertex.DjikstrasDistanceLabel +
                                currentDjikstraQueryObject.Edge.Slack;
                            DjikstraEarlyTerminationVertexProperties currentVertexInStandardQueue;

                            lock (EarlyTerminationDjikstraQueue)
                            {
                                currentVertexInStandardQueue =
                                    (from djikstrasVertex in EarlyTerminationDjikstraQueue.ToList()
                                        where
                                            djikstrasVertex.Vertex.Vertex.Equals(
                                                currentDjikstraQueryObject.TargetDjikstrasVertex.Vertex.Vertex)
                                        select djikstrasVertex).FirstOrDefault();
                            }
                            //synchronize the update to avoid inconsistent values.
                            if (currentVertexInStandardQueue != null &&
                                !EarlyTerminationExactVerticesQueue.ToList().Contains(currentVertexInStandardQueue))
                                //some other thread might have removed currentVertexInStandarQueue
                            {
                                /////////--------------------------------THIS IS WHAT IM CHANGING------------------------------//////////
                                lock (EarlyTerminationDjikstraQueue)
                                {
                                    ////    int x= StandardDjikstraQueue.IndexOf(currentVertexInStandardQueue);
                                    EarlyTerminationDjikstraQueue.Remove(currentVertexInStandardQueue);
                                }
                                /////////--------------------------------THIS IS WHAT IM CHANGING------------------------------//////////

                                lock (currentVertexInStandardQueue)
                                {


                                    currentVertexInStandardQueue.Vertex.SetDjikstrasDistanceLabel(newDistanceLabel);
                                    currentVertexInStandardQueue.Vertex.SetParentVertex(
                                        currentDjikstraQueryObject.SourceDjikstrasVertex.Vertex.Vertex);
                                    currentVertexInStandardQueue.SetIsRelevant(
                                        currentDjikstraQueryObject.SourceDjikstrasVertex.IsRelevant);

                                    lock (EarlyTerminationDjikstraQueue)
                                    {
                                        EarlyTerminationDjikstraQueue.Add(currentVertexInStandardQueue);
                                    }

                                    //.TargetDjikstrasVertex.SetDjikstrasDistanceLabel(newDistanceLabel);
                                    //currentDjikstraQueryObject.TargetDjikstrasVertex.SetParentVertex(currentDjikstraQueryObject.SourceDjikstrasVertex.Vertex);

                                    if (currentDjikstraQueryObject.Caliber +
                                        currentDjikstraQueryObject.SourceDjikstrasVertex.Vertex.DjikstrasDistanceLabel >=
                                        newDistanceLabel && currentVertexInStandardQueue.IsRelevant)
                                    {
                                        //lock (StandardDjikstraQueue)
                                        //{
                                        //  StandardDjikstraQueue.Remove(currentVertexInStandardQueue);
                                        //}
                                        lock (EarlyTerminationExactVerticesQueue)
                                        {
                                            EarlyTerminationExactVerticesQueue.Add(currentVertexInStandardQueue);
                                        }
                                        lock (EarlyTerminationDjikstraQueue)
                                        {
                                            EarlyTerminationDjikstraQueue.Remove(currentVertexInStandardQueue);
                                        }




                                        //currentVertexInStandardQueue = null;
                                    }
                                }
                            }
                        }
                    }
                });

            }


        //add the new lowest distance label item to the queue
            else
            {                
                if (EarlyTerminationDjikstraQueue.GetFirst().IsRelevant)
                {
                    
                    EarlyTerminationExactVerticesQueue.Add(EarlyTerminationDjikstraQueue.GetFirst());
                    EarlyTerminationDjikstraQueue.RemoveFirst();
                }

            }

        }
















        
        
        
        
        
        
        
        private void ComputeSingleSourceNonNegativeShortestPath_EdgeTracking(VertexProperties sourceVertex)
        {
            if (ExactVerticesQueue.Count > 0)
            {

                //this is getting overwritten each time....
                //targetDjikstrasQueryObjects = new List<DjikstrasQueryResult>();
                Parallel.ForEach(ExactVerticesQueue.ToList(), currentDjikstrasVertex =>
                {
                    IEnumerable<TaggedEdge<VertexProperties, EdgeProperties>> outEdges;
                    bool areOutEdgesPresent = false;
                    areOutEdgesPresent = Graph.TryGetOutEdges(currentDjikstrasVertex.Vertex, out outEdges);
                    if (areOutEdgesPresent && outEdges.ToList().Count > 0)
                    {

                        var targetDjikstrasQueryObjectsTemp =
                            (from edge in outEdges
                             from djikstrasvertex in StandardDjikstraQueue
                             from targetVertexCaliber in VertexCaliberDictionary
                             where edge.Target.Equals(djikstrasvertex.Vertex)
                                   && targetVertexCaliber.Key.Equals(edge.Target)
                             select
                                 new DjikstrasQueryResult(targetVertexCaliber.Value, currentDjikstrasVertex,
                                     djikstrasvertex, edge.Tag)).ToList();

                        //now enumerate over the query results and make changes as necessary
                        lock (TargetDjikstrasQueryObjects)
                        {
                            TargetDjikstrasQueryObjects =
                                TargetDjikstrasQueryObjects.Concat(targetDjikstrasQueryObjectsTemp).ToList();
                        }

                    }

                    lock (DjikstrasShortestPathsEdgeTracking)
                    {
                        if (!DjikstrasShortestPathsEdgeTracking.ContainsKey(currentDjikstrasVertex.Vertex))
                            DjikstrasShortestPathsEdgeTracking.Add(currentDjikstrasVertex.Vertex,
                                new DjikstrasEdgeTrackObject(currentDjikstrasVertex.DjikstrasDistanceLabel,currentDjikstrasVertex.NumberOfEdges));
                    }
                });



                ExactVerticesQueue.Clear();
                if (TargetDjikstrasQueryObjects.Count == 0)
                    return;

                Parallel.ForEach(TargetDjikstrasQueryObjects, currentDjikstraQueryObject =>
                {

                    if ((currentDjikstraQueryObject.SourceDjikstrasVertex.DjikstrasDistanceLabel +
                        currentDjikstraQueryObject.Edge.Slack
                        < currentDjikstraQueryObject.TargetDjikstrasVertex.DjikstrasDistanceLabel) ||
                        (currentDjikstraQueryObject.SourceDjikstrasVertex.DjikstrasDistanceLabel +
                        currentDjikstraQueryObject.Edge.Slack
                        == currentDjikstraQueryObject.TargetDjikstrasVertex.DjikstrasDistanceLabel && currentDjikstraQueryObject.SourceDjikstrasVertex.NumberOfEdges + 1 < currentDjikstraQueryObject.TargetDjikstrasVertex.NumberOfEdges ))
                    {



                        int newDistanceLabel =
                            currentDjikstraQueryObject.SourceDjikstrasVertex.DjikstrasDistanceLabel +
                            currentDjikstraQueryObject.Edge.Slack;


                        var currentVertexInStandardQueue =
                            (from djikstrasVertex in StandardDjikstraQueue.ToList()
                             where
                                 djikstrasVertex.Vertex.Equals(
                                     currentDjikstraQueryObject.TargetDjikstrasVertex.Vertex)
                             select djikstrasVertex).FirstOrDefault();

                        //synchronize the update to avoid inconsistent values.
                        if (currentVertexInStandardQueue != null &&
                            !ExactVerticesQueue.ToList().Contains(currentVertexInStandardQueue)) //some other thread might have removed currentVertexInStandarQueue
                        {
                            lock (currentVertexInStandardQueue)
                            {
                                currentVertexInStandardQueue.SetDjikstrasDistanceLabel(newDistanceLabel);
                                currentVertexInStandardQueue.SetParentVertex(
                                    currentDjikstraQueryObject.SourceDjikstrasVertex.Vertex);
                                currentVertexInStandardQueue.SetNumberOfEdges(currentDjikstraQueryObject.SourceDjikstrasVertex.NumberOfEdges+1);


                                //.TargetDjikstrasVertex.SetDjikstrasDistanceLabel(newDistanceLabel);
                                //currentDjikstraQueryObject.TargetDjikstrasVertex.SetParentVertex(currentDjikstraQueryObject.SourceDjikstrasVertex.Vertex);

                                if (currentDjikstraQueryObject.Caliber +
                                    currentDjikstraQueryObject.SourceDjikstrasVertex.DjikstrasDistanceLabel >=
                                    newDistanceLabel)
                                {
                                    //lock (StandardDjikstraQueue)
                                    //{
                                    //  StandardDjikstraQueue.Remove(currentVertexInStandardQueue);
                                    //}
                                    lock (ExactVerticesQueue)
                                    {
                                        ExactVerticesQueue.Add(currentVertexInStandardQueue);
                                    }




                                    //currentVertexInStandardQueue = null;
                                }
                            }
                        }
                    }
                });

            }


        //add the new lowest distance label item to the queue
            else
            {
                ExactVerticesQueue.Add(StandardDjikstraQueue.GetFirst());
                StandardDjikstraQueue.RemoveFirst();
            }


        }












        
        private void InitializeStandardDjikstraQueue(VertexProperties sourceVertex)
        {
            //Parallel.ForEach(Graph.Vertices, currentVertex =>            
            foreach (VertexProperties currentVertex in Graph.Vertices)            
            {
                ////////////////--------------ANOTHER CHANGE IM MAKING-------------------////////////////////
                IEnumerable<TaggedEdge<VertexProperties, EdgeProperties>> outEdges;
                Graph.TryGetOutEdges(currentVertex, out outEdges);
                foreach (var currentOutEdge in outEdges)
                {
                    //if (
                    //    !StandardDjikstraQueue.Contains(new DjikstrasVertexProperties(currentOutEdge.Target, null,
                    //        int.MaxValue)))
                        DjikstrasVertexProperties checkForContained=StandardDjikstraQueue.AsParallel().FirstOrDefault(p => p.Vertex.Equals(currentOutEdge.Target));
                    if(checkForContained==null)    
                    StandardDjikstraQueue.Add(new DjikstrasVertexProperties(currentOutEdge.Target, null, int.MaxValue));                                                       
                }

                ////////////////--------------ANOTHER CHANGE IM MAKING-------------------////////////////////
                //if(!currentVertex.Equals(sourceVertex))
                //    StandardDjikstraQueue.Add(new DjikstrasVertexProperties(currentVertex, null, int.MaxValue));                                                       
            }
                    
        }


        private void InitializeEarlyTerminationDjikstraQueue(VertexProperties sourceVertex)
        {

            {
                //Parallel.ForEach(Graph.Vertices, currentVertex =>            
                foreach (VertexProperties currentVertex in Graph.Vertices)
                {
                    ////////////////--------------ANOTHER CHANGE IM MAKING-------------------////////////////////
                    IEnumerable<TaggedEdge<VertexProperties, EdgeProperties>> outEdges;
                    Graph.TryGetOutEdges(currentVertex, out outEdges);
                    foreach (var currentOutEdge in outEdges)
                    {
                        if (!sourceVertex.Equals(currentOutEdge.Target) && !currentOutEdge.Target.Equals(ConstraintEdge.Source))
                        {

                            var containedVertex = EarlyTerminationDjikstraQueue.AsParallel().FirstOrDefault
                                (p => p.Vertex.Vertex.Equals(currentOutEdge.Target));
                                    
                                //if (
                                //!EarlyTerminationDjikstraQueue.Contains(
                                //    new DjikstraEarlyTerminationVertexProperties(
                                //        new DjikstrasVertexProperties(currentOutEdge.Target, null,
                                //            int.MaxValue), false)))
                            if(containedVertex==null)
                                EarlyTerminationDjikstraQueue.Add(
                                    new DjikstraEarlyTerminationVertexProperties(
                                        new DjikstrasVertexProperties(currentOutEdge.Target, null,
                                            int.MaxValue), false));

                        }
                        
                    }
                }

            }
        }


        #endregion
    }
}
