using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using QuickGraph;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;

namespace Tejas.Jhu.GraphUtilities
{
    public class GraphHelper : IGraphHelper
    {
        #region private class properties

        private static BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> ConstraintGraph;
        
        
        #endregion
        /// <summary>
        /// The method converts a list of constraints (given as input in the form of a list) to a bi-directional graph.
        /// The graph can then be used for negative cycle detection etc...
        /// </summary>
        /// <param name="constraintsList">List of constraints</param>
        /// <returns>A bi-directional graph representation of the constraints</returns>
        public BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>
            ConvertConstraintsToGraph(IList<string> constraintsList)
        {

            ConstraintGraph = new BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>>();
            List<TaggedEdge<VertexProperties, EdgeProperties>> edgeList=ConvertConstriantsToEdges(constraintsList);
        
            foreach (TaggedEdge<VertexProperties, EdgeProperties> currentEdge in edgeList)
            {
             if(!ConstraintGraph.ContainsVertex(currentEdge.Source))
                ConstraintGraph.AddVertex(currentEdge.Source);
                if(!ConstraintGraph.ContainsVertex(currentEdge.Target))
                ConstraintGraph.AddVertex(currentEdge.Target);
                ConstraintGraph.AddEdge(currentEdge);
            }

            return ConstraintGraph;
        }

        public List<string> RetrieveConstraintsFromCycle(int startIndex, int endIndex, IList<VertexProperties> vertexList,
            BidirectionalGraph<VertexProperties, TaggedEdge<VertexProperties, EdgeProperties>> graph)
        {
            List<string> constraintList = new List<string>();
            String constraintString = "";
            //check if it is a negative cycle

            #region Process negative cycle

            if (startIndex == -1 && endIndex == -1)
            {
                //process the constraints in the cycle
                for (int index = 0; index < vertexList.Count; index++)
                {
                    constraintString = "";
                    if (vertexList.ElementAt((index)%vertexList.Count).IsNegative)
                        constraintString = string.Concat(constraintString, "-",
                            vertexList.ElementAt(index%vertexList.Count).Name, " ");
                    else
                    {
                        constraintString = string.Concat(constraintString, "+",
                            vertexList.ElementAt(index%vertexList.Count).Name, " ");
                    }
                    if (vertexList.ElementAt((index + 1)%vertexList.Count).IsNegative)
                        constraintString = string.Concat(constraintString, "+",
                            vertexList.ElementAt((index + 1)%vertexList.Count).Name, " <= ");
                    else
                    {
                        constraintString = string.Concat(constraintString, "-",
                            vertexList.ElementAt((index + 1)%vertexList.Count).Name, " <= ");
                    }

                    int edgeWeight = (from edge in graph.Edges
                        where
                            edge.Source.Name.Equals(vertexList.ElementAt((index)%vertexList.Count).Name,
                                StringComparison.InvariantCultureIgnoreCase)
                            && edge.Source.IsNegative == vertexList.ElementAt((index)%vertexList.Count).IsNegative
                            &&
                            edge.Target.Name.Equals(vertexList.ElementAt((index + 1)%vertexList.Count).Name,
                                StringComparison.InvariantCultureIgnoreCase)
                            && edge.Target.IsNegative == vertexList.ElementAt((index + 1)%vertexList.Count).IsNegative
                        select edge.Tag.Weight).FirstOrDefault();

                    constraintString = string.Concat(constraintString, edgeWeight.ToString());
                    constraintList.Add(constraintString);

                }
                //join the


            }
                #endregion

                #region Process strongly connected component

            else
            {
                bool isCycleCompleted = false;
                int currentIndex = startIndex;
                //string constraintString = "";
                constraintString = "";
                int constraintWeight;

                int weightFromStartIndexToEndIndex = (from edge in graph.Edges
                    where vertexList.Contains(edge.Source)
                          && vertexList.Contains(edge.Target)
                          && vertexList.IndexOf(edge.Source) + 1 == vertexList.IndexOf(edge.Target)
                          && vertexList.IndexOf(edge.Source) >= startIndex
                        //&& vertexList.IndexOf(edge.Target) > startIndex 
                        //&& vertexList.IndexOf(edge.Source) < endIndex
                          && vertexList.IndexOf(edge.Target) <= endIndex
                    select edge.Tag.Weight).Sum();

                if (vertexList.ElementAt((startIndex)%vertexList.Count).IsNegative)
                    constraintString = string.Concat(constraintString, "-",
                        vertexList.ElementAt(startIndex%vertexList.Count).Name, " ");
                else
                {
                    constraintString = string.Concat(constraintString, "+",
                        vertexList.ElementAt(startIndex%vertexList.Count).Name, " ");
                }
                if (vertexList.ElementAt((endIndex)%vertexList.Count).IsNegative)
                    constraintString = string.Concat(constraintString, "+",
                        vertexList.ElementAt((endIndex)%vertexList.Count).Name, " <= ");
                else
                {
                    constraintString = string.Concat(constraintString, "-",
                        vertexList.ElementAt((endIndex)%vertexList.Count).Name, " <= ");
                }

                constraintWeight = weightFromStartIndexToEndIndex;

                constraintString = string.Concat(constraintString, constraintWeight.ToString());

                constraintList.Add(constraintString);

                //vertexList.Reverse();

                constraintString = "";

                //all nodes upto startIndex
                int weightFromZeroToStartIndex = (from edge in graph.Edges
                    where vertexList.Contains(edge.Source)
                          && vertexList.Contains(edge.Target)
                          && vertexList.IndexOf(edge.Source) + 1 == vertexList.IndexOf(edge.Target)
                          && vertexList.IndexOf(edge.Target) <= startIndex
                    select edge.Tag.Weight).Sum();

                //All nodes from endIndex to the end of the vertex list
                int weightFromEndindexToEnd = (from edge in graph.Edges
                    where vertexList.Contains(edge.Source)
                          && vertexList.Contains(edge.Target)
                          && vertexList.IndexOf(edge.Source) + 1 == vertexList.IndexOf(edge.Target)
                          && vertexList.IndexOf(edge.Source) >= endIndex
                    select edge.Tag.Weight).Sum();


                //now find the edge that connects the first and last vertices..

                int finalConnectingWeight = (from currentEdge in graph.Edges
                    where vertexList.Contains(currentEdge.Source)
                          && vertexList.Contains(currentEdge.Target)
                          && vertexList.IndexOf(currentEdge.Target) == 0
                          && vertexList.IndexOf(currentEdge.Source) == vertexList.Count - 1
                    select currentEdge.Tag.Weight).FirstOrDefault();


                //add all the weights to get the weight of the constraint..

                constraintWeight = weightFromZeroToStartIndex + weightFromEndindexToEnd + finalConnectingWeight;

                if (vertexList.ElementAt((endIndex)%vertexList.Count).IsNegative)
                    constraintString = string.Concat(constraintString, "-",
                        vertexList.ElementAt(endIndex%vertexList.Count).Name, " ");
                else
                {
                    constraintString = string.Concat(constraintString, "+",
                        vertexList.ElementAt(endIndex%vertexList.Count).Name, " ");
                }
                if (vertexList.ElementAt((startIndex)%vertexList.Count).IsNegative)
                    constraintString = string.Concat(constraintString, "+",
                        vertexList.ElementAt((startIndex)%vertexList.Count).Name, " <= ");
                else
                {
                    constraintString = string.Concat(constraintString, "-",
                        vertexList.ElementAt((startIndex)%vertexList.Count).Name, " <= ");
                }

                constraintString = string.Concat(constraintString, constraintWeight.ToString());

                constraintList.Add(constraintString);

            }


            #endregion

            return constraintList;
        }

        public List<TaggedEdge<VertexProperties,EdgeProperties>> ConvertConstriantsToEdges(IList<string> constraintList)
        {
             var edgeList = new List<TaggedEdge<VertexProperties, EdgeProperties>>();

            Parallel.ForEach(constraintList, currentConstraint =>
            {
                VertexProperties firstVertexPositive;
                VertexProperties firstVertexNegative;
                VertexProperties secondVertexPositive;
                VertexProperties secondVertexNegative;
                TaggedEdge<VertexProperties, EdgeProperties> originalEdge = null;
                TaggedEdge<VertexProperties, EdgeProperties> counterEdge = null;
                bool isFirstVariablePositive = false;
                bool isSecondVariablePositive = false;

                string firstVertexName = "";
                string secondVertexName = "";


                int weight = 0;
                string[] constraintProperties = Regex.Split(currentConstraint, @"\s+");

                #region Check variable coefficients

                if (!constraintProperties[0].Contains("-"))
                    isFirstVariablePositive = true;
                firstVertexName = constraintProperties[0].Substring(1);


                if (constraintProperties.Length == 4)
                {
                    if (!constraintProperties[1].Contains("-"))
                        isSecondVariablePositive = true;
                    secondVertexName = constraintProperties[1].Substring(1);
                }


                #endregion

                #region For constraints involving two distinct variables

                // to accept only two variable constraints in which both variables are distinct.
                if (constraintProperties.Length == 4 &&
                    !(firstVertexName.Equals(secondVertexName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    try
                    {
                        weight = Convert.ToInt32(constraintProperties[3]);
                    }
                    catch (Exception e)
                    {
                        Console.Write(e);
                    }

                    firstVertexPositive = new VertexProperties(firstVertexName, int.MaxValue, false,
                        false);
                    firstVertexNegative = new VertexProperties(firstVertexName, int.MaxValue, false,
                        true);
                    secondVertexPositive = new VertexProperties(secondVertexName, int.MaxValue, false,
                        false);
                    secondVertexNegative = new VertexProperties(secondVertexName, int.MaxValue, false,
                        true);


                    // x - y < d
                    if (isFirstVariablePositive && !isSecondVariablePositive &&
                        constraintProperties[2].Equals("<=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        originalEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (firstVertexPositive, secondVertexPositive,
                                    new EdgeProperties(0, weight));

                        counterEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (secondVertexNegative, firstVertexNegative,
                                    new EdgeProperties(0, weight));
                    }
                        // x - y > d
                    else if (isFirstVariablePositive && !isSecondVariablePositive
                             && constraintProperties[2].Equals(">=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        weight = -weight;

                        originalEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (secondVertexPositive, firstVertexPositive,
                                    new EdgeProperties(0, weight));

                        counterEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (firstVertexNegative, secondVertexNegative,
                                    new EdgeProperties(0, weight));
                    }

                        // x + y < d
                    else if (isFirstVariablePositive && isSecondVariablePositive
                             && constraintProperties[2].Equals("<=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        originalEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (firstVertexPositive, secondVertexNegative,
                                    new EdgeProperties(0, weight));

                        counterEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (secondVertexPositive, firstVertexNegative,
                                    new EdgeProperties(0, weight));
                    }
                        // x + y > d
                    else if (isFirstVariablePositive && isSecondVariablePositive
                             && constraintProperties[2].Equals(">=", StringComparison.InvariantCultureIgnoreCase))
                    {

                        weight = -weight;

                        originalEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (firstVertexNegative, secondVertexPositive,
                                    new EdgeProperties(0, weight));

                        counterEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (secondVertexNegative, firstVertexPositive,
                                    new EdgeProperties(0, weight));
                    }

                        // -x -y < d
                    else if (!isFirstVariablePositive && !isSecondVariablePositive &&
                             constraintProperties[2].Equals("<=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        originalEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (firstVertexNegative, secondVertexPositive,
                                    new EdgeProperties(0, weight));

                        counterEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (secondVertexNegative, firstVertexPositive,
                                    new EdgeProperties(0, weight));

                    }

                        // - x - y > d
                    else if (!isFirstVariablePositive && !isSecondVariablePositive &&
                             constraintProperties[2].Equals(">=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        weight = -weight;
                        originalEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (firstVertexPositive, secondVertexNegative,
                                    new EdgeProperties(0, weight));

                        counterEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (secondVertexPositive, firstVertexNegative,
                                    new EdgeProperties(0, weight));

                    }

                        // -x +y < d
                    else if (!isFirstVariablePositive && isSecondVariablePositive &&
                             constraintProperties[2].Equals("<=", StringComparison.InvariantCultureIgnoreCase))
                    {

                        originalEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (firstVertexNegative, secondVertexNegative,
                                    new EdgeProperties(0, weight));

                        counterEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (secondVertexPositive, firstVertexPositive,
                                    new EdgeProperties(0, weight));

                    }

                        // -x + y > d
                    else if (!isFirstVariablePositive && isSecondVariablePositive &&
                             constraintProperties[2].Equals(">=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        weight = -weight;
                        originalEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (firstVertexPositive, secondVertexPositive,
                                    new EdgeProperties(0, weight));

                        counterEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (secondVertexNegative, firstVertexNegative,
                                    new EdgeProperties(0, weight));

                    }

                    //add edges to list
                    lock (edgeList)
                    {
                        edgeList.Add(originalEdge);
                        edgeList.Add(counterEdge);
                    }
                }

                    #endregion

                # region For constraints involving one variable

                else if (constraintProperties.Length == 3)
                {
                    firstVertexPositive = new VertexProperties(firstVertexName, int.MaxValue, false,
                        false);
                    firstVertexNegative = new VertexProperties(firstVertexName, int.MaxValue, false,
                        true);

                    try
                    {
                        weight = Convert.ToInt32(constraintProperties[2]);
                    }
                    catch (Exception e)
                    {
                        Console.Write(e);
                    }

                    weight *= 2;
                    // x < d
                    if (isFirstVariablePositive &&
                        constraintProperties[1].Equals("<=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        originalEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (firstVertexPositive, firstVertexNegative,
                                    new EdgeProperties(0, weight));

                    }
                        // x > d
                    else if (isFirstVariablePositive &&
                             constraintProperties[1].Equals(">=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        weight = -weight;
                        originalEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (firstVertexNegative, firstVertexPositive,
                                    new EdgeProperties(0, weight));
                    }

                        // -x < d
                    else if (!isFirstVariablePositive &&
                             constraintProperties[1].Equals("<=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        originalEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (firstVertexNegative, firstVertexPositive,
                                    new EdgeProperties(0, weight));
                    }

                        // -x > d
                    else if (!isFirstVariablePositive &&
                             constraintProperties[1].Equals(">=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        weight = -weight;

                        originalEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (firstVertexPositive, firstVertexNegative,
                                    new EdgeProperties(0, weight));
                    }

                    //add edge to Edgelist
                    lock (edgeList)
                    {
                        edgeList.Add(originalEdge);
                    }
                }

                    #endregion

                #region For constraints involving two similar variables
                   
                    //for constraints of the form x + x <=d or x - x <=d etc..
                else if (constraintProperties.Length == 4 &&
                         firstVertexName.Equals(secondVertexName, StringComparison.InvariantCultureIgnoreCase))
                {

                    firstVertexPositive = new VertexProperties(firstVertexName, int.MaxValue, false,
                        false);
                    firstVertexNegative = new VertexProperties(firstVertexName, int.MaxValue, false,
                        true);

                    try
                    {
                        weight = Convert.ToInt32(constraintProperties[3]);
                    }
                    catch (Exception e)
                    {
                        Console.Write(e);
                    }
                    // x + x < d
                    if (isFirstVariablePositive && isSecondVariablePositive &&
                        constraintProperties[2].Equals("<=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        originalEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (firstVertexPositive, firstVertexNegative,
                                    new EdgeProperties(0, weight));

                    }
                        // x + x > d
                    else if (isFirstVariablePositive && isSecondVariablePositive &&
                             constraintProperties[2].Equals(">=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        weight = -weight;
                        originalEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (firstVertexNegative, firstVertexPositive,
                                    new EdgeProperties(0, weight));
                    }
                        // -x -x < d
                    else if (!isFirstVariablePositive && !isSecondVariablePositive &&
                             constraintProperties[2].Equals("<=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        originalEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (firstVertexNegative, firstVertexPositive,
                                    new EdgeProperties(0, weight));
                    }
                        // -x -x > d 
                    else if (!isFirstVariablePositive && !isSecondVariablePositive &&
                             constraintProperties[2].Equals(">=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        weight = -weight;

                        originalEdge =
                            new TaggedEdge<VertexProperties, EdgeProperties>
                                (firstVertexPositive, firstVertexNegative,
                                    new EdgeProperties(0, weight));
                    }

                    //add edge to list
                    lock (edgeList)
                    {
                        edgeList.Add(originalEdge);
                    }

                }

                #endregion

                
            });

            return edgeList;
        }
        

    }
}