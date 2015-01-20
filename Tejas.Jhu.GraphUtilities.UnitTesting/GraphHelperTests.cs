using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;
using NUnit.Framework;
using QuickGraph;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;


namespace Tejas.Jhu.GraphUtilities.UnitTesting
{
    [TestFixture]
    public class GraphHelperTests
    {
        #region ConvertConstraintsToGraph Tests


        # region Distinct Less than

        [Test]
        public void ConvertConstraintsToGraph_DistinctPosNegLessThan()
        {
            //create data
            string constraint = "+x -y <= 4";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            //create object
            GraphHelper graphUtilities = new GraphHelper();
            //call function to be tested
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> resultGraph =
                graphUtilities.ConvertConstraintsToGraph(constraintList);
            //assert the results
            Assert.That(resultGraph.EdgeCount == 2);
            Assert.That(resultGraph.VertexCount == 4);
            TaggedEdge<VertexProperties, EdgeProperties> edgeResult1 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("x") &&
                      edge.Source.IsNegative == false &&
                      edge.Target.Name.Equals("y") &&
                      edge.Target.IsNegative == false
                select edge).FirstOrDefault();

            Assert.That(edgeResult1 != null);

            TaggedEdge<VertexProperties, EdgeProperties> edgeResult2 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("y") &&
                      edge.Source.IsNegative == true &&
                      edge.Target.Name.Equals("x") &&
                      edge.Target.IsNegative == true
                select edge).FirstOrDefault();
            Assert.That(edgeResult2 != null);

        }



        [Test]
        public void ConvertConstraintsToGraph_DistinctPosPosLessThan()
        {
            //create data
            string constraint = "+x +y <= 4";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            //create object
            GraphHelper graphUtilities = new GraphHelper();
            //call function to be tested
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> resultGraph =
                graphUtilities.ConvertConstraintsToGraph(constraintList);
            //assert the results
            Assert.That(resultGraph.EdgeCount == 2);
            Assert.That(resultGraph.VertexCount == 4);
            TaggedEdge<VertexProperties, EdgeProperties> edgeResult1 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("x") &&
                      edge.Source.IsNegative == false &&
                      edge.Target.Name.Equals("y") &&
                      edge.Target.IsNegative == true
                select edge).FirstOrDefault();

            Assert.That(edgeResult1 != null);

            TaggedEdge<VertexProperties, EdgeProperties> edgeResult2 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("y") &&
                      edge.Source.IsNegative == false &&
                      edge.Target.Name.Equals("x") &&
                      edge.Target.IsNegative == true
                select edge).FirstOrDefault();
            Assert.That(edgeResult2 != null);

        }

        [Test]
        public void ConvertConstraintsToGraph_DistinctNegPosLessThan()
        {
            //create data
            string constraint = "-x +y <= 4";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            //create object
            GraphHelper graphUtilities = new GraphHelper();
            //call function to be tested
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> resultGraph =
                graphUtilities.ConvertConstraintsToGraph(constraintList);
            //assert the results
            Assert.That(resultGraph.EdgeCount == 2);
            Assert.That(resultGraph.VertexCount == 4);
            TaggedEdge<VertexProperties, EdgeProperties> edgeResult1 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("x") &&
                      edge.Source.IsNegative == true &&
                      edge.Target.Name.Equals("y") &&
                      edge.Target.IsNegative == true
                select edge).FirstOrDefault();

            Assert.That(edgeResult1 != null);

            TaggedEdge<VertexProperties, EdgeProperties> edgeResult2 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("y") &&
                      edge.Source.IsNegative == false &&
                      edge.Target.Name.Equals("x") &&
                      edge.Target.IsNegative == false
                select edge).FirstOrDefault();
            Assert.That(edgeResult2 != null);

        }

        [Test]
        public void ConvertConstraintsToGraph_DistinctNegNegLessThan()
        {
            //create data
            string constraint = "-x -y <= 4";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            //create object
            GraphHelper graphUtilities = new GraphHelper();
            //call function to be tested
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> resultGraph =
                graphUtilities.ConvertConstraintsToGraph(constraintList);
            //assert the results
            Assert.That(resultGraph.EdgeCount == 2);
            Assert.That(resultGraph.VertexCount == 4);
            TaggedEdge<VertexProperties, EdgeProperties> edgeResult1 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("x") &&
                      edge.Source.IsNegative == true &&
                      edge.Target.Name.Equals("y") &&
                      edge.Target.IsNegative == false
                select edge).FirstOrDefault();

            Assert.That(edgeResult1 != null);

            TaggedEdge<VertexProperties, EdgeProperties> edgeResult2 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("y") &&
                      edge.Source.IsNegative == true &&
                      edge.Target.Name.Equals("x") &&
                      edge.Target.IsNegative == false
                select edge).FirstOrDefault();
            Assert.That(edgeResult2 != null);

        }

        # endregion

        #region Distinct Greater Than

        [Test]
        public void ConvertConstraintsToGraph_DistinctPosNegGreaterThan()
        {
            //create data
            string constraint = "+x -y >= 4";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            //create object
            GraphHelper graphUtilities = new GraphHelper();
            //call function to be tested
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> resultGraph =
                graphUtilities.ConvertConstraintsToGraph(constraintList);
            //assert the results
            Assert.That(resultGraph.EdgeCount == 2);
            Assert.That(resultGraph.VertexCount == 4);
            TaggedEdge<VertexProperties, EdgeProperties> edgeResult1 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("x") &&
                      edge.Source.IsNegative == true &&
                      edge.Target.Name.Equals("y") &&
                      edge.Target.IsNegative == true
                select edge).FirstOrDefault();

            Assert.That(edgeResult1 != null);

            TaggedEdge<VertexProperties, EdgeProperties> edgeResult2 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("y") &&
                      edge.Source.IsNegative == false &&
                      edge.Target.Name.Equals("x") &&
                      edge.Target.IsNegative == false
                select edge).FirstOrDefault();
            Assert.That(edgeResult2 != null);

        }

        [Test]
        public void ConvertConstraintsToGraph_DistinctNegPosGreaterThan()
        {
            //create data
            string constraint = "-x +y >= 4";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            //create object
            GraphHelper graphUtilities = new GraphHelper();
            //call function to be tested
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> resultGraph =
                graphUtilities.ConvertConstraintsToGraph(constraintList);
            //assert the results
            Assert.That(resultGraph.EdgeCount == 2);
            Assert.That(resultGraph.VertexCount == 4);
            TaggedEdge<VertexProperties, EdgeProperties> edgeResult1 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("x") &&
                      edge.Source.IsNegative == false &&
                      edge.Target.Name.Equals("y") &&
                      edge.Target.IsNegative == false
                select edge).FirstOrDefault();

            Assert.That(edgeResult1 != null);

            TaggedEdge<VertexProperties, EdgeProperties> edgeResult2 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("y") &&
                      edge.Source.IsNegative == true &&
                      edge.Target.Name.Equals("x") &&
                      edge.Target.IsNegative == true
                select edge).FirstOrDefault();
            Assert.That(edgeResult2 != null);

        }

        [Test]
        public void ConvertConstraintsToGraph_DistinctPosPosGreaterThan()
        {
            //create data
            string constraint = "+x +y >= 4";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            //create object
            GraphHelper graphUtilities = new GraphHelper();
            //call function to be tested
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> resultGraph =
                graphUtilities.ConvertConstraintsToGraph(constraintList);
            //assert the results
            Assert.That(resultGraph.EdgeCount == 2);
            Assert.That(resultGraph.VertexCount == 4);
            TaggedEdge<VertexProperties, EdgeProperties> edgeResult1 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("x") &&
                      edge.Source.IsNegative == true &&
                      edge.Target.Name.Equals("y") &&
                      edge.Target.IsNegative == false
                select edge).FirstOrDefault();

            Assert.That(edgeResult1 != null);

            TaggedEdge<VertexProperties, EdgeProperties> edgeResult2 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("y") &&
                      edge.Source.IsNegative == true &&
                      edge.Target.Name.Equals("x") &&
                      edge.Target.IsNegative == false
                select edge).FirstOrDefault();
            Assert.That(edgeResult2 != null);

        }

        [Test]
        public void ConvertConstraintsToGraph_DistinctNegNegGreaterThan()
        {
            //create data
            string constraint = "-x -y >= 4";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            //create object
            GraphHelper graphUtilities = new GraphHelper();
            //call function to be tested
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> resultGraph =
                graphUtilities.ConvertConstraintsToGraph(constraintList);
            //assert the results
            Assert.That(resultGraph.EdgeCount == 2);
            Assert.That(resultGraph.VertexCount == 4);
            TaggedEdge<VertexProperties, EdgeProperties> edgeResult1 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("x") &&
                      edge.Source.IsNegative == false &&
                      edge.Target.Name.Equals("y") &&
                      edge.Target.IsNegative == true
                select edge).FirstOrDefault();

            Assert.That(edgeResult1 != null);

            TaggedEdge<VertexProperties, EdgeProperties> edgeResult2 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("y") &&
                      edge.Source.IsNegative == false &&
                      edge.Target.Name.Equals("x") &&
                      edge.Target.IsNegative == true
                select edge).FirstOrDefault();
            Assert.That(edgeResult2 != null);

        }

        #endregion

        #region One variable less than

        [Test]
        public void ConvertConstraintsToGraph_SingleVarPosLessThan()
        {
            //create data
            string constraint = "+x  <= 4";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            //create object
            GraphHelper graphUtilities = new GraphHelper();
            //call function to be tested
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> resultGraph =
                graphUtilities.ConvertConstraintsToGraph(constraintList);
            //assert the results
            Assert.That(resultGraph.EdgeCount == 1);
            Assert.That(resultGraph.VertexCount == 2);
            TaggedEdge<VertexProperties, EdgeProperties> edgeResult1 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("x") &&
                      edge.Source.IsNegative == false &&
                      edge.Target.Name.Equals("x") &&
                      edge.Target.IsNegative == true
                select edge).FirstOrDefault();

            Assert.That(edgeResult1 != null);
        }


        [Test]
        public void ConvertConstraintsToGraph_SingleVarNegLessThan()
        {
            //create data
            string constraint = "-x  <= 4";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            //create object
            GraphHelper graphUtilities = new GraphHelper();
            //call function to be tested
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> resultGraph =
                graphUtilities.ConvertConstraintsToGraph(constraintList);
            //assert the results
            Assert.That(resultGraph.EdgeCount == 1);
            Assert.That(resultGraph.VertexCount == 2);
            TaggedEdge<VertexProperties, EdgeProperties> edgeResult1 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("x") &&
                      edge.Source.IsNegative == true &&
                      edge.Target.Name.Equals("x") &&
                      edge.Target.IsNegative == false
                select edge).FirstOrDefault();

            Assert.That(edgeResult1 != null);



        }

        #endregion

        # region One variable greater than

        [Test]
        public void ConvertConstraintsToGraph_SingleVarPosGreaterThan()
        {
            //create data
            string constraint = "+x  >= 4";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            //create object
            GraphHelper graphUtilities = new GraphHelper();
            //call function to be tested
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> resultGraph =
                graphUtilities.ConvertConstraintsToGraph(constraintList);
            //assert the results
            Assert.That(resultGraph.EdgeCount == 1);
            Assert.That(resultGraph.VertexCount == 2);
            TaggedEdge<VertexProperties, EdgeProperties> edgeResult1 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("x") &&
                      edge.Source.IsNegative == true &&
                      edge.Target.Name.Equals("x") &&
                      edge.Target.IsNegative == false
                select edge).FirstOrDefault();

            Assert.That(edgeResult1 != null);



        }

        [Test]
        public void ConvertConstraintsToGraph_SingleVarNegGreaterThan()
        {
            //create data
            string constraint = "-x  >= 4";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            //create object
            GraphHelper graphUtilities = new GraphHelper();
            //call function to be tested
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> resultGraph =
                graphUtilities.ConvertConstraintsToGraph(constraintList);
            //assert the results
            Assert.That(resultGraph.EdgeCount == 1);
            Assert.That(resultGraph.VertexCount == 2);
            TaggedEdge<VertexProperties, EdgeProperties> edgeResult1 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("x") &&
                      edge.Source.IsNegative == false &&
                      edge.Target.Name.Equals("x") &&
                      edge.Target.IsNegative == true
                select edge).FirstOrDefault();

            Assert.That(edgeResult1 != null);



        }

        #endregion

        #region Two variable similar less than

        [Test]
        public void ConvertConstraintsToGraph_SimilarVarPosPosLessThan()
        {
            //create data
            string constraint = "+x  +x <= 4";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            //create object
            GraphHelper graphUtilities = new GraphHelper();
            //call function to be tested
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> resultGraph =
                graphUtilities.ConvertConstraintsToGraph(constraintList);
            //assert the results
            Assert.That(resultGraph.EdgeCount == 1);
            Assert.That(resultGraph.VertexCount == 2);
            TaggedEdge<VertexProperties, EdgeProperties> edgeResult1 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("x") &&
                      edge.Source.IsNegative == false &&
                      edge.Target.Name.Equals("x") &&
                      edge.Target.IsNegative == true
                select edge).FirstOrDefault();

            Assert.That(edgeResult1 != null);



        }

        [Test]
        public void ConvertConstraintsToGraph_SimilarVarNegativeNegativeLessThan()
        {
            //create data
            string constraint = "-x  -x <= 4";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            //create object
            GraphHelper graphUtilities = new GraphHelper();
            //call function to be tested
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> resultGraph =
                graphUtilities.ConvertConstraintsToGraph(constraintList);
            //assert the results
            Assert.That(resultGraph.EdgeCount == 1);
            Assert.That(resultGraph.VertexCount == 2);
            TaggedEdge<VertexProperties, EdgeProperties> edgeResult1 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("x") &&
                      edge.Source.IsNegative == true &&
                      edge.Target.Name.Equals("x") &&
                      edge.Target.IsNegative == false
                select edge).FirstOrDefault();

            Assert.That(edgeResult1 != null);



        }



        #endregion

        # region Two variable similar greater than

        [Test]
        public void ConvertConstraintsToGraph_SimilarVarPosGreaterThan()
        {
            //create data
            string constraint = "+x  +x >= 4";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            //create object
            GraphHelper graphUtilities = new GraphHelper();
            //call function to be tested
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> resultGraph =
                graphUtilities.ConvertConstraintsToGraph(constraintList);
            //assert the results
            Assert.That(resultGraph.EdgeCount == 1);
            Assert.That(resultGraph.VertexCount == 2);
            TaggedEdge<VertexProperties, EdgeProperties> edgeResult1 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("x") &&
                      edge.Source.IsNegative == true &&
                      edge.Target.Name.Equals("x") &&
                      edge.Target.IsNegative == false
                select edge).FirstOrDefault();

            Assert.That(edgeResult1 != null);



        }


        [Test]
        public void ConvertConstraintsToGraph_SimilarVarNegGreaterThan()
        {
            //create data
            string constraint = "-x  -x >= 4";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint);
            //create object
            GraphHelper graphUtilities = new GraphHelper();
            //call function to be tested
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> resultGraph =
                graphUtilities.ConvertConstraintsToGraph(constraintList);
            //assert the results
            Assert.That(resultGraph.EdgeCount == 1);
            Assert.That(resultGraph.VertexCount == 2);
            TaggedEdge<VertexProperties, EdgeProperties> edgeResult1 = (from edge in resultGraph.Edges
                where edge.Source.Name.Equals("x") &&
                      edge.Source.IsNegative == false &&
                      edge.Target.Name.Equals("x") &&
                      edge.Target.IsNegative == true
                select edge).FirstOrDefault();

            Assert.That(edgeResult1 != null);



        }

        #endregion

        #endregion

        #region RetrieveConstraintsFromCycle Tests

        [Test]
        public void RetrieveConstraintsFromCycle_twoConstraintCyle()
        {
            //populate the input data
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph =
                new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();
            VertexProperties v1 = new VertexProperties("u", 0, true, false);
            VertexProperties v2 = new VertexProperties("v", 0, true, false);
            TaggedEdge<VertexProperties, EdgeProperties> edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2,
                new EdgeProperties(0, 4));
            TaggedEdge<VertexProperties, EdgeProperties> edge2 = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v1,
                new EdgeProperties(0, -6));

            graph.AddVertex(v1);
            graph.AddVertex(v2);
            graph.AddEdge(edge1);
            graph.AddEdge(edge2);
            List<VertexProperties> vertexList = new List<VertexProperties>();
            vertexList.Add(v1);
            vertexList.Add(v2);
            //call the function to test
            GraphHelper graphHelper = new GraphHelper();
            List<string> result = graphHelper.RetrieveConstraintsFromCycle(-1, -1, vertexList, graph);
            Assert.That(result.Count == 2);
            Assert.That(result.ElementAt(0).Equals("+u -v <= 4"));
            Assert.That(result.ElementAt(1).Equals("+v -u <= -6"));




        }

        [Test]
        public void RetrieveConstraintsFromCycle_twoConstraintCyleUsingConvertConstraintToGraph()
        {
            //populate the input data
            string constraint1 = "+u -v <= 6";
            string constraint2 = "+v -u <= -10";
            List<string> constraintList = new List<string>();
            constraintList.Add(constraint1);
            constraintList.Add(constraint2);
            GraphHelper graphHelper = new GraphHelper();
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph =
                graphHelper.ConvertConstraintsToGraph(constraintList);
            VertexProperties v1 = new VertexProperties("u", 0, true, false);
            VertexProperties v2 = new VertexProperties("v", 0, true, false);
            List<VertexProperties> vertexList = new List<VertexProperties>();
            vertexList.Add(v1);
            vertexList.Add(v2);
            //call the function to test

            List<string> result = graphHelper.RetrieveConstraintsFromCycle(-1, -1, vertexList, graph);
            Assert.That(result.Count == 2);
            Assert.That(result.ElementAt(0).Equals(constraint1));
            Assert.That(result.ElementAt(1).Equals(constraint2));

        }


        #endregion

        #region RetrieveConstraintsFromCycle SCC tests

        //start index is first elment. Second element is in the middle of the vertex list.
        [Test]
        public void RetrieveConstraintsFromCycle_SCCTest_FirstMiddle()
        {
            //populate the input data

            VertexProperties v1 = new VertexProperties("u", 1, true, false);
            VertexProperties v2 = new VertexProperties("v", 2, true, false);
            VertexProperties v3 = new VertexProperties("u", 3, true, true);
            VertexProperties v4 = new VertexProperties("t", 1, true, false);


            TaggedEdge<VertexProperties, EdgeProperties> edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2,
                new EdgeProperties(0, 1));
            TaggedEdge<VertexProperties, EdgeProperties> edge2 = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v3,
                new EdgeProperties(0, 2));
            TaggedEdge<VertexProperties, EdgeProperties> edge3 = new TaggedEdge<VertexProperties, EdgeProperties>(v3, v4,
                new EdgeProperties(0, 3));
            TaggedEdge<VertexProperties, EdgeProperties> edge4 = new TaggedEdge<VertexProperties, EdgeProperties>(v4, v1,
                new EdgeProperties(0, 4));


            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstraintGraph =
                new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();

            ConstraintGraph.AddVertex(v1);
            ConstraintGraph.AddVertex(v2);
            ConstraintGraph.AddVertex(v3);
            ConstraintGraph.AddVertex(v4);

            ConstraintGraph.AddEdge(edge1);
            ConstraintGraph.AddEdge(edge2);
            ConstraintGraph.AddEdge(edge3);
            ConstraintGraph.AddEdge(edge4);

            List<VertexProperties> sccList = new List<VertexProperties>();
            sccList.Add(v1);
            sccList.Add(v2);
            sccList.Add(v3);
            sccList.Add(v4);

            GraphHelper graphHelper = new GraphHelper();

            //call the function to be tested

            List<string> result = graphHelper.RetrieveConstraintsFromCycle(sccList.IndexOf(v1), sccList.IndexOf(v3),
                sccList, ConstraintGraph);

            //assert the results

            Assert.That(result.Count == 2);
            Assert.That(result.Contains("+u +u <= 3"));

        }

        //both elements are in the middle of the vertex list.

        [Test]
        public void RetrieveConstraintsFromCycle_SCCTest_MiddleMiddle()
        {
            //populate the input data

            VertexProperties v1 = new VertexProperties("v", 1, true, false);
            VertexProperties v2 = new VertexProperties("u", 2, true, false);
            VertexProperties v3 = new VertexProperties("x", 3, true, true);
            VertexProperties v4 = new VertexProperties("u", 3, true, true);
            VertexProperties v5 = new VertexProperties("t", 1, true, false);


            TaggedEdge<VertexProperties, EdgeProperties> edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2,
                new EdgeProperties(0, 1));
            TaggedEdge<VertexProperties, EdgeProperties> edge2 = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v3,
                new EdgeProperties(0, 2));
            TaggedEdge<VertexProperties, EdgeProperties> edge3 = new TaggedEdge<VertexProperties, EdgeProperties>(v3, v4,
                new EdgeProperties(0, 3));
            TaggedEdge<VertexProperties, EdgeProperties> edge4 = new TaggedEdge<VertexProperties, EdgeProperties>(v4, v5,
                new EdgeProperties(0, 4));
            TaggedEdge<VertexProperties, EdgeProperties> edge5 = new TaggedEdge<VertexProperties, EdgeProperties>(v5, v1,
                new EdgeProperties(0, 5));

            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstraintGraph =
                new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();

            ConstraintGraph.AddVertex(v1);
            ConstraintGraph.AddVertex(v2);
            ConstraintGraph.AddVertex(v3);
            ConstraintGraph.AddVertex(v4);
            ConstraintGraph.AddVertex(v5);

            ConstraintGraph.AddEdge(edge1);
            ConstraintGraph.AddEdge(edge2);
            ConstraintGraph.AddEdge(edge3);
            ConstraintGraph.AddEdge(edge4);
            ConstraintGraph.AddEdge(edge5);

            List<VertexProperties> sccList = new List<VertexProperties>();
            sccList.Add(v1);
            sccList.Add(v2);
            sccList.Add(v3);
            sccList.Add(v4);
            sccList.Add(v5);

            GraphHelper graphHelper = new GraphHelper();

            //call the function to be tested

            List<string> result = graphHelper.RetrieveConstraintsFromCycle(sccList.IndexOf(v2), sccList.IndexOf(v4),
                sccList, ConstraintGraph);

            //assert the results

            Assert.That(result.Count == 2);
            Assert.That(result.Contains("+u +u <= 5"));
            Assert.That(result.Contains("-u -u <= 10"));   


        }

        //both vertices are at the end
        [Test]
        public void RetrieveConstraintsFromCycle_SCCTest_EndEnd()
        {
            //populate the input data

            VertexProperties v1 = new VertexProperties("u", 1, true, false);
            VertexProperties v2 = new VertexProperties("v", 2, true, false);
            VertexProperties v3 = new VertexProperties("x", 3, true, true);
            VertexProperties v4 = new VertexProperties("t", 3, true, true);
            VertexProperties v5 = new VertexProperties("u", 1, true, true);


            TaggedEdge<VertexProperties, EdgeProperties> edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2,
                new EdgeProperties(0, 1));
            TaggedEdge<VertexProperties, EdgeProperties> edge2 = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v3,
                new EdgeProperties(0, 2));
            TaggedEdge<VertexProperties, EdgeProperties> edge3 = new TaggedEdge<VertexProperties, EdgeProperties>(v3, v4,
                new EdgeProperties(0, 3));
            TaggedEdge<VertexProperties, EdgeProperties> edge4 = new TaggedEdge<VertexProperties, EdgeProperties>(v4, v5,
                new EdgeProperties(0, 4));
            TaggedEdge<VertexProperties, EdgeProperties> edge5 = new TaggedEdge<VertexProperties, EdgeProperties>(v5, v1,
                new EdgeProperties(0, 5));

            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstraintGraph =
                new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();

            ConstraintGraph.AddVertex(v1);
            ConstraintGraph.AddVertex(v2);
            ConstraintGraph.AddVertex(v3);
            ConstraintGraph.AddVertex(v4);
            ConstraintGraph.AddVertex(v5);

            ConstraintGraph.AddEdge(edge1);
            ConstraintGraph.AddEdge(edge2);
            ConstraintGraph.AddEdge(edge3);
            ConstraintGraph.AddEdge(edge4);
            ConstraintGraph.AddEdge(edge5);

            List<VertexProperties> sccList = new List<VertexProperties>();
            sccList.Add(v1);
            sccList.Add(v2);
            sccList.Add(v3);
            sccList.Add(v4);
            sccList.Add(v5);

            GraphHelper graphHelper = new GraphHelper();

            //call the function to be tested

            List<string> result = graphHelper.RetrieveConstraintsFromCycle(sccList.IndexOf(v1), sccList.IndexOf(v5),
                sccList, ConstraintGraph);

            //assert the results

            Assert.That(result.Count == 2);
            Assert.That(result.Contains("+u +u <= 10"));
            Assert.That(result.Contains("-u -u <= 5"));


        }


        //one vertex is in the middle and the other is at the end
        [Test]
        public void RetrieveConstraintsFromCycle_SCCTest_MiddleEnd()
        {
            //populate the input data

            VertexProperties v1 = new VertexProperties("v", 1, true, false);
            VertexProperties v2 = new VertexProperties("u", 2, true, false);
            VertexProperties v3 = new VertexProperties("x", 3, true, true);
            VertexProperties v4 = new VertexProperties("t", 3, true, true);
            VertexProperties v5 = new VertexProperties("u", 1, true, true);


            TaggedEdge<VertexProperties, EdgeProperties> edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2,
                new EdgeProperties(0, 1));
            TaggedEdge<VertexProperties, EdgeProperties> edge2 = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v3,
                new EdgeProperties(0, 2));
            TaggedEdge<VertexProperties, EdgeProperties> edge3 = new TaggedEdge<VertexProperties, EdgeProperties>(v3, v4,
                new EdgeProperties(0, 3));
            TaggedEdge<VertexProperties, EdgeProperties> edge4 = new TaggedEdge<VertexProperties, EdgeProperties>(v4, v5,
                new EdgeProperties(0, 4));
            TaggedEdge<VertexProperties, EdgeProperties> edge5 = new TaggedEdge<VertexProperties, EdgeProperties>(v5, v1,
                new EdgeProperties(0, 5));

            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstraintGraph =
                new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();

            ConstraintGraph.AddVertex(v1);
            ConstraintGraph.AddVertex(v2);
            ConstraintGraph.AddVertex(v3);
            ConstraintGraph.AddVertex(v4);
            ConstraintGraph.AddVertex(v5);

            ConstraintGraph.AddEdge(edge1);
            ConstraintGraph.AddEdge(edge2);
            ConstraintGraph.AddEdge(edge3);
            ConstraintGraph.AddEdge(edge4);
            ConstraintGraph.AddEdge(edge5);

            List<VertexProperties> sccList = new List<VertexProperties>();
            sccList.Add(v1);
            sccList.Add(v2);
            sccList.Add(v3);
            sccList.Add(v4);
            sccList.Add(v5);

            GraphHelper graphHelper = new GraphHelper();

            //call the function to be tested

            List<string> result = graphHelper.RetrieveConstraintsFromCycle(sccList.IndexOf(v2), sccList.IndexOf(v5),
                sccList, ConstraintGraph);

            //assert the results

            Assert.That(result.Count == 2);
            Assert.That(result.Contains("+u +u <= 9"));
            Assert.That(result.Contains("-u -u <= 6"));


        }


        #endregion


    }
}
