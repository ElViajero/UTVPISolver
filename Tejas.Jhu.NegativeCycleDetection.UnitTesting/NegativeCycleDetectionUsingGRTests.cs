using System.Collections.Generic;
using NUnit.Framework;
using QuickGraph;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.NegativeCycleDetection.DataContracts;

namespace Tejas.Jhu.NegativeCycleDetection.UnitTesting
{
    [TestFixture]
    public class NegativeCycleDetectionUsingGRTests
    {
        [Test]
        [Repeat(2)]
        public void TestDetectNegativeCycles_PositiveCycle()
        {
            // System under test
            GraphTraversalAlgorithms gta = new GraphTraversalAlgorithms();
            INegativeCycleDetection negativeCycleDetectionObj = new NegativeCycleDetectionUsingGR(gta);
            
            // Prepare the input
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph = ConstructPositiveCycleGraph();
            
            // Call the system under test
            NegativeCycleDetectionResult results = negativeCycleDetectionObj.DetectNegativeCycles(graph);

            // Check the output
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> scannedGraph = results.ScannedGraph;
            List<VertexProperties> failedConstraintsList = results.FailedConstraints;

            // NOTE: Add your assertions here for automated checking of the results
            //Assert.That(vertexDistanceLabelList.Count, Is.EqualTo(3));
            //Assert.That(vertexDistanceLabelList.Contains(), Is.EqualTo(3));
            Assert.That(results.IsNegativeCycleDetected, Is.EqualTo(false));
        }

        [Test]
        [Repeat(2)]
        public void TestDetectNegativeCycles_NegativeCycle()
        {
            // System under test
            GraphTraversalAlgorithms gta = new GraphTraversalAlgorithms();
            INegativeCycleDetection negativeCycleDetectionObj = new NegativeCycleDetectionUsingGR(gta);

            // Prepare the input
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph = ConstructNegativeCycleGraph();

            // Call the system under test
            NegativeCycleDetectionResult results = negativeCycleDetectionObj.DetectNegativeCycles(graph);

            // Check the output
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> scannedGraph = results.ScannedGraph;
            List<VertexProperties> failedConstraintsList = results.FailedConstraints;

            // NOTE: Add your assertions here for automated checking of the results
            //Assert.That(vertexDistanceLabelList.Count, Is.EqualTo(3));
            //Assert.That(vertexDistanceLabelList.Contains(), Is.EqualTo(3));
            Assert.That(results.IsNegativeCycleDetected, Is.EqualTo(true));
        }



        [Test]
        [Repeat(2)]
        public void TestDetectNegativeCycles_DisjointNegativeCycle()
        {
            // System under test
            GraphTraversalAlgorithms gta = new GraphTraversalAlgorithms();
            INegativeCycleDetection negativeCycleDetectionObj = new NegativeCycleDetectionUsingGR(gta);

            // Prepare the input
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph = ConstructDisjointNegativeCycleGraph();

            // Call the system under test
            NegativeCycleDetectionResult results = negativeCycleDetectionObj.DetectNegativeCycles(graph);

            // Check the output
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> scannedGraph = results.ScannedGraph;
            List<VertexProperties> failedConstraintsList = results.FailedConstraints;

            // NOTE: Add your assertions here for automated checking of the results
            //Assert.That(vertexDistanceLabelList.Count, Is.EqualTo(3));
            //Assert.That(vertexDistanceLabelList.Contains(), Is.EqualTo(3));
            Assert.That(results.IsNegativeCycleDetected, Is.EqualTo(true));
        }

        #region Prepare Graph Data

        private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstructPositiveCycleGraph()
        {
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph =
                new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();

            // Add values to the graph here
            VertexProperties v1;
            VertexProperties v2;
            VertexProperties v3;
            TaggedEdge<VertexProperties, EdgeProperties> edge;

            v1 = new VertexProperties("U", 0, false,true);
            v2 = new VertexProperties("V", 0, false,true);
            edge = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(0, 3));

            graph.AddVertex(v1);
            graph.AddVertex(v2);
            graph.AddEdge(edge);

            v3 = new VertexProperties("W", 0, false,true);
            edge = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v3, new EdgeProperties(0, 5));

            graph.AddVertex(v3);
            graph.AddEdge(edge);

            edge = new TaggedEdge<VertexProperties, EdgeProperties>(v3, v1, new EdgeProperties(0, 5));
            graph.AddEdge(edge);
            
            return graph;
        }


        private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstructNegativeCycleGraph()
        {
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph =
                new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();

            // Add values to the graph here
            VertexProperties v1;
            VertexProperties v2;
            VertexProperties v3;

            TaggedEdge<VertexProperties, EdgeProperties> edge;

            v1 = new VertexProperties("U", 0, false,false);
            v2 = new VertexProperties("V", 0, false,false);
            edge = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(0, 3));

            graph.AddVertex(v1);
            graph.AddVertex(v2);
            graph.AddEdge(edge);

            v3 = new VertexProperties("W", 0, false,false);
            edge = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v3, new EdgeProperties(0, 5));

            graph.AddVertex(v3);
            graph.AddEdge(edge);

            edge = new TaggedEdge<VertexProperties, EdgeProperties>(v3, v2, new EdgeProperties(0,-15));
            graph.AddEdge(edge);

            return graph;
        }



        private BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstructDisjointNegativeCycleGraph()
        {
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph =
                new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();

            // Add values to the graph here
            VertexProperties v1;
            VertexProperties v2;
            VertexProperties v3;
            VertexProperties v4;

            TaggedEdge<VertexProperties, EdgeProperties> edge;

            v1 = new VertexProperties("U", 0, false,false);
            v2 = new VertexProperties("V", 0, false,true);
            edge = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(0, 3));

            graph.AddVertex(v1);
            graph.AddVertex(v2);
            graph.AddEdge(edge);

            v3 = new VertexProperties("W", 0, false,true);
            v4 = new VertexProperties("X", 0, false,true);
            edge = new TaggedEdge<VertexProperties, EdgeProperties>(v3, v4, new EdgeProperties(0, 5));

            graph.AddVertex(v3);
            graph.AddVertex(v4);
            graph.AddEdge(edge);

            edge = new TaggedEdge<VertexProperties, EdgeProperties>(v4, v3, new EdgeProperties(0, -15));
            graph.AddEdge(edge);

            return graph;
        }

        #endregion
    }
}