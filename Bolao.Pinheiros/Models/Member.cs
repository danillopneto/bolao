namespace Bolao.Pinheiros.Models
{
    public class Member
    {
        public int id { get; set; }
        public int status { get; set; }
        public string statusText { get; set; }
        public Position position { get; set; }
        public Formation formation { get; set; }
        public YardFormation yardFormation { get; set; }
        public Substitution substitution { get; set; }
        public Injury injury { get; set; }
        public int competitorId { get; set; }
        public int athleteId { get; set; }
        public string name { get; set; }
        public string shortName { get; set; }
        public int jerseyNumber { get; set; }
        public string nameForURL { get; set; }
    }
}