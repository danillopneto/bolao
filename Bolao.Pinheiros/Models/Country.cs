namespace Bolao.Pinheiros.Models
{
    public class Country
    {
        public int id { get; set; }
        public string name { get; set; }
        public int totalGames { get; set; }
        public int liveGames { get; set; }
        public string nameForURL { get; set; }
    }
}