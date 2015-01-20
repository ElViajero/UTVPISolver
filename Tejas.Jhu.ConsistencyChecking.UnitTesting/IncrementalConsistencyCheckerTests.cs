using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using NUnit.Framework;
using QuickGraph;
using Tejas.Jhu.ConsistencyChecking.DataContracts;
using Tejas.Jhu.GraphUtilities;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.IncrementalQSatChecking;
using Tejas.Jhu.IncrementalZSatChecking;
using Tejas.Jhu.NegativeCycleDetection;

namespace Tejas.Jhu.ConsistencyChecking.UnitTesting
{
    [TestFixture]
    public class IncrementalConsistencyCheckerTests
     {
        [Test]
        [Repeat(200)]
        public void CheckIncrementalConsistency_SingleNewConstraint()
        {

            IList<string> constraintList = new List<string>();
            string constraint1 = "+U -V <= 1";
            string constraint2 = "+U -Y <= 4";
            string constraint3 = "+Y -X <= 7";
            string constraint4 = "+V -X <= 2";
            //            string constraint5 = "+U -X <= 2";

            constraintList.Add(constraint1);
            constraintList.Add(constraint2);
            constraintList.Add(constraint3);
            constraintList.Add(constraint4);
            //constraintList.Add(constraint5);

            IGraphHelper graphHelper = new GraphHelper();
            IGraphTraversalAlgorithms graphTraversalAlgorithm = new GraphTraversalAlgorithms();



            INegativeCycleDetection ncdAlgorithm = new NegativeCycleDetectionUsingGR(graphTraversalAlgorithm);

            ConsistencyChecker consistencyChecker = new NonIncrementalConsistencyChecker(ncdAlgorithm,
                graphTraversalAlgorithm, graphHelper);
            ConsistencyCheckResults resultOfCheck = consistencyChecker.CheckConsistency(constraintList);

            var v1 = resultOfCheck.Graph.Vertices.FirstOrDefault(p => p.Name.Equals("U") && p.IsNegative == false);
            var v2 = resultOfCheck.Graph.Vertices.FirstOrDefault(p => p.Name.Equals("Y") && p.IsNegative == false);
            var v3 = resultOfCheck.Graph.Vertices.FirstOrDefault(p => p.Name.Equals("X") && p.IsNegative == false);
            Assert.That(v1.DistanceLabel == 0);
            Assert.That(v2.DistanceLabel == 4);
            Assert.That(v3.DistanceLabel == 3);

            string incrementalConstraint = "+X -Y <= -7";
            List<string> incrementalConstraintList=new List<string>();
            incrementalConstraintList.Add(incrementalConstraint);
            IIncrementalQSatChecking qSatChecking = new IncrementalQSatChecking.IncrementalQSatChecking();
            IIncrementalZSatChecking zSatChecking = new IncrementalZSatChecking.IncrementalZSatChecking(graphHelper,
                graphTraversalAlgorithm);
            ConsistencyChecker checker= new IncrementalConsistencyChecker(graphHelper,graphTraversalAlgorithm,qSatChecking,zSatChecking);
            var finalResults = checker.CheckConsistency(resultOfCheck.Graph, incrementalConstraint);
            Assert.That(finalResults.IsConsistencyCheckSuccessful);
            Assert.That(finalResults.Graph.EdgeCount==10);

        }




        [Test]
        [Repeat(200)]
        public void CheckIncrementalConsistency_SingleNewConstraint_UNSAT()
        {

            IList<string> constraintList = new List<string>();
            string constraint1 = "+U -V <= 1";
            string constraint2 = "+U -Y <= 4";
            string constraint3 = "+Y -X <= 7";
            string constraint4 = "+V -X <= 2";
                        string constraint5 = "+U +U <= 1";

            constraintList.Add(constraint1);
            constraintList.Add(constraint2);
            constraintList.Add(constraint3);
            constraintList.Add(constraint4);
            constraintList.Add(constraint5);

            IGraphHelper graphHelper = new GraphHelper();
            IGraphTraversalAlgorithms graphTraversalAlgorithm = new GraphTraversalAlgorithms();



            INegativeCycleDetection ncdAlgorithm = new NegativeCycleDetectionUsingGR(graphTraversalAlgorithm);

            ConsistencyChecker consistencyChecker = new NonIncrementalConsistencyChecker(ncdAlgorithm,
                graphTraversalAlgorithm, graphHelper);
            ConsistencyCheckResults resultOfCheck = consistencyChecker.CheckConsistency(constraintList);

            var v1 = resultOfCheck.Graph.Vertices.FirstOrDefault(p => p.Name.Equals("U") && p.IsNegative == false);
            var v2 = resultOfCheck.Graph.Vertices.FirstOrDefault(p => p.Name.Equals("Y") && p.IsNegative == false);
            var v3 = resultOfCheck.Graph.Vertices.FirstOrDefault(p => p.Name.Equals("X") && p.IsNegative == false);
            Assert.That(v1.DistanceLabel == 0);
            Assert.That(v2.DistanceLabel == 4);
            Assert.That(v3.DistanceLabel == 3);

            string incrementalConstraint = "-U -U <= -1";
            List<string> incrementalConstraintList = new List<string>();
            incrementalConstraintList.Add(incrementalConstraint);
            IIncrementalQSatChecking qSatChecking = new IncrementalQSatChecking.IncrementalQSatChecking();
            IIncrementalZSatChecking zSatChecking = new IncrementalZSatChecking.IncrementalZSatChecking(graphHelper,
                graphTraversalAlgorithm);
            ConsistencyChecker checker = new IncrementalConsistencyChecker(graphHelper, graphTraversalAlgorithm, qSatChecking, zSatChecking);
            var finalResults = checker.CheckConsistency(resultOfCheck.Graph, incrementalConstraint);
            Assert.That(!finalResults.IsConsistencyCheckSuccessful);
            //Assert.That(finalResults.Graph.EdgeCount == 10);

        }

     }
}