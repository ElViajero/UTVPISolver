using System;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;
using Tejas.Jhu.NegativeCycleDetection.DataContracts;

namespace Tejas.Jhu.IncrementalQSatChecking.DataContracts
{
    public class PseudoSlackProperties : IComparable<PseudoSlackProperties>
    {
        public PseudoSlackValue Value { get; set; }
        public VertexProperties Vertex;

        public PseudoSlackProperties(VertexProperties vertex, PseudoSlackValue value)
        {
            Vertex = vertex;
            Value = value;
        }

        public int CompareTo(PseudoSlackProperties other)
        {
            if(Vertex.Equals(other.Vertex))//erase this clause to roll back
            return 0;
            if (other.Value.NumberOfEdges == int.MaxValue && other.Value.PseudoSlack == 0 )//erase this clause to roll back
                return -1;
            if (Value.PseudoSlack < other.Value.PseudoSlack ||(Value.PseudoSlack==other.Value.PseudoSlack && Value.NumberOfEdges < other.Value.NumberOfEdges))
                return -1;
            if (Value.PseudoSlack == other.Value.PseudoSlack && Value.NumberOfEdges == other.Value.NumberOfEdges && Vertex.Equals(other.Vertex))
                return 0;

                return 1;
        }
    }
}