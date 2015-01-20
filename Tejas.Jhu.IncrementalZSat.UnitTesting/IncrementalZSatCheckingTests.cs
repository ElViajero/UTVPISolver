using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using QuickGraph;
using Tejas.Jhu.ConsistencyChecking;
using Tejas.Jhu.ConsistencyChecking.DataContracts;
using Tejas.Jhu.GraphUtilities;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.IncrementalQSatChecking;
using Tejas.Jhu.IncrementalZSatChecking;
using Tejas.Jhu.IncrementalZSatChecking.DataContracts;
using Tejas.Jhu.NegativeCycleDetection;

namespace Tejas.Jhu.IncrementalZSat.UnitTesting
{
    public class IncrementalZSatCheckingTests
    {
        [Test]
        [Repeat (200)]
        public void IncrementalZSatCheckingTest()
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
            IIncrementalQSatChecking qSatChecking = new IncrementalQSatChecking.IncrementalQSatChecking();

            var res=qSatChecking.CheckQSatisfiability(resultOfCheck.Graph,
                new TaggedEdge<VertexProperties, EdgeProperties>(new VertexProperties("X", int.MaxValue, true, false),
                    new VertexProperties("Y", 0, true, false), new EdgeProperties(int.MaxValue, -7)));
            Assert.That(res.ChangedPotentialValues.Count==1);

            IIncrementalZSatChecking zSatChecking = new IncrementalZSatChecking.IncrementalZSatChecking(graphHelper, graphTraversalAlgorithm);

            IncrementalZSatResults finalResult = zSatChecking.CheckZSatisfiability(res.ConstraintGraph, res.ConstraintEdge, res.ChangedPotentialValues);

            Assert.That(finalResult.IsConsistencyCheckSuccessful);
            Assert.That(finalResult.SourceRelevantShortestPathsList.Count == 1);
            Assert.That(finalResult.TargetRelevantShortestPathsList.Count == 3);
            Assert.That(finalResult.TargetRelevantShortestPathsList.ContainsKey(new VertexProperties("U", 0, false, false)));
            Assert.That(finalResult.TargetRelevantShortestPathsList.ContainsKey(new VertexProperties("V", 0, false, false)));
            Assert.That(finalResult.TargetRelevantShortestPathsList.ContainsKey(new VertexProperties("X", 0, false, false)));


        }

        [Test]
        [Repeat (200)]
        public void IncrementalZSatCheckingTest_UNSAT()
        {

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
                Assert.That(resultOfCheck.IsConsistencyCheckSuccessful);
                Assert.That(v1.DistanceLabel == 0);
                Assert.That(v2.DistanceLabel == 4);
                Assert.That(v3.DistanceLabel == 3);
                IIncrementalQSatChecking qSatChecking = new IncrementalQSatChecking.IncrementalQSatChecking();

                var res = qSatChecking.CheckQSatisfiability(resultOfCheck.Graph,
                    new TaggedEdge<VertexProperties, EdgeProperties>(new VertexProperties("U", int.MaxValue, true,true),
                        new VertexProperties("U", 0, true, false), new EdgeProperties(int.MaxValue, -1)));
              //  Assert.That(res.ChangedPotentialValues.Count == 1);

                IIncrementalZSatChecking zSatChecking = new IncrementalZSatChecking.IncrementalZSatChecking(graphHelper, graphTraversalAlgorithm);

                IncrementalZSatResults finalResult = zSatChecking.CheckZSatisfiability(res.ConstraintGraph, res.ConstraintEdge, res.ChangedPotentialValues);

                Assert.That(!finalResult.IsConsistencyCheckSuccessful);
                //Assert.That(finalResult.SourceRelevantShortestPathsList.Count == 1);
                //Assert.That(finalResult.TargetRelevantShortestPathsList.Count == 3);
                //Assert.That(finalResult.TargetRelevantShortestPathsList.ContainsKey(new VertexProperties("U", 0, false, false)));
                //Assert.That(finalResult.TargetRelevantShortestPathsList.ContainsKey(new VertexProperties("V", 0, false, false)));
                //Assert.That(finalResult.TargetRelevantShortestPathsList.ContainsKey(new VertexProperties("X", 0, false, false)));


            }
        }

    }
}