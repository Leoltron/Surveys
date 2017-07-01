using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Surveys.Passer
{
    sealed class SurveyForm : Form, IAnswerable
    {
        private readonly Survey survey;
        private readonly int[] answers;

        private bool SurveyFinishedFlag = false;

        private readonly Button nextButton;
        private readonly Button prevButton;
        private readonly Label title;

        private readonly Panel[] panels;

        private readonly PersonalDataPanel personalDataPanel;

        private int currentPage;
        private int PagesCount => survey.QuestionsAmount + (HavePersonalDataPoints ? 1 : 0);

        private bool HavePersonalDataPoints => survey.PersonalDataPoints.Length > 0;

        public void Answer(int questionId, int answerId) => answers[questionId] = answerId;
        private bool AnsweredAllQuestions => answers.All(answer => answer >= 0);

        private bool IsLastPage => currentPage == PagesCount - 1;
        private bool IsPersonalDataPage => currentPage == 0 && HavePersonalDataPoints;
        private int GetCurrentQuestionNumber => HavePersonalDataPoints ? currentPage - 1 : currentPage;

        public SurveyForm(Survey survey)
        {
            Text = @"Опрос";
            Icon = new Icon("icon.ico");
            MinimumSize = new Size(500, 400);
            CenterToScreen();

            this.survey = survey;
            answers = new int[survey.QuestionsAmount];
            for (var j = 0; j < survey.QuestionsAmount; j++)
                answers[j] = -1;


            panels = new Panel[PagesCount];

            if (HavePersonalDataPoints)
                panels[0] = personalDataPanel = new PersonalDataPanel(survey.PersonalDataPoints) {Visible = true};

            var i = HavePersonalDataPoints ? 1 : 0;
            for (var j = 0; j < survey.QuestionsAmount; j++)
            {
                panels[i] = new QuestionPanel(this, survey, j) {Visible = false};
                i++;
            }

            foreach (var panel in panels)
            {
                panel.BackColor = Color.White;
                panel.BorderStyle = BorderStyle.Fixed3D;
                panel.Location = new Point(-3, 50);
                Controls.Add(panel);
            }

            title = new Label
            {
                Location = new Point(10, 10),
                Font = new Font(Font.FontFamily.Name, 18f),
                Size = new Size(500, 40)
            };
            Controls.Add(title);

            nextButton = new Button();
            nextButton.Click += (sender, args) =>
            {
                if (IsLastPage)
                {
                    if (!AnsweredAllQuestions)
                        MessageBox.Show(
                            @"Не на все вопросы дан ответ. Пожалуйста, вернитесь и закончите опрос.",
                            @"Опрос",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    else if (
                        MessageBox.Show(
                            @"Вы уверены, что хотите завершить опрос?",
                            @"Опрос",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        ) == DialogResult.Yes)
                    {
                        SaveSurveyResults();
                        SurveyFinishedFlag = true;
                        Close();
                        MessageBox.Show(
                            @"Ваш результат сохранен. Спасибо за участие в опросе!",
                            @"Опрос",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
                else MovePage(1);
            };
            Controls.Add(nextButton);

            prevButton = new Button {Text = @"Назад"};
            prevButton.Click += (sender, args) =>
            {
                if (currentPage > 0)
                    MovePage(-1);
            };
            Controls.Add(prevButton);

            UpdateControls();
            Resize += (sender, args) => OnResize();
            Load += (sender, args) => OnResize();
        }

        private void MovePage(int pagesAmount)
        {
            var newNumber = currentPage + pagesAmount;
            if (0 <= newNumber && newNumber < PagesCount)
            {
                panels[currentPage].Visible = false;
                panels[newNumber].Visible = true;
                currentPage = newNumber;
                UpdateControls();
            }
        }

        private void SaveSurveyResults()
        {
            var output = new List<string>();
            if (HavePersonalDataPoints)
                output.AddRange(personalDataPanel.GetPersonalData().Select(pd => $"{pd.Item1}: {pd.Item2}"));
            output.Add("");
            output.Add("Дата прохождения опросв: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
            if (survey.Categories.Length > 0 && survey.Questions.Any(q => q.Category > 0))
            {
                output.Add("");
                output.Add(@"Сумма по категориям:");
                output.AddRange(GetCategoriesSummary().Select(tuple => $"{tuple.Item1}: {tuple.Item2}"));
            }
            output.Add("");
            output.Add(@"Ответы:");
            for (var i = 0; i < survey.QuestionsAmount; i++)
                output.Add($"{i+1}) {survey.Questions[i].Text}: {survey.Questions[i].Answers[answers[i]]}");
            File.WriteAllLines(GetFirstAvailablePath(),output);
        }

        private Tuple<string, int>[] GetCategoriesSummary()
        {
            var result = new Tuple<string, int>[survey.Categories.Length];
            for (var i = 0; i < survey.Categories.Length; i++)
                result[i] = Tuple.Create(
                    survey.Categories[i],
                    survey.Questions.Sum(q => q.Category == i ? q.Answers[answers[i]].PointsForAnswer : 0)
                );
            return result;
        }

        private static string GetFirstAvailablePath()
        {
            var dirName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (File.Exists($"{dirName}\\SurveyResults.txt"))
                return $"{dirName}\\SurveyResults.txt";
            var i = 0;
            while (File.Exists($"{dirName}\\SurveyResults({i}).txt"))
                i++;
            return $"{dirName}\\SurveyResults({i}).txt";
        }

        private void UpdateControls()
        {
            title.Text = IsPersonalDataPage
                ? @"Пожалуйста, укажите свои данные"
                : $"Вопрос {GetCurrentQuestionNumber + 1} из {survey.Questions.Length}";
            nextButton.Text = IsLastPage ? @"Готово" : @"Далее";
            prevButton.Enabled = currentPage > 0;
        }

        private const int ButtonMargin = 5;
        private const int ButtonWidth = 100;
        private const int ButtonHeight = 30;

        private void OnResize()
        {
            prevButton.Location = new Point(ButtonMargin, Height - ButtonMargin - ButtonHeight - 40);
            nextButton.Location = new Point(Width - ButtonMargin - ButtonWidth - 16,
                Height - ButtonMargin - ButtonHeight - 40);

            nextButton.Size = prevButton.Size = new Size(ButtonWidth, ButtonHeight);

            var panelSize = new Size(Width - 11, Height - nextButton.Height - 100);
            foreach (var panel in panels)
                panel.Size = panelSize;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (SurveyFinishedFlag) return;
            var dialogResult = MessageBox.Show(
                @"Ответы не будут сохранены. Вы уверены, что хотите выйти?",
                @"Опрос",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.No)
                e.Cancel = true;
        }
    }
}