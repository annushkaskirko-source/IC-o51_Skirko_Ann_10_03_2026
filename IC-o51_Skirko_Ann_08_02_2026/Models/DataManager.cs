using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IC_o51_Skirko_Ann_08_02_2026.Models
{
    public class DataManager
    {
        private readonly GameManager _gameManager;

        private readonly string _filePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");

        public DataManager(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        // Збереження гри
        public bool SaveData()
        {
            try
            {
                var data = new GameData
                {
                    PlayerName = _gameManager.Player.Name,
                    PlayerLevel = _gameManager.Player.Level,
                    PlayerExperience = _gameManager.Player.Experience
                };

                foreach (var quest in _gameManager.GetAllQuests())
                {
                    data.Quests.Add(new QuestDto
                    {
                        Id = quest.Id,
                        Name = quest.Name,
                        Description = quest.Description,
                        Category = quest.Category,
                        Difficulty = quest.Difficulty,
                        Status = quest.Status,
                        ExperienceReward = quest.ExperienceReward,
                        CreatedDate = quest.CreatedDate,
                        CompletedDate = quest.CompletedDate
                    });
                }

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(_filePath, json);

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Завантаження гри
        public bool LoadData()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return false;

                string json = File.ReadAllText(_filePath);

                var data = JsonConvert.DeserializeObject<GameData>(json);

                if (data == null)
                    return false;

                // Завантажуємо гравця
                _gameManager.LoadPlayerData(
                    data.PlayerName,
                    data.PlayerLevel,
                    data.PlayerExperience);

                // очищаємо старі квести
                var repo = (QuestRepository)_gameManager.GetRepository();
                repo.Clear();

                foreach (var dto in data.Quests)
                {
                    var quest = new Quest(
                        dto.Name,
                        dto.Description,
                        dto.Category,
                        dto.Difficulty);

                    quest.Id = dto.Id;

                    if (dto.Status == QuestStatus.Completed)
                        quest.Complete();

                    quest.SetCreatedDate(dto.CreatedDate);
                    quest.SetCompletedDate(dto.CompletedDate);

                    repo.AddFromLoad(quest);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
