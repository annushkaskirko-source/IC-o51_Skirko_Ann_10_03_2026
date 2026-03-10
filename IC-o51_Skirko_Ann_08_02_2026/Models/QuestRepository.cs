using IC_o51_Skirko_Ann_08_02_2026.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IC_o51_Skirko_Ann_08_02_2026
{
    // Репозиторій для роботи з квестами
    public class QuestRepository : IRepository<Quest>
    {
        private List<Quest> _quests;
        private int _nextId;

        // Конструктор
        public QuestRepository()
        {
            _quests = new List<Quest>();
            _nextId = 1;
        }

        // Кількість квестів
        public int Count => _quests.Count;

        // Додати новий квест
        public void Add(Quest item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            item.Id = _nextId++; // генеруємо унікальний ID
            _quests.Add(item);
        }

        // Видалити квест
        public bool Remove(Quest item)
        {
            if (item == null)
                return false;

            return _quests.Remove(item);
        }

        // Отримати квест за ID
        public Quest GetById(int id)
        {
            return _quests.FirstOrDefault(q => q.Id == id);
        }

        // Отримати всі квести
        public IEnumerable<Quest> GetAll()
        {
            return _quests.ToList(); // повертаємо копію
        }

        // Оновити квест
        public void Update(Quest item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var existingQuest = GetById(item.Id);
            if (existingQuest == null)
                throw new InvalidOperationException($"Квест з ID {item.Id} не знайдено.");

            // Оновлюємо квест у списку
            int index = _quests.IndexOf(existingQuest);
            _quests[index] = item;
        }

        // Знайти квести за умовою
        public IEnumerable<Quest> Find(Predicate<Quest> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return _quests.FindAll(predicate);
        }

        // Додаткові методи для зручності

        // Отримати активні квести
        public IEnumerable<Quest> GetActiveQuests()
        {
            return Find(q => q.Status == QuestStatus.Active);
        }

        // Отримати виконані квести
        public IEnumerable<Quest> GetCompletedQuests()
        {
            return Find(q => q.Status == QuestStatus.Completed);
        }

        // Отримати квести за категорією
        public IEnumerable<Quest> GetQuestsByCategory(QuestCategory category)
        {
            return Find(q => q.Category == category);
        }

        // Отримати квести за складністю
        public IEnumerable<Quest> GetQuestsByDifficulty(QuestDifficulty difficulty)
        {
            return Find(q => q.Difficulty == difficulty);
        }
        public void AddFromLoad(Quest quest)
        {
            _quests.Add(quest);

            if (quest.Id >= _nextId)
            {
                _nextId = quest.Id + 1;
            }
        }
       
        // Очистити всі квести
        public void Clear()
        {
            _quests.Clear();
            _nextId = 1;
        }
    }
}
