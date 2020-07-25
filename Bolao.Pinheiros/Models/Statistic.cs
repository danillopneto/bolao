namespace Bolao.Pinheiros.Models
{
    public class Statistic
    {
        public int id { get; set; }
        public string name { get; set; }
        public int categoryId { get; set; }
        public string categoryName { get; set; }
        public bool isMajor { get; set; }
        public string value { get; set; }
        public double valuePercentage { get; set; }
        public bool isPrimary { get; set; }
    }
}