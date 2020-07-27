using System.Collections.Generic;

namespace Bolao.Pinheiros.Models
{
    public class Event
    {
        public int competitorId { get; set; }
        public int statusId { get; set; }
        public int stageId { get; set; }
        public int order { get; set; }
        public int num { get; set; }
        public double gameTime { get; set; }
        public int addedTime { get; set; }
        public string gameTimeDisplay { get; set; }
        public int gameTimeAndStatusDisplayType { get; set; }
        public int playerId { get; set; }
        public bool isMajor { get; set; }
        public EventType eventType { get; set; }
        public List<int> extraPlayers { get; set; }

        public bool IsGoal()
        {
            return eventType.name == Game.GOAL_NAME;
        }
        
        public bool IsTeamGoal(int competitorId)
        {
            return IsGoal() && this.competitorId == competitorId;
        }
    }
}