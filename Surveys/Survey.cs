namespace Surveys
{
    public class Survey
    {
        public readonly string[] PersonalDataPoints;
        public readonly string[] Categories;
        public readonly Question[] Questions;

        public int QuestionsAmount => Questions.Length;

        public Survey(string[] categories, Question[] questions, string[] personalDataPoints)
        {
            Categories = categories ?? new string[0];
            Questions = questions ?? new Question[0];
            PersonalDataPoints = personalDataPoints ?? new string[0];
        }
    }
}