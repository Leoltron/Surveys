using System.ComponentModel;
using System.Linq;
using Surveys.Editor;

namespace Surveys
{
    public class QuestionBuilder
    {
        private readonly SurveyBuilder builder;
        public string Text;
        public BindingList<Answer> Answers;
        public int Category;

        public QuestionBuilder(SurveyBuilder builder)
        {
            this.builder = builder;
            Text = "";
            Answers = new BindingList<Answer>();
            Category = -1;
        }

        public QuestionBuilder(SurveyBuilder builder, Question question)
        {
            this.builder = builder;
            Text = question.Text;
            Answers = new BindingList<Answer>(question.Answers);
            Category = question.Category;
        }

        private string GetCategory()
        {
            return Category >= 0 && Category < builder.Categories.Count ? builder.Categories[Category] : "";
        }

        public override string ToString()
        {
            return $"[{GetCategory()}] {Text} - {string.Join(", ", Answers)}";
        }

        public Question ToQuestion()
        {
            return new Question {Answers = Answers.Select(a => a.Clone()).ToArray(), Text = Text, Category = Category};
        }

        public QuestionBuilder Clone()
        {
            return new QuestionBuilder(builder,ToQuestion());
        }
    }
}