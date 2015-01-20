using System;

namespace Tejas.Jhu.GraphUtilities.GraphBusinessObjects
{
    public class DjikstrasVertexProperties:IComparable<DjikstrasVertexProperties>
    {
        public VertexProperties Vertex { get; private set; }
        public VertexProperties ParentVertex { get; private set; }
        public int NumberOfEdges { get; private set; }
        public int DjikstrasDistanceLabel { get; private set; }

        public DjikstrasVertexProperties(VertexProperties vertex,VertexProperties parentVertex, int djikstrasDistanceLabel)
        {
            Vertex = vertex;
            ParentVertex = parentVertex;
            DjikstrasDistanceLabel = djikstrasDistanceLabel;
            NumberOfEdges = int.MaxValue;
        }

        public DjikstrasVertexProperties(VertexProperties vertex, VertexProperties parentVertex, int djikstrasDistanceLabel, int numberOfEdges)
        {
            Vertex = vertex;
            ParentVertex = parentVertex;
            DjikstrasDistanceLabel = djikstrasDistanceLabel;
            NumberOfEdges = numberOfEdges;
        }

        public void SetDjikstrasDistanceLabel(int djikstrasDistanceLabel)
        {
            DjikstrasDistanceLabel = djikstrasDistanceLabel;
        }

        public void SetParentVertex(VertexProperties parentVertex)
        {
            ParentVertex = parentVertex;
        }

        public void SetNumberOfEdges(int numberOfEdges)
        {
            NumberOfEdges = numberOfEdges;
        }

        public int CompareTo(DjikstrasVertexProperties other)
        {
            if (Vertex.Equals(other.Vertex)) //&&                                 
                return 0;
             if (DjikstrasDistanceLabel < other.DjikstrasDistanceLabel || 
                 (DjikstrasDistanceLabel==other.DjikstrasDistanceLabel && NumberOfEdges<other.NumberOfEdges))
                return -1;                         
                
            return 1;
            //return -2;
        }
    }
}