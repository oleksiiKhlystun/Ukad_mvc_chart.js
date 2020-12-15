using System.Collections.Generic;

namespace test2MVC.Models
{
    public class QueryTiming
    {
        public string UrlValue { get; set; }
        public int Value { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }       
    }
    public class GroupQueryTiming
    {
        public IReadOnlyList<QueryTiming> Items { get; set; }
    }
}
