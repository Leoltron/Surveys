using System;
using System.Drawing;
using System.Windows.Forms;

namespace Surveys.Editor
{
    sealed class PersonalDataTabPage : TabPage
    {
        private readonly SurveyBuilder builder;
        private readonly TabControl tabControl;

        private readonly ListBox personalDataPointsListBox;
        private readonly Button addDataPointButton;
        private readonly Button removeDataPointButton;
        private readonly TextBox dataPointTextBox;

        private int newDataPointsAdded = 0;

        public PersonalDataTabPage(TabControl tabControl, SurveyBuilder builder)
        {
            this.tabControl = tabControl;
            this.builder = builder;

            Text = @"Личные данные";
            personalDataPointsListBox = new ListBox
            {
                SelectionMode = SelectionMode.One,
                DataSource = this.builder.PersonalDataPoints
            };
            personalDataPointsListBox.SelectedIndexChanged += UpdateCurrentDataPoint;

            addDataPointButton = new Button { Text = @"Добавить" };
            addDataPointButton.Click += (sender, args) =>
            {
                this.builder.PersonalDataPoints.Add("Пункт " + (newDataPointsAdded + 1));
                newDataPointsAdded++;
            };

            removeDataPointButton = new Button { Text = @"Удалить" };
            removeDataPointButton.Click += (sender, args) =>
            {
                if (IsValidDataPointSelected)
                    builder.PersonalDataPoints.RemoveAt(personalDataPointsListBox.SelectedIndex);
            };

            dataPointTextBox = new TextBox();
            dataPointTextBox.TextChanged += OnTextChanged;

            Controls.Add(personalDataPointsListBox);
            Controls.Add(addDataPointButton);
            Controls.Add(removeDataPointButton);
            Controls.Add(dataPointTextBox);

            UpdateCurrentDataPoint();
        }

        private bool blockDataPointUpdate = false;

        private void UpdateCurrentDataPoint(object sender = null, EventArgs e = null)
        {
            if (blockDataPointUpdate) return;
            var newIndex = personalDataPointsListBox.SelectedIndex;
            var canEditDataPoint = newIndex >= 0 && newIndex < builder.PersonalDataPoints.Count;

            UpdateDataPointTextBox(canEditDataPoint ? builder.PersonalDataPoints[newIndex] : @"Выберите пункт");

            removeDataPointButton.Enabled =
                dataPointTextBox.Enabled = canEditDataPoint;
        }

        private bool IsValidDataPointSelected => personalDataPointsListBox.SelectedIndex >= 0 &&
                                              personalDataPointsListBox.SelectedIndex < builder.PersonalDataPoints.Count;

        private string SelectedDataPoint => builder.PersonalDataPoints[personalDataPointsListBox.SelectedIndex];

        private void OnTextChanged(object sender, EventArgs args)
        {
            if (IsValidDataPointSelected && dataPointTextBox.Text != SelectedDataPoint)
            {
                blockDataPointUpdate = true;
                builder.PersonalDataPoints[personalDataPointsListBox.SelectedIndex] = dataPointTextBox.Text;
                blockDataPointUpdate = false;
            }
        }

        private void UpdateDataPointTextBox(string text)
        {
            if (dataPointTextBox.Text != text)
                dataPointTextBox.Text = text;
        }

        private const int DataPointEditorControlsHeight = 20;
        private const int DataPointEditorControlsOffset = 5;

        public void UpdateSizes()
        {
            var top = tabControl.Bottom - DataPointEditorControlsHeight - 35;
            personalDataPointsListBox.Size = new Size(tabControl.Width, top);

            addDataPointButton.Size = removeDataPointButton.Size = new Size(70, DataPointEditorControlsHeight);

            addDataPointButton.Location = new Point(0, top);
            removeDataPointButton.Location = new Point(addDataPointButton.Right + DataPointEditorControlsOffset, top);

            var textBoxLeft = removeDataPointButton.Right + DataPointEditorControlsOffset;
            dataPointTextBox.Location = new Point(textBoxLeft, top);
            dataPointTextBox.Size = new Size(tabControl.Width - textBoxLeft - 10, DataPointEditorControlsHeight);
        }
    }
}
