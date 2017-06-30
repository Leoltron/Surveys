using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Surveys.Editor
{
    public sealed class CategoriesTabPage : TabPage
    {
        private readonly SurveyBuilder builder;
        private readonly TabControl tabControl;

        private readonly ListBox categoriesListBox;
        private readonly Button addCategoryButton;
        private readonly Button removeCategoryButton;
        private readonly TextBox categoryTextBox;

        private int newCategoriesAdded = 0;

        private Action existingCategoryChanged;

        public event Action ExistingCategoryChanged
        {
            add => existingCategoryChanged += value;
            remove => existingCategoryChanged -= value;
        }

        public CategoriesTabPage(TabControl tabControl, SurveyBuilder builder)
        {
            this.tabControl = tabControl;
            this.builder = builder;

            Text = @"Категории вопросов";
            categoriesListBox = new ListBox
            {
                SelectionMode = SelectionMode.One,
                DataSource = this.builder.Categories
            };
            categoriesListBox.SelectedIndexChanged += UpdateCurrentCategory;

            addCategoryButton = new Button {Text = @"Добавить"};
            addCategoryButton.Click += (sender, args) =>
            {
                this.builder.Categories.Add("Категория " + (newCategoriesAdded + 1));
                newCategoriesAdded++;
            };

            removeCategoryButton = new Button {Text = @"Удалить"};
            removeCategoryButton.Click += (sender, args) => RemoveCategoryAt(categoriesListBox.SelectedIndex);

            categoryTextBox = new TextBox();
            categoryTextBox.TextChanged += OnTextChanged;

            Controls.Add(categoriesListBox);
            Controls.Add(addCategoryButton);
            Controls.Add(removeCategoryButton);
            Controls.Add(categoryTextBox);

            UpdateCurrentCategory();
        }

        private bool blockCategoryUpdate = false;

        private void UpdateCurrentCategory(object sender = null, EventArgs e = null)
        {
            if (blockCategoryUpdate) return;
            var newIndex = categoriesListBox.SelectedIndex;
            var canEditCategory = newIndex >= 0 && newIndex < builder.Categories.Count;

            UpdateCategoryTextBox(canEditCategory ? builder.Categories[newIndex] : @"Выберите категорию");

            removeCategoryButton.Enabled =
                categoryTextBox.Enabled = canEditCategory;
        }

        private bool ValidCategorySelected => categoriesListBox.SelectedIndex >= 0 &&
                                              categoriesListBox.SelectedIndex < builder.Categories.Count;

        private string SelectedCategory => builder.Categories[categoriesListBox.SelectedIndex];

        private void OnTextChanged(object sender, EventArgs args)
        {
            if (ValidCategorySelected && categoryTextBox.Text != SelectedCategory)
            {
                blockCategoryUpdate = true;
                builder.Categories[categoriesListBox.SelectedIndex] = categoryTextBox.Text;
                existingCategoryChanged?.Invoke();
                blockCategoryUpdate = false;
            }
        }

        private void RemoveCategoryAt(int i)
        {
            if (i < 0 || i >= builder.Categories.Count) return;
            builder.Categories.RemoveAt(i);
            foreach (var question in builder.Questions.Where(q => q.Category > i))
                question.Category--;
            foreach (var question in builder.Questions.Where(q => q.Category == i))
                question.Category = -1;

            existingCategoryChanged?.Invoke();
            UpdateCurrentCategory();
        }

        private void UpdateCategoryTextBox(string text)
        {
            if (categoryTextBox.Text != text)
                categoryTextBox.Text = text;
        }

        private const int CategoryEditorControlsHeight = 20;
        private const int CategoryEditorControlsOffset = 5;

        public void UpdateSizes()
        {
            var top = tabControl.Bottom - CategoryEditorControlsHeight - 35;
            categoriesListBox.Size = new Size(tabControl.Width, top);

            addCategoryButton.Size = removeCategoryButton.Size = new Size(70, CategoryEditorControlsHeight);

            addCategoryButton.Location = new Point(0, top);
            removeCategoryButton.Location = new Point(addCategoryButton.Right + CategoryEditorControlsOffset, top);

            var textBoxLeft = removeCategoryButton.Right + CategoryEditorControlsOffset;
            categoryTextBox.Location = new Point(textBoxLeft, top);
            categoryTextBox.Size = new Size(tabControl.Width - textBoxLeft - 10, CategoryEditorControlsHeight);
        }
    }
}