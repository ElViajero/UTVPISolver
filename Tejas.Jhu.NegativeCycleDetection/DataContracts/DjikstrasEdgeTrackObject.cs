namespace Tejas.Jhu.NegativeCycleDetection.DataContracts
{
    public class DjikstrasEdgeTrackObject
    {
       public int Distance { get; set; }
        public int NumberOfEdges { get; set; }

        public DjikstrasEdgeTrackObject(int distance,int numberOfEdges)
        {
            Distance = distance;
            NumberOfEdges = numberOfEdges;
        }

    }
}