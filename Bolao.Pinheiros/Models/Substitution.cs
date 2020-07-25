namespace Bolao.Pinheiros.Models
{
    public class Substitution
    {
        public int playerId { get; set; }
        public double time { get; set; }
        public double addedTime { get; set; }
        public int type { get; set; }
        public int status { get; set; }
        public int? eventOrder { get; set; }
    }
}