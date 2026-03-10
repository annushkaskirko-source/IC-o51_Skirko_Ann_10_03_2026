using IC_o51_Skirko_Ann_08_02_2026.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IC_o51_Skirko_Ann_08_02_2026
{
    public partial class Form1 : Form
    {
        private GameManager gameManager;

        public Form1()
        {
            InitializeComponent();

            gameManager = GameManager.Instance;

            // Ініціалізація ComboBox
            categoryComboBox.DataSource = Enum.GetValues(typeof(QuestCategory));
            difficultyComboBox.DataSource = Enum.GetValues(typeof(QuestDifficulty));

            // Категорії для фільтра
            var filterCategories = new List<object> { "Всі категорії" };
            filterCategories.AddRange(Enum.GetValues(typeof(QuestCategory)).Cast<object>());
            filterCategoryComboBox.DataSource = filterCategories;
            filterCategoryComboBox.SelectedIndex = 0;

            // Завантаження даних
            if (gameManager.LoadGame())
            {
                MessageBox.Show("Дані завантажено!", "Інформація",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            UpdateStats();
            RefreshQuestList();
        }
        // Збереження при закритті
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            gameManager.SaveGame();
            base.OnFormClosing(e);
        }
        // Обробник кнопки "Додати квест"
        private void addQuestButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(questTextBox.Text))
            {
                MessageBox.Show("Введіть назву квесту", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Quest quest = new Quest(
                    questTextBox.Text,
                    descriptionTextBox.Text ?? "Без опису",
                    (QuestCategory)categoryComboBox.SelectedItem,
                    (QuestDifficulty)difficultyComboBox.SelectedItem
                );

                gameManager.AddQuest(quest);

                RefreshQuestList();
                UpdateStats();

                questTextBox.Clear();
                descriptionTextBox.Clear();

                MessageBox.Show(
                    $"Квест '{quest.Name}' додано!\n" +
                    $"Категорія: {quest.Category}\n" +
                    $"Складність: {quest.Difficulty}",
                    "Успіх",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обробник кнопки "Виконати квест"
        private void completeQuestButton_Click(object sender, EventArgs e)
        {
            if (questListBox.SelectedItem == null)
            {
                MessageBox.Show("Оберіть квест для виконання", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Quest selectedQuest = (Quest)questListBox.SelectedItem;

            if (selectedQuest.Status == QuestStatus.Completed)
            {
                MessageBox.Show("Цей квест вже виконано!", "Увага",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            gameManager.CompleteQuest(selectedQuest);

            RefreshQuestList();
            UpdateStats();

            MessageBox.Show(
                $"Квест '{selectedQuest.Name}' виконано!\n" +
                $"+{selectedQuest.ExperienceReward} XP",
                "Вітаємо!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        // Оновити список квестів
        private void RefreshQuestList()
        {
            questListBox.Items.Clear();

            var quests = gameManager.GetAllQuests();

            foreach (var quest in quests)
            {
                questListBox.Items.Add(quest);
            }
        }

        // Оновити статистику
        private void UpdateStats()
        {
            levelLabel.Text = $"Рівень: {gameManager.Player.Level}";
            xpLabel.Text = $"XP: {gameManager.Player.Experience}";
            xpProgressBar.Value = gameManager.Player.GetProgressPercent();

            totalQuestsLabel.Text =
                $"Всього квестів: {gameManager.GetTotalQuestsCount()}";

            activeQuestsLabel.Text =
                $"Активних: {gameManager.GetActiveQuestsCount()}";

            completedQuestsLabel.Text =
                $"Виконано: {gameManager.GetCompletedQuestsCount()}";
        }

        // Обробник кнопки "Видалити квест"
        private void button1_Click(object sender, EventArgs e)
        {
            if (questListBox.SelectedItem == null)
            {
                MessageBox.Show("Оберіть квест для видалення", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Quest selectedQuest = (Quest)questListBox.SelectedItem;

            var result = MessageBox.Show(
                $"Ви впевнені, що хочете видалити квест '{selectedQuest.Name}'?",
                "Підтвердження",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                gameManager.RemoveQuest(selectedQuest);

                RefreshQuestList();
                UpdateStats();

                MessageBox.Show("Квест видалено", "Успіх",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void filterCategoryComboBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (filterCategoryComboBox.SelectedIndex == 0)
            {
                RefreshQuestList();
                return;
            }

            QuestCategory selectedCategory =
                (QuestCategory)filterCategoryComboBox.SelectedItem;

            var filteredQuests =
                gameManager.GetQuestsByCategory(selectedCategory);

            questListBox.Items.Clear();

            foreach (var quest in filteredQuests)
            {
                questListBox.Items.Add(quest);
            }
        }
        // Вибір квесту у списку
        private void questListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (questListBox.SelectedItem == null)
                return;

            Quest selectedQuest = (Quest)questListBox.SelectedItem;

            questTextBox.Text = selectedQuest.Name;
            descriptionTextBox.Text = selectedQuest.Description;
            categoryComboBox.SelectedItem = selectedQuest.Category;
            difficultyComboBox.SelectedItem = selectedQuest.Difficulty;
        }

        private void editQuestButton_Click(object sender, EventArgs e)
        {
            if (questListBox.SelectedItem == null)
            {
                MessageBox.Show("Оберіть квест для редагування",
                    "Помилка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(questTextBox.Text))
            {
                MessageBox.Show("Назва не може бути порожньою",
                    "Помилка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            Quest selectedQuest = (Quest)questListBox.SelectedItem;

            try
            {
                gameManager.UpdateQuest(
                    selectedQuest,
                    questTextBox.Text.Trim(),
                    descriptionTextBox.Text ?? "Без опису",
                    (QuestCategory)categoryComboBox.SelectedItem,
                    (QuestDifficulty)difficultyComboBox.SelectedItem
                );

                RefreshQuestList();
                UpdateStats();

                MessageBox.Show("Квест успішно оновлено!",
                    "Успіх",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Помилка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void resetFilterButton_Click_1(object sender, EventArgs e)
        {
            filterCategoryComboBox.SelectedIndex = 0;
            RefreshQuestList();
        }
    }
}

