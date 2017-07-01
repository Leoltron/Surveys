using System;
using System.Drawing;
using System.Windows.Forms;

namespace Surveys.Passer
{
    sealed class PersonalDataPanel : Panel
    {
        private readonly Label[] names;
        private readonly TextBox[] values;

        private const int ElementHeight = 20;
        private const int ElementMargin = 20;

        public PersonalDataPanel(string[] dataPoints)
        {
            AutoScroll = true;

            names = new Label[dataPoints.Length];
            values = new TextBox[dataPoints.Length];
            for (var i = 0; i < names.Length; i++)
            {
                names[i] = new Label
                {
                    Text = dataPoints[i],
                    Location = new Point(ElementMargin, (i + 1) * ElementMargin + i * ElementHeight),
                };
                names[i].Size =
                    new Size(
                        TextRenderer.MeasureText(
                            dataPoints[i],
                            names[i].Font,
                            new Size(int.MaxValue, int.MaxValue)
                        ).Width,
                        ElementHeight);

                Controls.Add(names[i]);

                values[i] = new TextBox {Location = new Point(names[i].Right + ElementMargin, names[i].Top - 2)};
                Controls.Add(values[i]);
            }

            Resize += (sender, args) => OnResize();
            OnResize();
        }

        private void OnResize()
        {
            for (var i = 0; i < names.Length; i++)
                values[i].Size = new Size(Width - names[i].Right - 100, ElementHeight);
        }

        public Tuple<string, string>[] GetPersonalData()
        {
            var result = new Tuple<string, string>[names.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = Tuple.Create(names[i].Text, values[i].Text);
            return result;
        }
    }
}