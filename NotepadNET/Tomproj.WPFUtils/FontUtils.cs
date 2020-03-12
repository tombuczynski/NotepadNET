using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tomproj.WPFUtils
{
    public static class FontUtils
    {
        public struct Font
        {
            private double _fontSizePx;

            public Color ForegroundColor { get; set; }
            public SolidColorBrush ForegroundBrush => new SolidColorBrush(ForegroundColor);
            public FontFamily Family { get; set; }
            public double SizePx { get => _fontSizePx; set => _fontSizePx = value; }
            public double SizePt { get => (int)(_fontSizePx * 72.0 / 96.0); set => _fontSizePx = value * 96.0 / 72.0; }
            public FontWeight Weight { get; set; }
            public FontStyle Style { get; set; }
            public TextDecorationCollection Decorations { get; set; }
            public bool Underline {
                get
                {
                    if (Decorations == null)
                        return false;
                    return Decorations.Contains(TextDecorations.Underline[0]);
                }
                set
                {
                    TextDecoration underline = TextDecorations.Underline[0];

                    if (Decorations == null)
                    {
                        if (value)
                            Decorations = new TextDecorationCollection();
                        else
                            return;
                    }

                    if (Decorations.Contains(underline))
                    {
                        if (!value)
                            Decorations.Remove(underline);
                    }
                    else
                    {
                        if (value)
                            Decorations.Add(underline);
                    }
                }
            }
            public bool Strikeout
            {
                get
                {
                    if (Decorations == null)
                        return false;
                    return Decorations.Contains(TextDecorations.Strikethrough[0]);
                }
                set
                {
                    TextDecoration strikeout = TextDecorations.Strikethrough[0];

                    if (Decorations == null)
                    {
                        if (value)
                            Decorations = new TextDecorationCollection();
                        else
                            return;
                    }

                    if (Decorations.Contains(strikeout))
                    {
                        if (!value)
                            Decorations.Remove(strikeout);
                    }
                    else
                    {
                        if (value)
                            Decorations.Add(strikeout);
                    }
                }
            }
            public bool Italic
            {
                get => Style == FontStyles.Italic;
                set => Style = value ? FontStyles.Italic : FontStyles.Normal;
            }
            public bool Bold
            {
                get => Weight >= FontWeights.Bold;
                set => Weight = value ? FontWeights.Bold : FontWeights.Normal;
            }

            public static Font Default => new Font()
            {
                Family = new FontFamily("Segoe UI"),
                SizePt = 10,
                Weight = FontWeights.Normal,
                Style = FontStyles.Normal,
                ForegroundColor = SystemColors.WindowTextColor,
                Decorations = null,
            };

            public static Font ExtractFrom(Control c) => new Font()
            {
                Family = c.FontFamily,
                SizePx = c.FontSize,
                Weight = c.FontWeight,
                Style = c.FontStyle,
                ForegroundColor = (c.Foreground as SolidColorBrush)?.Color ?? SystemColors.WindowTextColor,
                Decorations = (c as TextBox)?.TextDecorations,
            };

            public void ApplyTo(Control c)
            {
                c.FontFamily = Family;
                c.FontSize = SizePx;
                c.FontWeight = Weight;
                c.FontStyle = Style;
                c.Foreground = ForegroundBrush;

                if (c is TextBox)
                {
                    ((TextBox)c).TextDecorations = Decorations;
                }
            }

            public static explicit operator System.Drawing.Font(Font font)
            {
                System.Drawing.FontStyle wfFontStyle = System.Drawing.FontStyle.Regular;

                if (font.Italic)
                    wfFontStyle |= System.Drawing.FontStyle.Italic;

                if (font.Bold)
                    wfFontStyle |= System.Drawing.FontStyle.Bold;

                if (font.Underline)
                    wfFontStyle |= System.Drawing.FontStyle.Underline;

                if (font.Strikeout)
                    wfFontStyle |= System.Drawing.FontStyle.Strikeout;

                return new System.Drawing.Font(font.Family.Source, (float)font.SizePt, wfFontStyle); ;
            }

            public static explicit operator Font(System.Drawing.Font wfFont) => new Font()
            {
                Family = new FontFamily(wfFont.FontFamily.Name),
                SizePt = wfFont.SizeInPoints,
                Bold = wfFont.Bold,
                Italic = wfFont.Italic,
                Underline = wfFont.Underline,
                Strikeout = wfFont.Strikeout,

                ForegroundColor = SystemColors.WindowTextColor,
            };
        }

        public static bool ChooseFontDlg(ref Font font)
        {
            using (System.Windows.Forms.FontDialog dialog = new System.Windows.Forms.FontDialog())
            {
                dialog.ShowColor = true;
                dialog.ShowEffects = true;
                dialog.Font = (System.Drawing.Font)font;
                dialog.Color = font.ForegroundColor.Convert();

                bool fontChoosed = dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK;
                if (fontChoosed)
                {
                    font = (Font)dialog.Font;
                    font.ForegroundColor = dialog.Color.Convert();
                }

                return fontChoosed;
            }
        }
    }
}
