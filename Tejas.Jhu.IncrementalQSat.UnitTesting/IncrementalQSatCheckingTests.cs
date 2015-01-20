using System.Collections.Generic;
using NUnit.Framework;
using QuickGraph;
using Tejas.Jhu.ConsistencyChecking;
using Tejas.Jhu.ConsistencyChecking.DataContracts;
using Tejas.Jhu.GraphUtilities;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.IncrementalQSatChecking;
using Tejas.Jhu.IncrementalQSatChecking.DataContracts;
using Tejas.Jhu.NegativeCycleDetection;

namespace Tejas.Jhu.IncrementalQSat.UnitTesting
{
    [TestFixture]
    public class IncrementalQSatCheckingTests
    {

        #region Incremental Q Sat Tests
        [Test]
        [Repeat (200)]
        public void IncrementalQSatChecking_Acyclic_Test()
        {
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph =
                ConstructAcyclicGraph_WithPositiveSlack();
            VertexProperties v1 = new VertexProperties("U", 0, true,false);
            VertexProperties v2 = new VertexProperties("V", 1, true, false);

            TaggedEdge<VertexProperties,EdgeProperties> constraintEdge = new TaggedEdge<VertexProperties, EdgeProperties>(v1,v2,new EdgeProperties(int.MaxValue,0));
            IIncrementalQSatChecking incrementalQSatChecking= new IncrementalQSatChecking.IncrementalQSatChecking();

            IncrementalQSatCheckingResults result=
                incrementalQSatChecking.CheckQSatisfiability(constraintGraph,constraintEdge);
            Assert.That(result.IsConsistencyCheckSuccessful);
            Assert.That(result.FailedConstraintVerticesList.Count == 0);
            Assert.That(result.ChangedPotentialValues.Count == 2);
        }


        [Test]
        [Repeat (200)]
        public void IncrementalQSatChecking_Acyclic_NoChangeToPotential_Test()
        {
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph =
                ConstructAcyclicGraph_WithPositiveSlack();

            VertexProperties v2 = new VertexProperties("X", 3, true, false);
            VertexProperties v1 = new VertexProperties("Y", 4, true, false);

            TaggedEdge<VertexProperties, EdgeProperties> constraintEdge = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(int.MaxValue,2));
            IIncrementalQSatChecking incrementalQSatChecking = new IncrementalQSatChecking.IncrementalQSatChecking();

            IncrementalQSatCheckingResults result =
                incrementalQSatChecking.CheckQSatisfiability(constraintGraph, constraintEdge);
            Assert.That(result.IsConsistencyCheckSuccessful);
            Assert.That(result.FailedConstraintVerticesList.Count == 0);
            Assert.That(result.ChangedPotentialValues.Count == 0);
        }

        [Test]
        [Repeat(200)]
        public void IncrementalQSatChecking_Acyclic_QUnsat_Test()
        {
            IList<string> constraintList = new List<string>();
            string constraint1 = "+U -V <= 1";
            string constraint2 = "+U -Y <= 4";
            string constraint3 = "+Y -X <= 7`";
            string constraint4 = "+V -X <= 2";
            constraintList.Add(constraint1);
            constraintList.Add(constraint2);
            constraintList.Add(constraint3);
            constraintList.Add(constraint4);
            IGraphHelper graphHelper=new GraphHelper();
            IGraphTraversalAlgorithms graphTraversalAlgorithm=new GraphTraversalAlgorithms();
            //BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph =


            INegativeCycleDetection ncdAlgorithm = new NegativeCycleDetectionUsingGR(graphTraversalAlgorithm);
            
            ConsistencyChecker consistencyChecker = new NonIncrementalConsistencyChecker(ncdAlgorithm,
                graphTraversalAlgorithm, graphHelper);
            ConsistencyCheckResults resultOfCheck = consistencyChecker.CheckConsistency(constraintList);

            VertexProperties v2 = new VertexProperties("X", -10, true, false);
            VertexProperties v1 = new VertexProperties("Y", 45, true, false);

            TaggedEdge<VertexProperties, EdgeProperties> constraintEdge = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v1, new EdgeProperties(int.MaxValue, -20));
            IIncrementalQSatChecking incrementalQSatChecking = new IncrementalQSatChecking.IncrementalQSatChecking();

            IncrementalQSatCheckingResults result =
                incrementalQSatChecking.CheckQSatisfiability(resultOfCheck.Graph, constraintEdge);
            Assert.That(!result.IsConsistencyCheckSuccessful);
            Assert.That(result.FailedConstraintVerticesList.Count ==2);
            
            //Assert.That(result.ChangedPotentialValues.Count == 0);
        }


        [Test]
        [Repeat(200)]
        public void IncrementalQSatChecking_CheckForLeastEdgeCycle_Test()
        {
            IList<string> constraintList = new List<string>();
            string constraint1 = "+U -V <= 1";
            string constraint2 = "+U -Y <= 4";
            string constraint3 = "+Y -X <= 7`";
            string constraint4 = "+V -X <= 2";
            string constraint5 = "+U -X <= 2";
            
            constraintList.Add(constraint1);
            constraintList.Add(constraint2);
            constraintList.Add(constraint3);
            constraintList.Add(constraint4);
            constraintList.Add(constraint5);
             
            IGraphHelper graphHelper = new GraphHelper();
            IGraphTraversalAlgorithms graphTraversalAlgorithm = new GraphTraversalAlgorithms();
            //BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> constraintGraph =


            INegativeCycleDetection ncdAlgorithm = new NegativeCycleDetectionUsingGR(graphTraversalAlgorithm);

            ConsistencyChecker consistencyChecker = new NonIncrementalConsistencyChecker(ncdAlgorithm,
                graphTraversalAlgorithm, graphHelper);
            ConsistencyCheckResults resultOfCheck = consistencyChecker.CheckConsistency(constraintList);

            VertexProperties v2 = new VertexProperties("X", -10, true, false);
            VertexProperties v1 = new VertexProperties("U", 45, true, false);

            TaggedEdge<VertexProperties, EdgeProperties> constraintEdge = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v1, new EdgeProperties(int.MaxValue, -200));
            IIncrementalQSatChecking incrementalQSatChecking = new IncrementalQSatChecking.IncrementalQSatChecking();

            IncrementalQSatCheckingResults result =
                incrementalQSatChecking.CheckQSatisfiability(resultOfCheck.Graph, constraintEdge);
            Assert.That(!result.IsConsistencyCheckSuccessful);
            Assert.That(result.FailedConstraintVerticesList.Count == 2);

            //Assert.That(result.ChangedPotentialValues.Count == 0);
        }


        #endregion


        #region private methods to construct the graph

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

            VertexProperties v1 = new VertexProperties("U", 0, true,false);
            VertexProperties v2 = new VertexProperties("V", 1, true, false);
            VertexProperties v4 = new VertexProperties("X", 3, true, false);
            VertexProperties v5 = new VertexProperties("Y", 4, true, false);
            //VertexProperties v6 = new VertexProperties(Constants.SourceVertexName, 0, false, true);

            edge1 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v2, new EdgeProperties(0,1));
            edge2 = new TaggedEdge<VertexProperties, EdgeProperties>(v2, v4, new EdgeProperties(0, 2));
            edge3 = new TaggedEdge<VertexProperties, EdgeProperties>(v1, v5, new EdgeProperties(0, 4));
            edge4 = new TaggedEdge<VertexProperties, EdgeProperties>(v5, v4, new EdgeProperties(4, 3));
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