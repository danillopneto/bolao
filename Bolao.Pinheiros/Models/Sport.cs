namespace Bolao.Pinheiros.Models
{
    public class Sport
    {
        public int id { get; set; }
        public string name { get; set; }
        public string nameForURL { get; set; }
        public bool drawSupport { get; set; }
        public int? totalGames { get; set; }
        public int? liveGames { get; set; }
    }
}