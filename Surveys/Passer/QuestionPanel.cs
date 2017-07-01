using System;
using System.Drawing;
using System.Windows.Forms;

namespace Surveys.Passer
{
    sealed class QuestionPanel : Panel
    {
        private readonly Label questionTextLabel;

        private readonly Label[] answerTextLabels;
        private readonly RadioButton[] answerRadioButtons;

        private const int ElementHeight = 20;
        private const int ElementMargin = 10;

        public QuestionPanel(IAnswerable iAnswerable, Survey survey, int questionNumber)
        {
            HorizontalScroll.Maximum = 0;
            AutoScroll = false;
            HorizontalScroll.Visible = false;
            AutoScroll = true;

            var question = survey.Questions[questionNumber];

            questionTextLabel = new Label {Text = question.Text, Location = new Point(ElementMargin, ElementMargin)};
            questionTextLabel.Font = new Font(questionTextLabel.Font.FontFamily, 11);
            Controls.Add(questionTextLabel);

            answerTextLabels = new Label[question.Answers.Length];
            answerRadioButtons = new RadioButton[question.Answers.Length];

            for (var i = 0; i < question.Answers.Length; i++)
            {
                answerRadioButtons[i] = new RadioButton
                {
                    Location = new Point(ElementMargin, (i + 1) * ElementMargin + i * ElementHeight)
                };
                var j = i;
                answerRadioButtons[i].CheckedChanged += (sender, args) =>
                {
                    if (answerRadioButtons[j].Checked)
                        iAnswerable.Answer(questionNumber, j);
                };
                Controls.Add(answerRadioButtons[i]);

                answerTextLabels[i] = new Label {Text = question.Answers[i].Description};
                Controls.Add(answerTextLabels[i]);
            }

            SizeChanged += (sender, args) => OnSizeChanged();
        }

        private void OnSizeChanged()
        {
            using (var g = CreateGraphics())
            {
                questionTextLabel.SetOptimalSize(g, Width - 4 * ElementMargin);

                for (var i = 0; i < answerTextLabels.Length; i++)
                {
                    Control prevControl = i == 0 ? questionTextLabel : answerTextLabels[i - 1];

                    var y = prevControl.Bottom + (i == 0 ? ElementMargin * 2 : ElementMargin);

                    answerRadioButtons[i].Location = new Point(ElementMargin, y);
                    answerRadioButtons[i].Size = new Size(26, answerRadioButtons[i].Height);

                    answerTextLabels[i].Location = new Point(answerRadioButtons[i].Right, y + 5);
                    answerTextLabels[i].SetOptimalSize(g, Width - answerTextLabels[i].Left - ElementMargin * 2 - 10);
                }
            }
        }
    }

    public static class LabelExtensions
    {
        public static void SetOptimalSize(this Label label, Graphics g, int width)
        {
            var size = g.MeasureString(label.Text, label.Font, width);
            label.Size = new Size(width, (int) Math.Ceiling(size.Height));
        }
    }
}