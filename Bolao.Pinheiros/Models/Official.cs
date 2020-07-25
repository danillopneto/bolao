namespace Bolao.Pinheiros.Models
{
    public class Official
    {
        public int id { get; set; }
        public int athleteId { get; set; }
        public int countryId { get; set; }
        public int status { get; set; }
        public string name { get; set; }
        public string nameForURL { get; set; }
    }
}