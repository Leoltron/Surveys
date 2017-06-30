using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Surveys.Editor
{
    public sealed class SurveyEditorForm : Form
    {
        private readonly SurveyBuilder builder;

        private readonly TabControl tabControl;

        private readonly PersonalDataTabPage personalDataTabPage;
        private readonly CategoriesTabPage categoriesTabPage;
        private readonly QuestionsTabPage questionsTabPage;
        private readonly TabPage fileTabPage;
        private const int SaveButtonMargin = 5;
        private readonly Button saveButton;


        public SurveyEditorForm() : this(new SurveyBuilder())
        {
        }

        public SurveyEditorForm(Survey survey) : this(new SurveyBuilder(survey))
        {
        }

        private SurveyEditorForm(SurveyBuilder builder)
        {
            this.builder = builder;

            Icon = new Icon("icon.ico");
            Text = @"Редактор опросов";
            MinimumSize = new Size(700, 400);

            tabControl = new TabControl {Location = new Point(0, 2)};

            personalDataTabPage = new PersonalDataTabPage(tabControl, builder);
            tabControl.TabPages.Add(personalDataTabPage);

            categoriesTabPage = new CategoriesTabPage(tabControl, builder);
            tabControl.TabPages.Add(categoriesTabPage);

            questionsTabPage = new QuestionsTabPage(tabControl, builder);
            tabControl.TabPages.Add(questionsTabPage);

            categoriesTabPage.ExistingCategoryChanged += () => questionsTabPage.RefreshQuestionListBox();

            fileTabPage = new TabPage {Text = @"Файл"};
            saveButton = new Button {Location = new Point(SaveButtonMargin, SaveButtonMargin), Text = @"Сохранить"};
            saveButton.Click += (s, a) => SaveSurvey();
            fileTabPage.Controls.Add(saveButton);
            tabControl.TabPages.Add(fileTabPage);

            SizeChanged += UpdateSizes;
            Load += UpdateSizes;

            Controls.Add(tabControl);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            var dialogResult = MessageBox.Show(
                @"Вы хотите сохранить опрос?",
                @"Редактор опросов",
                MessageBoxButtons.YesNoCancel);
            switch (dialogResult)
            {
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
                case DialogResult.No:
                    break;
                case DialogResult.Yes:
                    e.Cancel = SaveSurvey() != DialogResult.OK;
                    break;
            }
        }

        private void UpdateSizes(object sender = null, EventArgs eventArgs = null)
        {
            tabControl.Size = ClientSize;


            saveButton.Size = new Size(
                tabControl.Size.Width - 19,
                tabControl.Size.Height - 37);

            personalDataTabPage.UpdateSizes();
            categoriesTabPage.UpdateSizes();
            questionsTabPage.UpdateSizes();
        }

        private DialogResult SaveSurvey()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = @"Сохранить опрос",
                InitialDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                Filter = @"Файлы JSON (*.json)|*.json|Все файлы (*.*)|*.*",
                FileName = "survey.json",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            var result = saveFileDialog.ShowDialog();

            if (result == DialogResult.OK)
                using (var file = File.CreateText(saveFileDialog.FileName))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, builder.BuildSurvey());
                }

            return result;
        }
    }
}