using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IC_o51_Skirko_Ann_08_02_2026.Models
{
    public class GameManager
    {
        private static GameManager _instance;
        private DataManager _dataManager;

        // Singleton Instance
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameManager();

                return _instance;
            }
        }

        public Player Player { get; private set; }

        // Використовуємо репозиторій замість List<Quest>
        private IRepository<Quest> _questRepository;

        // Приватний конструктор
        private GameManager()
        {
            Player = new Player("Anna");
            _questRepository = new QuestRepository();
            _dataManager = new DataManager(this);
        }
        public bool SaveGame()
        {
            return _dataManager.SaveData();
        }

        public bool LoadGame()
        {
            return _dataManager.LoadData();
        }

        public void LoadPlayerData(string name, int level, int experience)
        {
            Player.LoadData(name, level, experience);
        }
        // Додати квест
        public void AddQuest(Quest quest)
        {
            if (quest == null)
                throw new ArgumentNullException(nameof(quest));

            _questRepository.Add(quest);
        }

        // Видалити квест
        public bool RemoveQuest(Quest quest)
        {
            return _questRepository.Remove(quest);
        }

        // Завершити квест
        public void CompleteQuest(Quest quest)
        {
            if (quest == null)
                return;

            if (quest.Status == QuestStatus.Active)
            {
                quest.Complete();
                _questRepository.Update(quest); // оновлюємо в репозиторії
                Player.AddExperience(quest.ExperienceReward);
            }
        }

        // Отримати всі активні квести
        public IEnumerable<Quest> GetActiveQuests()
        {
            return ((QuestRepository)_questRepository).GetActiveQuests();
        }

        // Отримати всі виконані квести
        public IEnumerable<Quest> GetCompletedQuests()
        {
            return ((QuestRepository)_questRepository).GetCompletedQuests();
        }

        // Отримати квести за категорією
        public IEnumerable<Quest> GetQuestsByCategory(QuestCategory category)
        {
            return ((QuestRepository)_questRepository).GetQuestsByCategory(category);
        }

        // Статистика
        public int GetTotalQuestsCount()
        {
            return _questRepository.Count;
        }

        public int GetActiveQuestsCount()
        {
            return GetActiveQuests().Count();
        }

        public int GetCompletedQuestsCount()
        {
            return GetCompletedQuests().Count();
        }
        public void UpdateQuest(Quest quest,
                        string newName,
                        string newDescription,
                        QuestCategory newCategory,
                        QuestDifficulty newDifficulty)
        {
            if (quest == null)
                throw new ArgumentNullException(nameof(quest));

            quest.Edit(newName, newDescription, newCategory, newDifficulty);
            _questRepository.Update(quest);
        }
        public IEnumerable<Quest> GetAllQuests()
        {
            return _questRepository.GetAll();
        }
        public IRepository<Quest> GetRepository()
        {
            return _questRepository;
        }
    }
}
