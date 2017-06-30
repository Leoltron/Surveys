using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Surveys.Editor
{
    sealed class QuestionsTabPage : TabPage
    {
        private readonly SurveyBuilder builder;
        private readonly TabControl tabControl;

        private Button addQuestionButton;
        private Button removeQuestionButton;
        private Button cloneQuestionButton;

        private GroupBox groupBox;

        private ListBox answersListBox;
        private Button addAnswerButton;
        private Button removeAnswerButton;

        private Label answerDescriptionLabel;
        private TextBox answerDescriptionTextBox;

        private Label pointsForAnswerLabel;
        private NumericUpDown pointsForAnswerInput;

        private Label questionTextLabel;
        private TextBox questionTextBox;

        private Label categoryLabel;
        private ComboBox categoryComboBox;

        private ListBox questionsListbox;

        private int newQuestionsAdded = 0;
        private int newAnswersAdded = 0;

        private const int ButtonHeight = 20;
        private const int ButtonMargin = 5;

        public QuestionsTabPage(TabControl tabControl, SurveyBuilder builder)
        {
            this.tabControl = tabControl;
            this.builder = builder;

            Text = @"Вопросы";

            InitControls();

            questionsListbox.DataSource = builder.Questions;
            questionsListbox.SelectedIndexChanged += (sender, args) => UpdateQuestionControls();

            addQuestionButton.Click += (sender, args) =>
            {
                builder.Questions.Add(
                    new QuestionBuilder(builder) {Text = "Вопрос " + (newQuestionsAdded + 1), Category = -1});
                newQuestionsAdded++;
            };

            removeQuestionButton.Click += (sender, args) =>
            {
                if (IsValidQuestionSelected)
                    builder.Questions.RemoveAt(questionsListbox.SelectedIndex);
            };

            cloneQuestionButton.Click += (sender, args) =>
            {
                if (IsValidQuestionSelected)
                    builder.Questions.Add(SelectedQuestion.Clone());
            };

            questionTextBox.TextChanged += OnQuestionTextBoxChanged;

            categoryComboBox.DataSource = builder.Categories;
            categoryComboBox.SelectedIndexChanged += OnCategoryComboBoxChanged;

            answersListBox.SelectedIndexChanged += (sender, args) => UpdateAnswerControls();

            addAnswerButton.Click += (sender, args) =>
            {
                SelectedQuestion.Answers.Add(new Answer
                {
                    Description = @"Ответ " + (newAnswersAdded + 1),
                    PointsForAnswer = 0
                });
                RefreshQuestionListBox();
                newAnswersAdded++;
            };

            removeAnswerButton.Click += (sender, args) =>
            {
                if (IsValidAnswerSelected)
                {
                    SelectedQuestion.Answers.RemoveAt(answersListBox.SelectedIndex);
                    RefreshQuestionListBox();
                }
            };

            answerDescriptionTextBox.TextChanged += OnAnswerTextBoxChanged;

            pointsForAnswerInput.ValueChanged += OnPointsForAnswerChanged;

            UpdateQuestionControls();
            UpdateAnswerControls();
        }

        private void InitControls()
        {
            addQuestionButton = new Button
            {
                Text = @"Добавить вопрос",
                Location = new Point(ButtonMargin - 2, ButtonMargin),
            };
            Controls.Add(addQuestionButton);

            removeQuestionButton = new Button {Text = @"Удалить вопрос"};
            Controls.Add(removeQuestionButton);

            cloneQuestionButton = new Button {Text = @"Скопировать вопрос"};
            Controls.Add(cloneQuestionButton);

            groupBox = new GroupBox {Text = @"Ответы"};
            Controls.Add(groupBox);

            pointsForAnswerLabel = new Label {Text = @"Очки в категории за ответ:"};
            groupBox.Controls.Add(pointsForAnswerLabel);

            pointsForAnswerInput = new NumericUpDown
            {
                Minimum = int.MinValue,
                Maximum = int.MaxValue,
                Value = 0
            };
            groupBox.Controls.Add(pointsForAnswerInput);

            answerDescriptionLabel = new Label {Text = @"Текст ответа:"};
            groupBox.Controls.Add(answerDescriptionLabel);

            answerDescriptionTextBox = new TextBox();
            groupBox.Controls.Add(answerDescriptionTextBox);

            answersListBox = new ListBox();
            groupBox.Controls.Add(answersListBox);

            addAnswerButton = new Button {Text = @"Добавить"};
            groupBox.Controls.Add(addAnswerButton);

            removeAnswerButton = new Button {Text = @"Удалить"};
            groupBox.Controls.Add(removeAnswerButton);

            questionsListbox = new ListBox {Location = new Point(addQuestionButton.Left, addQuestionButton.Bottom + 5)};
            Controls.Add(questionsListbox);

            questionTextLabel = new Label {Text = @"Текст вопроса:"};
            Controls.Add(questionTextLabel);
            questionTextBox = new TextBox();
            questionTextBox.Lines = new[] {"", "", ""};
            Controls.Add(questionTextBox);

            categoryLabel = new Label {Text = @"Категория вопроса:"};
            Controls.Add(categoryLabel);
            categoryComboBox = new ComboBox {DropDownStyle = ComboBoxStyle.DropDownList};
            Controls.Add(categoryComboBox);
        }

        private bool IsValidQuestionSelected => questionsListbox.SelectedIndex >= 0 &&
                                                questionsListbox.SelectedIndex < builder.Questions.Count;

        private bool IsValidAnswerSelected => IsValidQuestionSelected &&
                                              answersListBox.SelectedIndex >= 0 &&
                                              answersListBox.SelectedIndex < SelectedQuestion.Answers.Count;

        private QuestionBuilder SelectedQuestion => builder.Questions[questionsListbox.SelectedIndex];
        private Answer SelectedAnswer => SelectedQuestion.Answers[answersListBox.SelectedIndex];

        private bool blockQuestionUpdate = false;

        private void UpdateQuestionControls()
        {
            if (blockQuestionUpdate) return;

            if (IsValidQuestionSelected)
            {
                removeQuestionButton.Enabled = true;
                cloneQuestionButton.Enabled = true;

                questionTextBox.Enabled = true;
                questionTextBox.Text = SelectedQuestion.Text;

                categoryComboBox.Enabled = true;
                categoryComboBox.SelectedIndex = SelectedQuestion.Category;

                addAnswerButton.Enabled = true;
                answersListBox.DataSource = SelectedQuestion.Answers;
            }
            else
            {
                removeQuestionButton.Enabled = false;
                cloneQuestionButton.Enabled = false;

                questionTextBox.Enabled = false;
                questionTextBox.Text = @"Выберите вопрос";

                categoryComboBox.Enabled = false;
                categoryComboBox.SelectedIndex = -1;

                addAnswerButton.Enabled = false;
                answersListBox.DataSource = null;
            }
        }

        private void OnQuestionTextBoxChanged(object sender, EventArgs args)
        {
            if (!IsValidQuestionSelected || SelectedQuestion.Text == questionTextBox.Text) return;
            blockQuestionUpdate = true;
            SelectedQuestion.Text = questionTextBox.Text;
            RefreshQuestionListBox();
            blockQuestionUpdate = false;
        }

        public void RefreshQuestionListBox()
        {
            typeof(ListBox).GetMethod("RefreshItems", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(questionsListbox, new object[] { });
        }

        private void OnCategoryComboBoxChanged(object sender, EventArgs args)
        {
            if (!IsValidQuestionSelected || SelectedQuestion.Category == categoryComboBox.SelectedIndex) return;
            blockQuestionUpdate = true;
            SelectedQuestion.Category = categoryComboBox.SelectedIndex;
            RefreshQuestionListBox();
            blockQuestionUpdate = false;
        }

        private bool blockAnswerUpdate = false;

        private void UpdateAnswerControls()
        {
            if (blockAnswerUpdate) return;

            removeAnswerButton.Enabled = IsValidAnswerSelected;

            answerDescriptionTextBox.Enabled = IsValidAnswerSelected;
            answerDescriptionTextBox.Text = IsValidAnswerSelected ? SelectedAnswer.Description : @"Выберите ответ...";

            pointsForAnswerInput.Enabled = IsValidAnswerSelected;
            pointsForAnswerInput.Value = IsValidAnswerSelected ? SelectedAnswer.PointsForAnswer : 0;
        }

        private void OnAnswerTextBoxChanged(object sender, EventArgs args)
        {
            if (!IsValidAnswerSelected || SelectedAnswer.Description == answerDescriptionTextBox.Text) return;
            blockAnswerUpdate = true;
            SelectedAnswer.Description = answerDescriptionTextBox.Text;
            RefreshAnswerListBox();
            RefreshQuestionListBox();
            blockAnswerUpdate = false;
        }

        private void RefreshAnswerListBox()
        {
            typeof(ListBox).GetMethod("RefreshItems", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(answersListBox, new object[] { });
        }

        private void OnPointsForAnswerChanged(object sender, EventArgs args)
        {
            if (!IsValidAnswerSelected || SelectedAnswer.PointsForAnswer == pointsForAnswerInput.Value) return;
            blockAnswerUpdate = true;
            SelectedAnswer.PointsForAnswer = (int) pointsForAnswerInput.Value;
            RefreshAnswerListBox();
            RefreshQuestionListBox();
            blockAnswerUpdate = false;
        }

        public void UpdateSizes()
        {
            addQuestionButton.Size = removeQuestionButton.Size = cloneQuestionButton.Size =
                new Size(tabControl.Width / 3 - (int) (ButtonMargin * 2.5), ButtonHeight);
            removeQuestionButton.Location = new Point(addQuestionButton.Right + ButtonMargin * 2, ButtonMargin);
            cloneQuestionButton.Location = new Point(removeQuestionButton.Right + ButtonMargin * 2, ButtonMargin);

            groupBox.Location = new Point(tabControl.Left, tabControl.Bottom - 175);
            groupBox.Size = new Size(tabControl.Size.Width - 8, 175 - 30);

            pointsForAnswerLabel.Location = new Point(5, groupBox.Height - 22);
            pointsForAnswerLabel.Size = new Size(150, 20);

            pointsForAnswerInput.Location = new Point(pointsForAnswerLabel.Right, groupBox.Height - 25);
            pointsForAnswerInput.Size = new Size(groupBox.Width - 164, 20);

            answerDescriptionLabel.Location = new Point(5, pointsForAnswerInput.Top - 20 - 5);
            answerDescriptionLabel.Size = new Size(80, 20);

            answerDescriptionTextBox.Location = new Point(answerDescriptionLabel.Right,
                pointsForAnswerInput.Top - 23 - 5);
            answerDescriptionTextBox.Size = new Size(groupBox.Width - answerDescriptionLabel.Size.Width - 15, 20);

            removeAnswerButton.Location = new Point(5, answerDescriptionLabel.Top - 40);
            addAnswerButton.Location = new Point(5, answerDescriptionLabel.Top - 72);
            addAnswerButton.Size = removeAnswerButton.Size = new Size(75, 23);

            answersListBox.Location = new Point(addAnswerButton.Right + 5, addAnswerButton.Top);
            answersListBox.Size = new Size(groupBox.Width - 95, 68);

            categoryLabel.Location = new Point(5, groupBox.Top - 25);
            categoryLabel.Size = new Size(120, 20);

            categoryComboBox.Location = new Point(categoryLabel.Right, groupBox.Top - 27);
            categoryComboBox.Size = new Size(tabControl.Width - categoryLabel.Width - 17, 20);

            questionTextLabel.Location = new Point(5, categoryLabel.Top - 25);
            questionTextLabel.Size = new Size(85, 20);

            questionTextBox.Location = new Point(questionTextLabel.Right + 5, questionTextLabel.Top - 2);
            questionTextBox.Size = new Size(tabControl.Width - questionTextLabel.Right - 17, 30);

            questionsListbox.Size = new Size(tabControl.Width - 15, questionTextBox.Top - 30);
        }
    }
}