using System;
using System.Drawing;
using System.Windows.Forms;

namespace Surveys
{
    sealed class SurveyForm : Form
    {
        private readonly Survey survey;

        private readonly Button nextButton;
        private readonly Button prevButton;
        private readonly Label title;
        private Panel[] panels;

        private int currentPage;
        private int PagesCount => survey.QuestionsAmount + (HavePersonalDataPoints ? 1 : 0);

        private bool HavePersonalDataPoints => survey.PersonalDataPoints.Length > 0;

        private bool IsLastPage => currentPage == PagesCount - 1;
        private bool IsPersonalDataPage => currentPage == 0 && HavePersonalDataPoints;
        private int GetCurrentQuestionNumber => HavePersonalDataPoints ? currentPage - 1 : currentPage;

        public SurveyForm(Survey survey)
        {
            Text = @"Опрос";
            Icon = new Icon("icon.ico");
            MinimumSize = new Size(500, 400);

            this.survey = survey;

            panels = new Panel[PagesCount];

            if (HavePersonalDataPoints)
                panels[0] = new PersonalDataPanel(survey.PersonalDataPoints) {Visible = true};

            var i = HavePersonalDataPoints ? 1 : 0;
            foreach (var question in survey.Questions)
            {
                panels[i] = new Panel() {Visible = false};
                i++;
            }

            foreach (var panel in panels)
            {
                panel.BorderStyle = BorderStyle.FixedSingle;
                panel.AutoScroll = true;
                panel.Location = new Point(0,50);
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
            Controls.Add(nextButton);

            prevButton = new Button {Text = @"Назад"};
            Controls.Add(prevButton);

            UpdateLabels();
            Resize += (sender, args) => OnResize();
            Load += (sender, args) => OnResize();
        }

        private void UpdateLabels()
        {
            title.Text = IsPersonalDataPage
                ? @"Пожалуйста, укажите свои данные"
                : $"Вопрос №{GetCurrentQuestionNumber + 1} из {survey.Questions.Length}";
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

            foreach (var panel in panels)
                panel.Size = new Size(Width-16, Height - nextButton.Height - 100);
        }
    }
}