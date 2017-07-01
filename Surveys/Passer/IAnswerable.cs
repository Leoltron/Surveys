namespace Surveys.Passer
{
    public interface IAnswerable
    {
        void Answer(int questionId, int answerId);
    }
}