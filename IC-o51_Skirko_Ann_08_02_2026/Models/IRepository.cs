using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IC_o51_Skirko_Ann_08_02_2026
{
    // Узагальнений інтерфейс репозиторію
    public interface IRepository<T> where T : class
    {
        // Додати новий елемент
        void Add(T item);

        // Видалити елемент
        bool Remove(T item);

        // Отримати елемент за ID
        T GetById(int id);

        // Отримати всі елементи
        IEnumerable<T> GetAll();

        // Оновити існуючий елемент
        void Update(T item);

        // Знайти елементи за умовою
        IEnumerable<T> Find(Predicate<T> predicate);

        // Кількість елементів
        int Count { get; }
    }
}
