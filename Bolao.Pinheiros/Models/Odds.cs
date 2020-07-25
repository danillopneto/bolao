using System.Collections.Generic;

namespace Bolao.Pinheiros.Models
{
    public class Odds
    {
        public int lineId { get; set; }
        public int gameId { get; set; }
        public int bookmakerId { get; set; }
        public int lineTypeId { get; set; }
        public string link { get; set; }
        public string trackingUrl { get; set; }
        public Bookmaker bookmaker { get; set; }
        public List<Option> options { get; set; }
    }
}