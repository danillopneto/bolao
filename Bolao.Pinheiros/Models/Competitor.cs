namespace Bolao.Pinheiros.Models
{
    public class Competitor
    {
        public int id { get; set; }
        public int countryId { get; set; }
        public int sportId { get; set; }
        public string name { get; set; }
        public string nameForURL { get; set; }
        public int type { get; set; }
        public int popularityRank { get; set; }
        public string longName { get; set; }
        public string shortName { get; set; }
    }
}