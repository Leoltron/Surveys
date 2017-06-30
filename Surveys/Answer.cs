namespace Surveys
{
    public class Answer
    {
        public string Description;
        public int PointsForAnswer;

        public override string ToString()
        {
            return $"{Description}({PointsForAnswer})";
        }

        public Answer Clone()
        {
            return new Answer{Description = Description, PointsForAnswer = PointsForAnswer};
        }
    }
}