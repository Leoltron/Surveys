using System.Drawing;
using System.Windows.Forms;

namespace Surveys
{
    sealed class SurveyForm : Form
    {
        private Survey survey;

        private Button nextButton;
        private Button prevButton;

        public SurveyForm(Survey survey)
        {
            Text = @"Опрос";
            Icon = new Icon("icon.ico");
            this.survey = survey;

            nextButton = new Button();
            prevButton = new Button();
        }
        
    }
}
