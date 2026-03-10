using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IC_o51_Skirko_Ann_08_02_2026.Models
{
    public class GameData
    {
        public string PlayerName { get; set; }
        public int PlayerLevel { get; set; }
        public int PlayerExperience { get; set; }

        public List<QuestDto> Quests { get; set; } = new List<QuestDto>();
    }
    public class QuestDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public QuestCategory Category { get; set; }
        public QuestDifficulty Difficulty { get; set; }
        public QuestStatus Status { get; set; }

        public int ExperienceReward { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
}
