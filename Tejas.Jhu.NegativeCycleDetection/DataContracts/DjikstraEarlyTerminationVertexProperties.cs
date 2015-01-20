using System;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;

namespace Tejas.Jhu.NegativeCycleDetection.DataContracts
{
    public class DjikstraEarlyTerminationVertexProperties : IComparable<DjikstraEarlyTerminationVertexProperties>
    {
        public DjikstrasVertexProperties Vertex { get; set; }
        public bool IsRelevant { get; private set; }

        public DjikstraEarlyTerminationVertexProperties(DjikstrasVertexProperties vertex, bool isRelevant)
        {
            Vertex = vertex;
            IsRelevant = isRelevant;
        }

        public void SetIsRelevant(bool isRelevant)
        {
            IsRelevant = isRelevant;
        }


        public int CompareTo(DjikstraEarlyTerminationVertexProperties other)
        {
            if (IsRelevant == other.IsRelevant)
                return Vertex.CompareTo(other.Vertex);
            if (other.IsRelevant)
                return 1;
            
            return -1;

        }
    }
}