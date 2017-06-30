using System.ComponentModel;
using System.Linq;

namespace Surveys.Editor
{
    public class SurveyBuilder
    {
        public readonly BindingList<string> PersonalDataPoints;
        public readonly BindingList<string> Categories;
        public readonly BindingList<QuestionBuilder> Questions;

        public SurveyBuilder()
        {
            PersonalDataPoints = new BindingList<string>();
            Categories = new BindingList<string>();
            Questions = new BindingList<QuestionBuilder>();
        }

        public SurveyBuilder(Survey survey)
        {
            Categories = new BindingList<string>(survey.Categories.ToList());
            Questions = new BindingList<QuestionBuilder>();
            foreach (var surveyQuestion in survey.Questions)
                Questions.Add(new QuestionBuilder(this, surveyQuestion));
        }

        public Survey BuildSurvey()
        {
            return new Survey(Categories.ToArray(), Questions.Select(q => q.ToQuestion()).ToArray(),PersonalDataPoints.ToArray());
        }
    }
}