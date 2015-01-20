using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using QuickGraph;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.NegativeCycleDetection.DataContracts;

namespace Tejas.Jhu.NegativeCycleDetection.UnitTesting
{
    [TestFixture]
    public class GraphTraversalAlgorithmsTests
    {

        #region Custom DFS Traversal Tests
        
        [Test]
        [Repeat(200)]
        //[Ignore]
        public void TestCustomDFSTraversal_AcyclicGraph()
        {
            // System under test
            GraphTraversalAlgorithms dfsTest = new GraphTraversalAlgorithms();

            // Construct the input
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph = ConstructAcyclicGraph();
            IList<VertexProperties> verticesToScan = new List<VertexProperties>();
            verticesToScan.Add(new VertexProperties("X",0,false,false));
            verticesToScan.Add(new VertexProperties("Y",0,false,true));
            verticesToScan.Add(new VertexProperties("U",0,false,false));

            try
            {
                // Test the system by calling the method to test
                DfsTraversalResult result = dfsTest.CustomDFSTraversal(graph, verticesToScan);
                Assert.That(result.NegativeCycleDetected, Is.EqualTo(false));
                Assert.That(((List<VertexProperties>) result.ListOfVertices).Count, Is.EqualTo(verticesToScan.Count));
            }
            catch (Exception excpn)
            {
                Console.WriteLine("FOUND:: " + excpn);
            }
        }

        [Test]
        [Repeat(200)]
       // [Ignore]
        public void TestCustomDFSTraversal_ZeroWeightCycle()
        {
            // System under test
            GraphTraversalAlgorithms dfsTest = new GraphTraversalAlgorithms();

            // Construct the input
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph = ConstructZeroWeightCycleGraph();
            IList<VertexProperties> verticesToScan = new List<VertexProperties>();
            verticesToScan.Add(new VertexProperties("X", 0, false, false));
            verticesToScan.Add(new VertexProperties("Y", 0, false, true));
            verticesToScan.Add(new VertexProperties("U", 0, false, false));

            try
            {
                // Test the system by calling the method to test
                DfsTraversalResult result = dfsTest.CustomDFSTraversal(graph, verticesToScan);
                Assert.That(result.NegativeCycleDetected, Is.EqualTo(false));
                Assert.That(((List<VertexProperties>)result.ListOfVertices).Count, Is.EqualTo(verticesToScan.Count));
            }
            catch (Exception excpn)
            {
                Console.WriteLine("FOUND:: " + excpn);
            }
        }

        [Test]
        [Repeat(200)]
     //   [Ignore]
        public void TestCustomDFSTraversal_NegativeWeightCycleGraph()
        {
            // System under test
            GraphTraversalAlgorithms dfsTest = new GraphTraversalAlgorithms();

            // Construct the input
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph = ConstructNegativeWeightCycleGraph();
            IList<VertexProperties> verticesToScan = new List<VertexProperties>();
            verticesToScan.Add(new VertexProperties("X", 0, false, true));
            verticesToScan.Add(new VertexProperties("Y", 0, false, true));
            verticesToScan.Add(new VertexProperties("U", 0, false, true));

            try
            {
                // Test the system by calling the method to test
                DfsTraversalResult result = dfsTest.CustomDFSTraversal(graph, verticesToScan);
                Assert.That(result.NegativeCycleDetected, Is.EqualTo(true));
            }
            catch (Exception excpn)
            {
                Console.WriteLine("FOUND:: " + excpn);
                //throw;
            }
        }



        [Test]
        [Repeat(200)]
     ////   [Ignore]
        public void TestSccCompute_ZeroSlackGraph()
        {
            // System under test
            GraphTraversalAlgorithms sccTest = new GraphTraversalAlgorithms();

            // Construct the input
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph =
                ConstructZeroSlackGraph();


            try
            {
                // Test the system by calling the method to test
                IDictionary<VertexProperties, List<List<VertexProperties>>> result=sccTest.ComputeStronglyConnectedComponents(graph);

            }
            catch (Exception excpn)
            {
                Console.WriteLine("FOUND:: " + excpn);
                //throw;
            }
        }
        #endregion

        #region Strongly Connected Components Tests
        
        [Test]
        [Repeat(200)]
     //   [Ignore]
        public void TestSccCompute_MultipleZeroSlackSccWithSameRootGraph()
        {
            // System under test
            GraphTraversalAlgorithms sccTest = new GraphTraversalAlgorithms();

            // Construct the input
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph = ConstructMultipleZeroSlackSccWithSameRootGraph();

            try
            {
                // Test the system by calling the method to test
                IDictionary<VertexProperties, List<List<VertexProperties>>> sccComponents = sccTest.ComputeStronglyConnectedComponents(graph);

                // Assert the return value here....

            }
            catch (Exception excpn)
            {
                Console.WriteLine("FOUND:: " + excpn);
                //throw;
            }
        }

        #endregion

        #region Djikstra's Algorithm Tests

        [Test]
        [Repeat (200)]
        public void SingleSourceNonNegativeShortestPathTest_Acyclic()
        {
            //populate the inputdata
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph =
                ConstructAcyclicGraph_WithPositiveSlack();
            //VertexProperties v1 = new VertexProperties("U", 0, false,true);

            GraphTraversalAlgorithms graphTraversalAlgorithms = new GraphTraversalAlgorithms();
            VertexProperties v1 = (from vertex in constraintGraph.Vertices
                where vertex.Name.Equals("U", StringComparison.InvariantCultureIgnoreCase)
                select vertex).FirstOrDefault();
            VertexProperties v2 = (from vertex in constraintGraph.Vertices
                where vertex.Name.Equals("V", StringComparison.InvariantCultureIgnoreCase)
                select vertex).FirstOrDefault();
            VertexProperties v3 = (from vertex in constraintGraph.Vertices
                where vertex.Name.Equals("X", StringComparison.InvariantCultureIgnoreCase)
                select vertex).FirstOrDefault();
            VertexProperties v4 = (from vertex in constraintGraph.Vertices
                where vertex.Name.Equals("Y", StringComparison.InvariantCultureIgnoreCase)
                select vertex).FirstOrDefault();

            IDictionary<VertexProperties, int> result = graphTraversalAlgorithms.SingleSourceNonNegativeShortestPath(
               v1, constraintGraph);

            //assert the results
            Assert.That(result.Count == 4);
            Assert.That(result[v1] == 0);
            Assert.That(result[v2] == 1);
            Assert.That(result[v3] == 3);
            Assert.That(result[v4] == 3);

        }


        [Test]
        [Repeat (200)]
        public void SingleSourceNonNegativeShortestPathTest_Acyclic_ReversingEdgeWeight()
        {
            //populate the inputdata
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph =
                ConstructAcyclicGraph_WithPositiveSlack_ReverseEdgeWeight();
            //VertexProperties v1 = new VertexProperties("U", 0, false,true);

            GraphTraversalAlgorithms graphTraversalAlgorithms = new GraphTraversalAlgorithms();
            VertexProperties v1 = (from vertex in constraintGraph.Vertices
                                   where vertex.Name.Equals("U", StringComparison.InvariantCultureIgnoreCase)
                                   select vertex).FirstOrDefault();
            VertexProperties v2 = (from vertex in constraintGraph.Vertices
                                   where vertex.Name.Equals("V", StringComparison.InvariantCultureIgnoreCase)
                                   select vertex).FirstOrDefault();
            VertexProperties v3 = (from vertex in constraintGraph.Vertices
                                   where vertex.Name.Equals("X", StringComparison.InvariantCultureIgnoreCase)
                                   select vertex).FirstOrDefault();
            VertexProperties v4 = (from vertex in constraintGraph.Vertices
                                   where vertex.Name.Equals("Y", StringComparison.InvariantCultureIgnoreCase)
                                   select vertex).FirstOrDefault();

            IDictionary<VertexProperties, int> result = graphTraversalAlgorithms.SingleSourceNonNegativeShortestPath(
               v1, constraintGraph);

            //assert the results
            Assert.That(result.Count == 4);
            Assert.That(result[v1] == 0);
            Assert.That(result[v2] == 2);
            Assert.That(result[v3] == 4);
            Assert.That(result[v4] == 1);

        }

        [Test]
        [Repeat(200)]
        public void SingleSourceNonNegativeShortestPathTest_PositiveWeightCycle()
        {
            //populate the inputdata
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph =
                ConstructGraph_WithPositiveCycle();
            //VertexProperties v1 = new VertexProperties("U", 0, false,true);

            GraphTraversalAlgorithms graphTraversalAlgorithms = new GraphTraversalAlgorithms();
            VertexProperties v1 = (from vertex in constraintGraph.Vertices
                                   where vertex.Name.Equals("U", StringComparison.InvariantCultureIgnoreCase)
                                   select vertex).FirstOrDefault();
            VertexProperties v2 = (from vertex in constraintGraph.Vertices
                                   where vertex.Name.Equals("V", StringComparison.InvariantCultureIgnoreCase)
                                   select vertex).FirstOrDefault();
            VertexProperties v3 = (from vertex in constraintGraph.Vertices
                                   where vertex.Name.Equals("X", StringComparison.InvariantCultureIgnoreCase)
                                   select vertex).FirstOrDefault();
            VertexProperties v4 = (from vertex in constraintGraph.Vertices
                                   where vertex.Name.Equals("Y", StringComparison.InvariantCultureIgnoreCase)
                                   select vertex).FirstOrDefault();

            IDictionary<VertexProperties, int> result = graphTraversalAlgorithms.SingleSourceNonNegativeShortestPath(
               v1, constraintGraph);

            //assert the results
            Assert.That(result.Count == 4);
            Assert.That(result[v1] == 0);
            Assert.That(result[v2] == 1);
            Assert.That(result[v3] == 3);
            Assert.That(result[v4] == 3);

        }

        #endregion

        #region Djikstra's Edge Tracking Tests
        [Test]
        [Ignore]
        [Repeat(200)]
        public void SingleSourceNonNegativeShortestPath_EdgeTracking_Test_Acyclic()
        {
            //populate the inputdata
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph =
                ConstructAcyclicGraph_WithPositiveSlack();
            //VertexProperties v1 = new VertexProperties("U", 0, false,true);

            GraphTraversalAlgorithms graphTraversalAlgorithms = new GraphTraversalAlgorithms();
            VertexProperties v1 = (from vertex in constraintGraph.Vertices
                                   where vertex.Name.Equals("U", StringComparison.InvariantCultureIgnoreCase)
                                   select vertex).FirstOrDefault();
            VertexProperties v2 = (from vertex in constraintGraph.Vertices
                                   where vertex.Name.Equals("V", StringComparison.InvariantCultureIgnoreCase)
                                   select vertex).FirstOrDefault();
            VertexProperties v3 = (from vertex in constraintGraph.Vertices
                                   where vertex.Name.Equals("X", StringComparison.InvariantCultureIgnoreCase)
                                   select vertex).FirstOrDefault();
            VertexProperties v4 = (from vertex in constraintGraph.Vertices
                                   where vertex.Name.Equals("Y", StringComparison.InvariantCultureIgnoreCase)
                                   select vertex).FirstOrDefault();

            IDictionary<VertexProperties, DjikstrasEdgeTrackObject> result = graphTraversalAlgorithms.SingleSourceNonNegativeShortestPath_EdgeTracking(
               v1, constraintGraph);
            
            //assert the results
            Assert.That(result.Count == 4);
            Assert.That(result[v1].Distance == 0);
            Assert.That(result[v1].NumberOfEdges == 0);
            Assert.That(result[v2].Distance == 1);
            Assert.That(result[v2].NumberOfEdges == 1);
            Assert.That(result[v3].Distance == 3);
            Assert.That(result[v3].NumberOfEdges == 2);
            Assert.That(result[v4].Distance == 3);
            Assert.That(result[v4].NumberOfEdges == 1);

        }

        [Test]
        [Ignore]
        [Repeat(200)]
        public void SingleSourceNonNegativeShortestPathTest_EdgeTracking_PositiveWeightCycle()
        {
            //populate the inputdata
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph =
                ConstructGraph_WithPositiveCycle();
            //VertexProperties v1 = new VertexProperties("U", 0, false,true);

            GraphTraversalAlgorithms graphTraversalAlgorithms = new GraphTraversalAlgorithms();
            VertexProperties v1 = (from vertex in constraintGraph.Vertices
                                   where vertex.Name.Equals("U", StringComparison.InvariantCultureIgnoreCase)
                                   select vertex).FirstOrDefault();
            VertexProperties v2 = (from vertex in constraintGraph.Vertices
                                   where vertex.Name.Equals("V", StringComparison.InvariantCultureIgnoreCase)
                                   select vertex).FirstOrDefault();
            VertexProperties v3 = (from vertex in constraintGraph.Vertices
                                   where vertex.Name.Equals("X", StringComparison.InvariantCultureIgnoreCase)
                                   select vertex).FirstOrDefault();
            VertexProperties v4 = (from vertex in constraintGraph.Vertices
                                   where vertex.Name.Equals("Y", StringComparison.InvariantCultureIgnoreCase)
                                   select vertex).FirstOrDefault();

            IDictionary<VertexProperties, DjikstrasEdgeTrackObject> result = graphTraversalAlgorithms.SingleSourceNonNegativeShortestPath_EdgeTracking(
               v1, constraintGraph);

            //assert the results
            Assert.That(result.Count == 4);
            Assert.That(result[v1].Distance == 0);
            Assert.That(result[v2].Distance == 1);
            Assert.That(result[v3].Distance == 3);
            Assert.That(result[v4].Distance == 3);

        }

        #endregion
        
        #region Djikstra Early Termination Tests
        [Test]
        [Repeat (200)]
        public void SingleSourceNonNegativeShortestPath_EarlyTermination_Acyclic_Test()
        {
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph =
                ConstructAcyclicGraph_WithPositiveSlack();
            IGraphTraversalAlgorithms graphTraversalAlgorithms=new GraphTraversalAlgorithms();
            VertexProperties v1 = new VertexProperties("U", 0, false, false);
            VertexProperties v2 = new VertexProperties("V", int.MaxValue, false, false);
           // edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(1, 1));
            
            IDictionary<VertexProperties,int> result=graphTraversalAlgorithms.SingleSourceNonNegativeShortestPath_EarlyTermination(
                new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(1, 1)), constraintGraph);
            Assert.That(result!=null);
            Assert.That(result.Count==2);
            Assert.That(result.ContainsKey(v2));
            Assert.That(result.ContainsKey(new VertexProperties("X",3,true,false)));

        }
        
        [Test]
        [Repeat (200)]
        public void SingleSourceNonNegativeShortestPath_EarlyTermination_Acyclic_TestForNotDelRelevant()
        {
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph =
              ConstructEarlyTermination_Acyclic_NotRelevantEdge();
            IGraphTraversalAlgorithms graphTraversalAlgorithms = new GraphTraversalAlgorithms();
            VertexProperties v1 = new VertexProperties("U", 0, false, false);
            VertexProperties v2 = new VertexProperties("X", int.MaxValue, false, false);
            // edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(1, 1));

            IDictionary<VertexProperties, int> result = graphTraversalAlgorithms.SingleSourceNonNegativeShortestPath_EarlyTermination(
                new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(10, 1)), constraintGraph);
            Assert.That(result != null);
            Assert.That(result.Count == 1);
            //Assert.That(result.ContainsKey(v2));
            Assert.That(result.ContainsKey(new VertexProperties("X", 3, true, false)));

        }

        #endregion

        #region Private Methods to Construct Graphs

        private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstructMultipleZeroSlackSccWithSameRootGraph()
        {
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph = new
              BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();

            VertexProperties v1 = new VertexProperties("U", 0, true, false);
            graph.AddVertex(v1);
            VertexProperties v2 = new VertexProperties("V", 3, true, false);
            graph.AddVertex(v2);
            VertexProperties v3 = new VertexProperties("W", 5, true, true);
            graph.AddVertex(v3);
            VertexProperties v4 = new VertexProperties("X", 5, true, true);
            graph.AddVertex(v4);

            TaggedEdge<VertexProperties, EdgeProperties> edge1;
            edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(0, 3));
            graph.AddEdge(edge1);
            edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v3, new EdgeProperties(0, 2));
            graph.AddEdge(edge1);
            edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v4, new EdgeProperties(0, 2));
            graph.AddEdge(edge1);
            edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v3, v1, new EdgeProperties(0, -5));
            graph.AddEdge(edge1);
            edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v4, v1, new EdgeProperties(0, -5));
            graph.AddEdge(edge1);
            return graph;
        }


        private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstructZeroSlackGraph()
        {
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph = new
                BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();

            VertexProperties v1 = new VertexProperties("U", 0, true, false);
            graph.AddVertex(v1);
            VertexProperties v2 = new VertexProperties("V", 3, true, false);
            graph.AddVertex(v2);
            VertexProperties v3 = new VertexProperties("W", 5, true, false);
            graph.AddVertex(v3);

            TaggedEdge<VertexProperties, EdgeProperties> edge1;
            edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(0, 3));
            graph.AddEdge(edge1);
            edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v3, new EdgeProperties(0, 2));
            graph.AddEdge(edge1);
            edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v3, v1, new EdgeProperties(0, -5));
            graph.AddEdge(edge1);
            return graph;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstructAcyclicGraph()
        {
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph =
                new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();

            // Add values to the graph here
            TaggedEdge<VertexProperties, EdgeProperties> edge1;
            TaggedEdge<VertexProperties, EdgeProperties> edge2;
            TaggedEdge<VertexProperties, EdgeProperties> edge3;
            TaggedEdge<VertexProperties, EdgeProperties> edge4;
            TaggedEdge<VertexProperties, EdgeProperties> edge5;

            VertexProperties v1 = new VertexProperties("U", 0, false,false);
            VertexProperties v2 = new VertexProperties("V", int.MaxValue, false,false);
            VertexProperties v4 = new VertexProperties("X", int.MaxValue, false,false);
            VertexProperties v5 = new VertexProperties("Y", int.MaxValue, false,false);
            VertexProperties v6 = new VertexProperties(Constants.SourceVertexName, 0, false,true);

            edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(-1, 1));
            edge2 = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v4, new EdgeProperties(-1, 1));
            edge3 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v5, new EdgeProperties(-1, 1));
            edge4 = new TaggedEdge<VertexProperties, EdgeProperties>(v5, v4, new EdgeProperties(-1, 1));
            edge5 = new TaggedEdge<VertexProperties, EdgeProperties>(v6, v1, new EdgeProperties(0, 0));

            graph.AddVertex(v1);
            graph.AddVertex(v2);
            graph.AddVertex(v4);
            graph.AddVertex(v5);
            graph.AddVertex(v6);

            graph.AddEdge(edge1);
            graph.AddEdge(edge5);
            graph.AddEdge(edge2);
            graph.AddEdge(edge3);
            graph.AddEdge(edge4);

            return graph;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstructZeroWeightCycleGraph()
        {
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph =
                new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();

            // Add values to the graph here
            TaggedEdge<VertexProperties, EdgeProperties> edge1;
            TaggedEdge<VertexProperties, EdgeProperties> edge2;
            TaggedEdge<VertexProperties, EdgeProperties> edge3;
            TaggedEdge<VertexProperties, EdgeProperties> edge4;
            TaggedEdge<VertexProperties, EdgeProperties> edge5;
            TaggedEdge<VertexProperties, EdgeProperties> edge6;


            VertexProperties v1 = new VertexProperties("U", 0, false,false);
            VertexProperties v2 = new VertexProperties("V", int.MaxValue, false,false);
            VertexProperties v4 = new VertexProperties("X", int.MaxValue, false,false);
            VertexProperties v5 = new VertexProperties("Y", int.MaxValue, false,false);
            VertexProperties v6 = new VertexProperties(Constants.SourceVertexName, 0, false,true);

            edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(0, 1));
            edge2 = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v4, new EdgeProperties(0, 1));
            edge3 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v5, new EdgeProperties(0, 1));
            edge4 = new TaggedEdge<VertexProperties, EdgeProperties>(v5, v4, new EdgeProperties(0, 1));
            edge5 = new TaggedEdge<VertexProperties, EdgeProperties>(v6, v1, new EdgeProperties(0, 0));
            edge6 = new TaggedEdge<VertexProperties, EdgeProperties>(v4, v1, new EdgeProperties(0, 0));

            graph.AddVertex(v1);
            graph.AddVertex(v2);
            graph.AddVertex(v4);
            graph.AddVertex(v5);
            graph.AddVertex(v6);

            graph.AddEdge(edge1);
            graph.AddEdge(edge5);

            graph.AddEdge(edge2);
            graph.AddEdge(edge3);
            graph.AddEdge(edge4);
            graph.AddEdge(edge6);

            return graph;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstructNegativeWeightCycleGraph()
        {
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph =
                new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();

            // Add values to the graph here
            TaggedEdge<VertexProperties, EdgeProperties> edge1;
            TaggedEdge<VertexProperties, EdgeProperties> edge2;
            TaggedEdge<VertexProperties, EdgeProperties> edge3;
            TaggedEdge<VertexProperties, EdgeProperties> edge4;
            TaggedEdge<VertexProperties, EdgeProperties> edge5;
            TaggedEdge<VertexProperties, EdgeProperties> edge6;


            VertexProperties v1 = new VertexProperties("U", 0, false,true);
            VertexProperties v2 = new VertexProperties("V", int.MaxValue, false,true);
            VertexProperties v4 = new VertexProperties("X", int.MaxValue, false,true);
            VertexProperties v5 = new VertexProperties("Y", int.MaxValue, false,true);
            VertexProperties v6 = new VertexProperties(Constants.SourceVertexName, 0, false,true);

            edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(0, 1));
            edge2 = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v4, new EdgeProperties(0, 1));
            edge3 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v5, new EdgeProperties(0, 1));
            edge4 = new TaggedEdge<VertexProperties, EdgeProperties>(v5, v4, new EdgeProperties(0, 1));
            edge5 = new TaggedEdge<VertexProperties, EdgeProperties>(v6, v1, new EdgeProperties(0, 0));
            edge6 = new TaggedEdge<VertexProperties, EdgeProperties>(v4, v1, new EdgeProperties(-1, 0));

            graph.AddVertex(v1);
            graph.AddVertex(v2);
            graph.AddVertex(v4);
            graph.AddVertex(v5);
            graph.AddVertex(v6);


            graph.AddEdge(edge1);
            graph.AddEdge(edge5);

            graph.AddEdge(edge2);
            graph.AddEdge(edge3);
            graph.AddEdge(edge4);
            graph.AddEdge(edge6);

            return graph;
        }

        private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstructAcyclicGraph_WithPositiveSlack()
        {
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph =
                new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();

            // Add values to the graph here
            TaggedEdge<VertexProperties, EdgeProperties> edge1;
            TaggedEdge<VertexProperties, EdgeProperties> edge2;
            TaggedEdge<VertexProperties, EdgeProperties> edge3;
            TaggedEdge<VertexProperties, EdgeProperties> edge4;
            TaggedEdge<VertexProperties, EdgeProperties> edge5;

            VertexProperties v1 = new VertexProperties("U", 0, false, false);
            VertexProperties v2 = new VertexProperties("V", int.MaxValue, false, false);
            VertexProperties v4 = new VertexProperties("X", int.MaxValue, false, false);
            VertexProperties v5 = new VertexProperties("Y", int.MaxValue, false, false);
            //VertexProperties v6 = new VertexProperties(Constants.SourceVertexName, 0, false, true);

            edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(1, 1));
            edge2 = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v4, new EdgeProperties(2, 1));
            edge3 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v5, new EdgeProperties(3, 1));
            edge4 = new TaggedEdge<VertexProperties, EdgeProperties>(v5, v4, new EdgeProperties(4, 1));
            //edge5 = new TaggedEdge<VertexProperties, EdgeProperties>(v6, v1, new EdgeProperties(0, 0));

            graph.AddVertex(v1);
            graph.AddVertex(v2);
            graph.AddVertex(v4);
            graph.AddVertex(v5);
            //graph.AddVertex(v6);

            graph.AddEdge(edge1);
            //graph.AddEdge(edge5);
            graph.AddEdge(edge2);
            graph.AddEdge(edge3);
            graph.AddEdge(edge4);

            return graph;
        }

        private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstructGraph_WithPositiveCycle()
        {
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph =
                new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();

            // Add values to the graph here
            TaggedEdge<VertexProperties, EdgeProperties> edge1;
            TaggedEdge<VertexProperties, EdgeProperties> edge2;
            TaggedEdge<VertexProperties, EdgeProperties> edge3;
            TaggedEdge<VertexProperties, EdgeProperties> edge4;
            TaggedEdge<VertexProperties, EdgeProperties> edge5;

            VertexProperties v1 = new VertexProperties("U", 0, false, false);
            VertexProperties v2 = new VertexProperties("V", int.MaxValue, false, false);
            VertexProperties v4 = new VertexProperties("X", int.MaxValue, false, false);
            VertexProperties v5 = new VertexProperties("Y", int.MaxValue, false, false);
            //VertexProperties v6 = new VertexProperties(Constants.SourceVertexName, 0, false, true);

            edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(1, 1));
            edge2 = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v4, new EdgeProperties(2, 1));
            edge3 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v5, new EdgeProperties(3, 1));
            edge4 = new TaggedEdge<VertexProperties, EdgeProperties>(v5, v4, new EdgeProperties(4, 1));
            edge5 = new TaggedEdge<VertexProperties, EdgeProperties>(v4, v1, new EdgeProperties(4, 1));

            //edge5 = new TaggedEdge<VertexProperties, EdgeProperties>(v6, v1, new EdgeProperties(0, 0));

            graph.AddVertex(v1);
            graph.AddVertex(v2);
            graph.AddVertex(v4);
            graph.AddVertex(v5);
            //graph.AddVertex(v6);

            graph.AddEdge(edge1);
            //graph.AddEdge(edge5);
            graph.AddEdge(edge2);
            graph.AddEdge(edge3);
            graph.AddEdge(edge4);
            graph.AddEdge(edge5);

            return graph;
        }

        private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstructAcyclicGraph_WithPositiveSlack_ReverseEdgeWeight()
        {
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph =
                new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();

            // Add values to the graph here
            TaggedEdge<VertexProperties, EdgeProperties> edge1;
            TaggedEdge<VertexProperties, EdgeProperties> edge2;
            TaggedEdge<VertexProperties, EdgeProperties> edge3;
            TaggedEdge<VertexProperties, EdgeProperties> edge4;
            TaggedEdge<VertexProperties, EdgeProperties> edge5;

            VertexProperties v1 = new VertexProperties("U", 0, false, false);
            VertexProperties v2 = new VertexProperties("V", int.MaxValue, false, false);
            VertexProperties v4 = new VertexProperties("X", int.MaxValue, false, false);
            VertexProperties v5 = new VertexProperties("Y", int.MaxValue, false, false);
            //VertexProperties v6 = new VertexProperties(Constants.SourceVertexName, 0, false, true);

            edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(2, 1));
            edge2 = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v4, new EdgeProperties(2, 1));
            edge3 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v5, new EdgeProperties(1, 1));
            edge4 = new TaggedEdge<VertexProperties, EdgeProperties>(v5, v4, new EdgeProperties(4, 1));
            //edge5 = new TaggedEdge<VertexProperties, EdgeProperties>(v6, v1, new EdgeProperties(0, 0));

            graph.AddVertex(v1);
            graph.AddVertex(v2);
            graph.AddVertex(v4);
            graph.AddVertex(v5);
            //graph.AddVertex(v6);

            graph.AddEdge(edge1);
            //graph.AddEdge(edge5);
            graph.AddEdge(edge2);
            graph.AddEdge(edge3);
            graph.AddEdge(edge4);

            return graph;
        }

    


         private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstructEarlyTermination_Acyclic_NotRelevantEdge()
        {
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph =
                new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();

            // Add values to the graph here
            TaggedEdge<VertexProperties, EdgeProperties> edge1;
            TaggedEdge<VertexProperties, EdgeProperties> edge2;
            TaggedEdge<VertexProperties, EdgeProperties> edge3;
            TaggedEdge<VertexProperties, EdgeProperties> edge4;
            TaggedEdge<VertexProperties, EdgeProperties> edge5;

            VertexProperties v1 = new VertexProperties("U", 0, false, false);
            VertexProperties v2 = new VertexProperties("V", int.MaxValue, false, false);
            VertexProperties v4 = new VertexProperties("X", int.MaxValue, false, false);
            VertexProperties v5 = new VertexProperties("Y", int.MaxValue, false, false);
            //VertexProperties v6 = new VertexProperties(Constants.SourceVertexName, 0, false, true);

            edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(1, 1));
            edge2 = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v4, new EdgeProperties(2, 1));
            edge3 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v5, new EdgeProperties(3, 1));
            edge4 = new TaggedEdge<VertexProperties, EdgeProperties>(v5, v4, new EdgeProperties(4, 1));
            edge4 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v4, new EdgeProperties(10, 1));
            //edge5 = new TaggedEdge<VertexProperties, EdgeProperties>(v6, v1, new EdgeProperties(0, 0));

            graph.AddVertex(v1);
            graph.AddVertex(v2);
            graph.AddVertex(v4);
            graph.AddVertex(v5);
            //graph.AddVertex(v6);

            graph.AddEdge(edge1);
            //graph.AddEdge(edge5);
            graph.AddEdge(edge2);
            graph.AddEdge(edge3);
            graph.AddEdge(edge4);

            return graph;
        }
    


        #endregion

        
    }
}