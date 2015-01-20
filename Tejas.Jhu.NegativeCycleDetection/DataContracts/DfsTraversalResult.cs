using System.Collections.Generic;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;

namespace Tejas.Jhu.NegativeCycleDetection.DataContracts
{
    public class DfsTraversalResult
    {
        public List<VertexProperties> ListOfVertices { get; private set; }
        public bool NegativeCycleDetected { get; private set; }

        public DfsTraversalResult(List<VertexProperties> listOfVertices, bool negativeCycleDetected)
        {
            ListOfVertices = listOfVertices;
            NegativeCycleDetected = negativeCycleDetected;
        }
    }
}