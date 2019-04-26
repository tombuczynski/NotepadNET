using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Tomproj.WPFUtils
{
    public class PrintUtils
    {
        private static FlowDocument CreateFlowDocument(string[] lines, FontUtils.Font font, double pageWidth)
        {
            FlowDocument fd = new FlowDocument
            {
                FontFamily = font.Family,
                FontSize = font.SizePx,
                FontStyle = font.Style,
                FontWeight = font.Weight,
                Background = Brushes.White,
                Foreground = Brushes.Black,

                ColumnGap = 0.0,
                ColumnWidth = pageWidth,

                LineStackingStrategy = System.Windows.LineStackingStrategy.MaxHeight,
            };

            foreach (var line in lines)
            {
                Paragraph p = new Paragraph(new Run(line));
                p.Margin = p.Padding = new System.Windows.Thickness(0.0);
                fd.Blocks.Add(p);
            }

            return fd;
        }

        public static void PrintLines(string[] lines, FontUtils.Font font, string jobDescription)
        {
            PrintDialog dialog = new PrintDialog();

            if (dialog.ShowDialog() == true)
            {
                FlowDocument fd = CreateFlowDocument(lines, font, dialog.PrintableAreaWidth);

                dialog.PrintDocument(((IDocumentPaginatorSource)fd).DocumentPaginator, jobDescription);
            }
        }

        public static void PrintText(string text, FontUtils.Font font, string jobDescription)
        {
            string[] lines = text.Split('\r');

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim('\n');
            }

            PrintLines(lines, font, jobDescription);
        }

    }
}
