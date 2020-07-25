using System.Collections.Generic;

namespace Bolao.Pinheiros.Models
{
    public class GameData
    {
        public long lastUpdateId { get; set; }
        public int requestedUpdateId { get; set; }
        public int ttl { get; set; }
        public Game game { get; set; }
        public List<Sport> sports { get; set; }
        public List<Country> countries { get; set; }
        public List<Competition> competitions { get; set; }
    }
}