using System.Collections.Generic;
using NUnit.Framework;
using QuickGraph;
using Tejas.Jhu.GraphUtilities;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.NegativeCycleDetection;

namespace Tejas.Jhu.ImplicationChecking.UnitTesting
{
    [TestFixture]
    public class NonIncrementalImplicationCheckerTests
    {
        #region NonIncrementalImplicationChecker Tests
        
        [Test]
        public void NonIncrementalImplicationChecker_SingleConstraintTest()
        {
            //populate the input data
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph =
                ConstructAcyclicGraph_WithPositiveSlack();

            string constraint = "+U -V <= 6";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            var graphTraversalAlgorithms = new GraphTraversalAlgorithms();
            var graphHelper=new GraphHelper();
            ImplicationChecker implicationChecker = new NonIncrementalImplicationChecker(graphTraversalAlgorithms,
                graphHelper);
            IList<string> result = implicationChecker.CheckImplication(constraintGraph, constraintList);
            Assert.That(result.Count==1);
            Assert.That(result[0].Equals("+U -V <= 6"));
        }

        [Test]
        public void NonIncrementalImplicationChecker_MultipleConstraintTest()
        {
            //populate the input data
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph =
                ConstructAcyclicGraph_WithPositiveSlack();

            string constraint = "+U -V <= 6";           
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            constraint = "+U -V <= 0";
            constraintList.Add(constraint);
            
            var graphTraversalAlgorithms = new GraphTraversalAlgorithms();
            var graphHelper = new GraphHelper();
            ImplicationChecker implicationChecker = new NonIncrementalImplicationChecker(graphTraversalAlgorithms,
                graphHelper);
            IList<string> result = implicationChecker.CheckImplication(constraintGraph, constraintList);
            Assert.That(result.Count == 1);
            Assert.That(result[0].Equals("+U -V <= 6"));
        }

        [Test]
        public void NonIncrementalImplicationChecker_MultipleConstraintWithNonShortestPathConstraintTest()
        {
            //populate the input data
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph =
                ConstructAcyclicGraph_WithPositiveSlack();

            string constraint = "+U -V <= 6";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            constraint = "+U -V <= 0";
            constraintList.Add(constraint);
            constraint = "+Y -X <= 10";
            constraintList.Add(constraint);
            var graphTraversalAlgorithms = new GraphTraversalAlgorithms();
            var graphHelper = new GraphHelper();
            ImplicationChecker implicationChecker = new NonIncrementalImplicationChecker(graphTraversalAlgorithms,
                graphHelper);
            IList<string> result = implicationChecker.CheckImplication(constraintGraph, constraintList);
            Assert.That(result.Count == 2);
            Assert.That(result.Contains("+U -V <= 6"));
            Assert.That(result.Contains("+Y -X <= 10"));
        }
        
        [Test]
        public void NonIncrementalImplicationChecker_MultipleConstraintWithNonGraphConstraintConstraintTest()
        {
            //populate the input data
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph =
                ConstructAcyclicGraph_WithPositiveSlack();

            string constraint = "+U -V <= 6";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            constraint = "+U -V <= 0";
            constraintList.Add(constraint);
            constraint = "+P -Z <= 10";
            constraintList.Add(constraint);
            var graphTraversalAlgorithms = new GraphTraversalAlgorithms();
            var graphHelper = new GraphHelper();
            ImplicationChecker implicationChecker = new NonIncrementalImplicationChecker(graphTraversalAlgorithms,
                graphHelper);
            IList<string> result = implicationChecker.CheckImplication(constraintGraph, constraintList);
            Assert.That(result.Count == 1);
            Assert.That(result[0].Equals("+U -V <= 6"));
        }

        [Test]
        public void NonIncrementalImplicationChecker_MultipleTransitiveClosureConstraintTest()
        {
            //populate the input data
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph =
                ConstructAcyclicGraph_WithPositiveSlack();

            string constraint = "+U -X <= 6";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            constraint = "+U -V <= 0";
            constraintList.Add(constraint);

            var graphTraversalAlgorithms = new GraphTraversalAlgorithms();
            var graphHelper = new GraphHelper();
            ImplicationChecker implicationChecker = new NonIncrementalImplicationChecker(graphTraversalAlgorithms,
                graphHelper);
            IList<string> result = implicationChecker.CheckImplication(constraintGraph, constraintList);
            Assert.That(result.Count == 1);
            Assert.That(result[0].Equals("+U -X <= 6"));
        }


        #endregion

        #region private helper methods

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
            VertexProperties v2 = new VertexProperties("V", 1, true, false);
            VertexProperties v4 = new VertexProperties("X", 3, false, false);
            VertexProperties v5 = new VertexProperties("Y", 3, false, false);
            //VertexProperties v6 = new VertexProperties(Constants.SourceVertexName, 0, false, true);

            edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(0, 1));
            edge2 = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v4, new EdgeProperties(0, 2));
            edge3 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v5, new EdgeProperties(0, 3));
            edge4 = new TaggedEdge<VertexProperties, EdgeProperties>(v5, v4, new EdgeProperties(4, 4));
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

        #endregion
    }
}