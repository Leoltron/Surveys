using System;
using System.Drawing;
using System.Windows.Forms;

namespace Surveys
{
    class PersonalDataPanel : Panel
    {
        private Label[] Names;
        private TextBox[] Values;

        private const int ElementHeight = 20;
        private const int ElementMargin = 20;

        public PersonalDataPanel(string[] dataPoints)
        {
            Names = new Label[dataPoints.Length];
            Values = new TextBox[dataPoints.Length];
            for (var i = 0; i < Names.Length; i++)
            {
                Names[i] = new Label
                {
                    Text = dataPoints[i],
                    Location = new Point(ElementMargin, (i + 1) * ElementMargin + i * ElementHeight),
                };
                Names[i].Size =
                    new Size(
                        TextRenderer.MeasureText(
                            dataPoints[i],
                            Names[i].Font,
                            new Size(int.MaxValue, int.MaxValue)
                        ).Width,
                        ElementHeight);

                Controls.Add(Names[i]);

                Values[i] = new TextBox {Location = new Point(Names[i].Right + ElementMargin, Names[i].Top - 2)};
                Controls.Add(Values[i]);
            }

            Resize += (sender, args) => OnResize();
            OnResize();
        }

        private void OnResize()
        {
            for (var i = 0; i < Names.Length; i++)
                Values[i].Size = new Size(Width - Names[i].Right - 100, ElementHeight);
        }

        public Tuple<string, string>[] GetPersonalData()
        {
            var result = new Tuple<string, string>[Names.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = Tuple.Create(Names[i].Text, Values[i].Text);
            return result;
        }
    }
}