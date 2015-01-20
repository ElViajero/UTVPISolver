namespace Tejas.Jhu.IncrementalQSatChecking.DataContracts
{
    public class PseudoSlackValue
    {
        public int PseudoSlack { get; set; }
        public int NumberOfEdges { get; set; }

        public PseudoSlackValue(int pseudoSlack,int numberOfEdges)
        {
            PseudoSlack = pseudoSlack;
            NumberOfEdges = numberOfEdges;
        }
 
    }
}