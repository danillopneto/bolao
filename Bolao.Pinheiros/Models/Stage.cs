namespace Bolao.Pinheiros.Models
{
    public class Stage
    {
        public int id { get; set; }
        public string name { get; set; }
        public string shortName { get; set; }
        public double homeCompetitorScore { get; set; }
        public double awayCompetitorScore { get; set; }
        public bool isEnded { get; set; }
        public bool? isCurrent { get; set; }
    }
}