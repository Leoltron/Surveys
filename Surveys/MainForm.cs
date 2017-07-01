using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using Surveys.Editor;
using Surveys.Passer;

namespace Surveys
{
    sealed class MainForm : Form
    {
        private readonly Button createSurveyButton;
        private readonly Button changeSurveyButton;
        private readonly Button passSurveyButton;

        public MainForm()
        {
            Text = @"Опросник";
            Icon = new Icon("icon.ico");

            MinimumSize = new Size(300, 300);
            CenterToScreen();

            Font = new Font("Tahoma", 20);
            createSurveyButton = new Button
            {
                Location = new Point(0, 0),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = {BorderSize = 0},
                Text = "Создать опрос"
            };
            createSurveyButton.Click += (s, a) =>
            {
                Hide();
                var f = new SurveyEditorForm();
                f.FormClosed += (sender, args) => Show();
                f.Show();
            };

            changeSurveyButton = new Button
            {
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = {BorderSize = 0},
                Text = "Изменить опрос"
            };
            changeSurveyButton.Click += (s, a) =>
            {
                var survey = OpenSurvey();
                if (survey == null) return;
                Hide();
                var f = new SurveyEditorForm(survey);
                f.FormClosed += (sender, args) => Show();
                f.Show();
            };

            passSurveyButton = new Button
            {
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = {BorderSize = 0},
                Text = "Пройти опрос"
            };
            passSurveyButton.Click += (s, a) =>
            {
                var survey = OpenSurvey();
                if (survey == null) return;
                Hide();
                var f = new SurveyForm(survey);
                f.FormClosed += (sender, args) => Show();
                f.Show();
            };

            OnSizeChanged();
            Controls.Add(createSurveyButton);
            Controls.Add(changeSurveyButton);
            Controls.Add(passSurveyButton);

            SizeChanged += OnSizeChanged;
        }

        private Survey OpenSurvey()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = @"Открыть опрос",
                InitialDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                Filter = @"Файлы JSON (*.json)|*.json|Все файлы (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            var result = openFileDialog.ShowDialog();
            try
            {
                if (result == DialogResult.OK)
                    using (var file = new StreamReader(openFileDialog.FileName))
                        return JsonConvert.DeserializeObject<Survey>(file.ReadToEnd());
            }
            catch (Exception e)
            {
                MessageBox.Show(@"Ошибка при открытии файла: " + e.Message, e.GetType().Name,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        private void OnSizeChanged(object sender = null, EventArgs args = null)
        {
            var newSize = new Size(ClientSize.Width / 2, ClientSize.Height / 2);
            createSurveyButton.Size = newSize;
            changeSurveyButton.Size = newSize;
            changeSurveyButton.Location = new Point(createSurveyButton.Right, 0);
            passSurveyButton.Location = new Point(0, createSurveyButton.Bottom);
            passSurveyButton.Size = new Size(ClientSize.Width, ClientSize.Height / 2);
        }
    }
}