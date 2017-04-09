namespace ChopSuey.AggregateQueries
{
    public class QuerySummary
    {
        public int Hits { get; set; }
        public int Errors { get; set; }
        public string State { get; set; }
        public AggregateQuery Query { get; set; }
    }
}