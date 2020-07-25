namespace Bolao.Pinheiros.Models
{
    public class Bookmaker
    {
        public int id { get; set; }
        public string name { get; set; }
        public string link { get; set; }
        public string nameForURL { get; set; }
        public ActionButton actionButton { get; set; }
        public string color { get; set; }
    }
}