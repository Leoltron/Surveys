using System;

namespace Surveys
{
    public class Survey
    {
        public readonly string[] PersonalDataPoints;
        public readonly string[] Categories;
        public readonly Question[] Questions;
        private readonly int[] answers;

        public int QuestionsAmount => Questions.Length;

        public Survey(string[] categories, Question[] questions, string[] personalDataPoints)
        {
            Categories = categories;
            Questions = questions;
            PersonalDataPoints = personalDataPoints;
            answers = new int[QuestionsAmount];
        }

        public void Answer(int questonNumber, int answerNumber)
        {
            if (questonNumber < 0 || questonNumber >= QuestionsAmount)
                throw new ArgumentOutOfRangeException(nameof(questonNumber), questonNumber,
                    @"Question number must be more than zero and less than amount of questions");
            answers[questonNumber] = answerNumber;
        }
    }
}