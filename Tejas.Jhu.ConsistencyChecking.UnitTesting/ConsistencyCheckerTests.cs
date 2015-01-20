using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Microsoft.SqlServer.Server;
using NUnit.Framework;
using QuickGraph;
using Tejas.Jhu.ConsistencyChecking.DataContracts;
using Tejas.Jhu.GraphUtilities;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.NegativeCycleDetection;

namespace Tejas.Jhu.ConsistencyChecking.UnitTesting
{
    [TestFixture]
    public class ConsistencyCheckerTests
    {
        #region Test Methods

        
        public void CheckConsistency_Incremental()
        {
            // System under test
            //ConsistencyChecker checker = new IncrementalConsistencyChecker();

            // Prepare the test data here
            // NOTE: Appropriately fill the data in the grpah and constraints list
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph = new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();
            List<string> constraintsList = new List<string>();

            // Test the system
            //ConsistencyCheckResults results = checker.CheckConsistency(graph, constraintsList);

            // Assert the results

        }

        [Test]
        [Repeat(100)]
        public void CheckConsistency_NonIncremental_SingleConstraint()
        {
            //populate the input data
            string constraint = "+U -V <= 6";
            var constraintList=new List<string>();
            constraintList.Add(constraint);
            IGraphHelper graphHelper = new GraphHelper();
            IGraphTraversalAlgorithms graphTraversalAlgorithm = new GraphTraversalAlgorithms();
            INegativeCycleDetection ncdAlgorithm = new NegativeCycleDetectionUsingGR(graphTraversalAlgorithm);
            // System under test
            ConsistencyChecker consistencyChecker = new NonIncrementalConsistencyChecker(ncdAlgorithm, graphTraversalAlgorithm,
                graphHelper);
            ConsistencyCheckResults result = consistencyChecker.CheckConsistency(constraintList);
            var v1 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("U", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == false
                      select vertex).FirstOrDefault();
            var v2 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("V", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == false
                      select vertex).FirstOrDefault();
            var v3 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("U", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == true
                      select vertex).FirstOrDefault();
            var v4 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("V", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == true
                      select vertex).FirstOrDefault();
            Assert.That(result != null);
            Assert.That(v1.DistanceLabel == 0);
            Assert.That(v1.DistanceLabel == 0);
            Assert.That(v3.DistanceLabel == 6);
            Assert.That(v4.DistanceLabel == 0);
            
            

            // Test the system
            //ConsistencyCheckResults results = checker.CheckConsistency(constraintsList);

            // Assert the results
        }

        [Test]
        [Repeat (100)]
        public void CheckConsistency_NonIncremental_MultipleConstraints()
        {
            //populate the input data
            string constraint = "+U -V <= 6";            
            var constraintList = new List<string>();
            constraint = "+V -U <= 4";
            constraintList.Add(constraint);
            IGraphHelper graphHelper = new GraphHelper();
            IGraphTraversalAlgorithms graphTraversalAlgorithm = new GraphTraversalAlgorithms();
            INegativeCycleDetection ncdAlgorithm = new NegativeCycleDetectionUsingGR(graphTraversalAlgorithm);
            // System under test
            ConsistencyChecker consistencyChecker = new NonIncrementalConsistencyChecker(ncdAlgorithm, graphTraversalAlgorithm,
                graphHelper);
            ConsistencyCheckResults result = consistencyChecker.CheckConsistency(constraintList);
            var v1 = (from vertex in result.Graph.Vertices
                where vertex.Name.Equals("U", StringComparison.InvariantCultureIgnoreCase) &&
                      vertex.IsNegative == false
                select vertex).FirstOrDefault();
            var v2 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("V", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == false
                      select vertex).FirstOrDefault();
            var v3 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("U", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == true
                      select vertex).FirstOrDefault();
            var v4 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("V", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == true
                      select vertex).FirstOrDefault();
            
            
            Assert.That(result != null);
            Assert.That(v1.DistanceLabel == 4);
            Assert.That(v2.DistanceLabel == 0);
            Assert.That(v3.DistanceLabel == 0);
            Assert.That(v4.DistanceLabel == 4);


            
            // Test the system
            //ConsistencyCheckResults results = checker.CheckConsistency(constraintsList);

            // Assert the results

        }


        [Test]
        [Repeat (100)]
        public void CheckConsistency_NonIncremental_MultipleConstraints_AnOddPath()
        {
            //populate the input data
            string constraint = "+U -V <= 6";            
            var constraintList = new List<string>();
            constraintList.Add(constraint);
            constraint = "+V -U <= 4";
            constraintList.Add(constraint);
            constraint = "+U <= 1";
            constraintList.Add(constraint);
            IGraphHelper graphHelper = new GraphHelper();
            IGraphTraversalAlgorithms graphTraversalAlgorithm = new GraphTraversalAlgorithms();
            INegativeCycleDetection ncdAlgorithm = new NegativeCycleDetectionUsingGR(graphTraversalAlgorithm);
            // System under test
            ConsistencyChecker consistencyChecker = new NonIncrementalConsistencyChecker(ncdAlgorithm, graphTraversalAlgorithm,
                graphHelper);
            ConsistencyCheckResults result = consistencyChecker.CheckConsistency(constraintList);

            var v1 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("U", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == false
                      select vertex).FirstOrDefault();
            var v2 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("V", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == false
                      select vertex).FirstOrDefault();
            var v3 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("U", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == true
                      select vertex).FirstOrDefault();
            var v4 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("V", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == true
                      select vertex).FirstOrDefault();


            Assert.That(result != null);
            if (v1.DistanceLabel == 0)
            {
                Assert.That(v2.DistanceLabel == 6);
                Assert.That(v3.DistanceLabel == 2);
                Assert.That(v4.DistanceLabel == 6);
            }
            else
            {
                Assert.That(v2.DistanceLabel == 0);
                Assert.That(v1.DistanceLabel == 4);
                Assert.That(v3.DistanceLabel ==6);
                Assert.That(v4.DistanceLabel == 10);


            }


            // Test the system
            //ConsistencyCheckResults results = checker.CheckConsistency(constraintsList);

            // Assert the results

        }


        [Test]
        [Repeat (200)]
        public void CheckConsistency_NonIncremental_MultipleConstraints_FailureByNegativeCycle()
        {
            //populate the input data
            string constraint = "+U -V <= 6";
            var constraintList = new List<string>();
            constraintList.Add(constraint);
            constraint = "+V -U <= 4";
            constraintList.Add(constraint);
            constraint = "+U <= 1";
            constraintList.Add(constraint);
            constraint = "+U >= 3";
            constraintList.Add(constraint);
            IGraphHelper graphHelper = new GraphHelper();
            IGraphTraversalAlgorithms graphTraversalAlgorithm = new GraphTraversalAlgorithms();
            INegativeCycleDetection ncdAlgorithm = new NegativeCycleDetectionUsingGR(graphTraversalAlgorithm);
            // System under test
            ConsistencyChecker consistencyChecker = new NonIncrementalConsistencyChecker(ncdAlgorithm, graphTraversalAlgorithm,
                graphHelper);
            ConsistencyCheckResults result = consistencyChecker.CheckConsistency(constraintList);

            var v1 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("U", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == false
                      select vertex).FirstOrDefault();
            var v2 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("V", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == false
                      select vertex).FirstOrDefault();
            var v3 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("U", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == true
                      select vertex).FirstOrDefault();
            var v4 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("V", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == true
                      select vertex).FirstOrDefault();


            Assert.That(result != null);
            Assert.That(!result.IsConsistencyCheckSuccessful);
            Assert.That(result.FailedConstraints.Count==2);
            Assert.That(result.FailedConstraints.Contains("+U +U <= 2"));
            Assert.That(result.FailedConstraints.Contains("-U -U <= -6"));
            //if (v1.DistanceLabel == 0)
            //{
            //    Assert.That(v2.DistanceLabel == 6);
            //    Assert.That(v3.DistanceLabel == 2);
            //    Assert.That(v4.DistanceLabel == 6);
            //}
            //else
            //{
            //    Assert.That(v2.DistanceLabel == 0);
            //    Assert.That(v1.DistanceLabel == 4);
            //    Assert.That(v3.DistanceLabel == 6);
            //    Assert.That(v4.DistanceLabel == 10);


            //}


            

        }



        [Test]
        public void CheckConsistency_NonIncremental_MultipleConstraints_FailureByZUnsat()
        {
            //populate the input data
            string constraint = "+U -V <= 1";
            var constraintList = new List<string>();
            constraintList.Add(constraint);
            constraint = "+V -X <= 2";
            constraintList.Add(constraint);
            constraint = "+U -Y <= 4";
            constraintList.Add(constraint);
            constraint = "+Y -X <= 7";
            constraintList.Add(constraint);
            constraint = "+U +U <= 1";
            constraintList.Add(constraint);
            constraint = "-U -U <= -1";
            constraintList.Add(constraint);
            IGraphHelper graphHelper = new GraphHelper();
            IGraphTraversalAlgorithms graphTraversalAlgorithm = new GraphTraversalAlgorithms();
            INegativeCycleDetection ncdAlgorithm = new NegativeCycleDetectionUsingGR(graphTraversalAlgorithm);
            // System under test
            ConsistencyChecker consistencyChecker = new NonIncrementalConsistencyChecker(ncdAlgorithm, graphTraversalAlgorithm,
                graphHelper);
            ConsistencyCheckResults result = consistencyChecker.CheckConsistency(constraintList);

            var v1 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("U", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == false
                      select vertex).FirstOrDefault();
            var v2 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("V", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == false
                      select vertex).FirstOrDefault();
            var v3 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("U", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == true
                      select vertex).FirstOrDefault();
            var v4 = (from vertex in result.Graph.Vertices
                      where vertex.Name.Equals("V", StringComparison.InvariantCultureIgnoreCase) &&
                            vertex.IsNegative == true
                      select vertex).FirstOrDefault();


            Assert.That(result != null);
            Assert.That(!result.IsConsistencyCheckSuccessful);
            //Assert.That(result.FailedConstraints.Count == 2);
            //Assert.That(result.FailedConstraints.Contains("+U +U <= 2"));
            //Assert.That(result.FailedConstraints.Contains("-U -U <= -6"));
            //if (v1.DistanceLabel == 0)
            //{
            //    Assert.That(v2.DistanceLabel == 6);
            //    Assert.That(v3.DistanceLabel == 2);
            //    Assert.That(v4.DistanceLabel == 6);
            //}
            //else
            //{
            //    Assert.That(v2.DistanceLabel == 0);
            //    Assert.That(v1.DistanceLabel == 4);
            //    Assert.That(v3.DistanceLabel == 6);
            //    Assert.That(v4.DistanceLabel == 10);


            //}




        }

  

        #endregion
    }
}