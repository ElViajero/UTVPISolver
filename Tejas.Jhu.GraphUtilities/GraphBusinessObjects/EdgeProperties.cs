namespace Tejas.Jhu.GraphUtilities.GraphBusinessObjects
{
    public class EdgeProperties
    {
        public int Slack { get; private set; }
        public int Weight { get; private set; }

        public EdgeProperties(int slack, int weight)
        {
            Slack = slack;
            Weight = weight;
        }

        public void SetSlack(int slack)
        {
            Slack = slack;
        }
    }
}