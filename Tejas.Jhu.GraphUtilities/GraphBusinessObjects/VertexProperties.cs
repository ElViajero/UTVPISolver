using System;

namespace Tejas.Jhu.GraphUtilities.GraphBusinessObjects
{
    public class VertexProperties : Object

    {
        public string Name { get; private set; }
        public int DistanceLabel { get; private set; }
        public bool Visited { get; private set; }
        public bool IsNegative { get; private set; }

        public VertexProperties(string name, int distanceLabel, bool visited,bool isNegative)
        {
            Name = name;
            DistanceLabel = distanceLabel;
            Visited = visited;
            IsNegative = isNegative;
        }

        public void SetDistanceLabel(int distanceLabel)
        {
            DistanceLabel = distanceLabel;
        }

        public void SetVisited(bool flag)
        {
            Visited = flag;
        }

        public string GetVertexName()
        {
            return IsNegative ? string.Format("{0}-", Name) : string.Format("{0}+", Name);
        }

        public int CompareTo(VertexProperties other)
        {
            if (Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase) && IsNegative==other.IsNegative)
                return 0;
                                    
            if (DistanceLabel < other.DistanceLabel)
                return -1;
            return 1;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            VertexProperties otherVertex = (VertexProperties)obj;
            if (IsNegative == otherVertex.IsNegative && Name.Equals(otherVertex.Name) )//&& Visited==otherVertex.Visited)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (Name.GetHashCode() + IsNegative.GetHashCode());
        }
    }
}