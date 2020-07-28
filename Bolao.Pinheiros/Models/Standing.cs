using System.Collections.Generic;

namespace Bolao.Pinheiros.Models
{
    public class Standing
    {
        public int competitionId { get; set; }
        public int seasonNum { get; set; }
        public int stageNum { get; set; }
        public string displayName { get; set; }
        public List<Group> groups { get; set; }
        public List<Header> headers { get; set; }
        public List<Row> rows { get; set; }
        public List<Destination> destinations { get; set; }
    }
}