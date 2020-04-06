using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Tomproj.WPFUtils;

namespace Notepad.NET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private OpenFileDialog OpenFileDlg;
        private SaveFileDialog SaveFileDlg;
        private string TextFilePath = null;
        private bool fTextModified = false;
        private Encoding TextEncoding = Encoding.GetEncoding(1250);
        private const int WM_CLIPBOARDUPDATE = 0x031D;

        private bool TextModified
        {
            get => fTextModified;
            set {
                fTextModified = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            OpenFileDlg = new OpenFileDialog
            {
                DefaultExt = "txt",
                Filter =
                "Pliki tekstowe (*.txt)|*.txt" +
                "|Pliki XML (*.xml)|*.xml" +
                "|Pliki źródłowe C# (*.cs)|*.cs" +
                "|Wszystkie pliki (*.*)|*.*",
                FilterIndex = 1,
                Title = "Otwieranie pliku"
            };

            SaveFileDlg = new SaveFileDialog
            {
                DefaultExt = OpenFileDlg.DefaultExt,
                Filter = OpenFileDlg.Filter,
                FilterIndex = 1,
                Title = "Zapisywanie pliku"
            };

            UpdateStatusBar();
        }

        private void LoadFile()
        {
            Editor.Text = File.ReadAllText(TextFilePath, TextEncoding);
            UpdateStatusBar();
            TextModified = false;
        }

        private void SaveFile()
        {
            File.WriteAllText(TextFilePath, Editor.Text, TextEncoding);
            UpdateStatusBar();
            TextModified = false;
        }

        private void UpdateStatusBar()
        {
            string textField = "[Brak pliku]";

            if (!string.IsNullOrEmpty(TextFilePath))
            {
                textField = Path.GetFileName(TextFilePath);
            }

            if (StatusBarTextField.Text != textField)
            {
                StatusBarTextField.Text = textField;
            }
        }

        private bool? FileSaveAskDialog()
        {
            MessageBoxResult result =
            MessageBox.Show("Zapisać zmiany w pliku ?", this.Title + " - zmiany w tekście",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);

            switch (result)
            {
                case MessageBoxResult.Cancel:
                    break;
                case MessageBoxResult.Yes:
                    return true;
                case MessageBoxResult.No:
                    return false; ;
                default:
                    break;
            }

            return null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBarUpdate(true, true, true);
        }

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TextFilePath))
            {
                OpenFileDlg.InitialDirectory = Path.GetDirectoryName(TextFilePath);
                OpenFileDlg.FileName = Path.GetFileName(TextFilePath);

            }

            bool? result = OpenFileDlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                TextFilePath = OpenFileDlg.FileName;
                LoadFile();
            }

        }

        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TextFilePath))
            {
               SaveFile();
            }
            else
            {
                MenuItem_SaveAs_Click(sender, e);
            }

        }

        private void MenuItem_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TextFilePath))
            {
                SaveFileDlg.InitialDirectory = Path.GetDirectoryName(TextFilePath);
                SaveFileDlg.FileName = Path.GetFileName(TextFilePath);

            }

            bool? result = SaveFileDlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                TextFilePath = SaveFileDlg.FileName;
                SaveFile();
            }
        }

        private void MenuItem_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_New_Click(object sender, RoutedEventArgs e)
        {
            if (ChangesAccepted())
            {
                Editor.Clear();
                TextFilePath = null;
                TextModified = false;
                UpdateStatusBar();
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !ChangesAccepted();
        }

        private bool ChangesAccepted()
        {
            if (TextModified)
            {
                bool? dlgResult = FileSaveAskDialog();

                if (dlgResult == null)
                {
                    return false;
                }
                else if (dlgResult.Value)
                {
                    MenuItem_Save_Click(null, null);
                }
            }

            return true;
        }

        private void Editor_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextModified = true;

            ToolBarUpdate(false, true, false);
        }

        private void MenuItem_Edit_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            MenuItem_Undo.IsEnabled = Editor.CanUndo;
            MenuItem_Redo.IsEnabled = Editor.CanRedo;
            MenuItem_Copy.IsEnabled = Editor.SelectionLength > 0;
            MenuItem_Cut.IsEnabled = Editor.SelectionLength > 0;
            MenuItem_Del.IsEnabled = Editor.SelectionLength > 0;
            MenuItem_Paste.IsEnabled = Clipboard.ContainsText();
        }

        private void MenuItem_SelAll_Click(object sender, RoutedEventArgs e)
        {
            Editor.SelectAll();
        }

        private void MenuItem_DateTime_Click(object sender, RoutedEventArgs e)
        {
            Editor.SelectedText = System.DateTime.Now.ToString();
            Editor.CaretIndex = Editor.SelectionStart + Editor.SelectionLength;
        }

        private void MenuItem_Paste_Click(object sender, RoutedEventArgs e)
        {
            Editor.Paste();
        }

        private void MenuItem_Undo_Click(object sender, RoutedEventArgs e)
        {
            Editor.Undo();
        }

        private void MenuItem_Redo_Click(object sender, RoutedEventArgs e)
        {
            Editor.Redo();
        }

        private void MenuItem_Cut_Click(object sender, RoutedEventArgs e)
        {
            Editor.Cut();
        }

        private void MenuItem_Copy_Click(object sender, RoutedEventArgs e)
        {
            Editor.Copy();
        }

        private void MenuItem_Del_Click(object sender, RoutedEventArgs e)
        {
            Editor.SelectedText = "";
        }

        private void MenuItem_Wrapping_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = ((MenuItem)sender).IsChecked;

            Editor.TextWrapping = isChecked ? TextWrapping.Wrap : TextWrapping.NoWrap;
        }

        private void MenuItem_Toolbar_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = ((MenuItem)sender).IsChecked;

            MainToolBar.Visibility = isChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        private void MenuItem_Statusbar_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = ((MenuItem)sender).IsChecked;

            MainStatusBar.Visibility = isChecked ? Visibility.Visible : Visibility.Collapsed;
            this.ResizeMode = isChecked ? ResizeMode.CanResizeWithGrip : ResizeMode.CanResize;
        }

        private void MenuItem_BackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            Color color = Colors.White;
            if (Editor.Background is SolidColorBrush)
            {
                color = ((SolidColorBrush)Editor.Background).Color;
            }


            if (ColorUtils.ChooseColorDlg(ref color))
            {
                Editor.Background = new SolidColorBrush(color);
            }
        }

        private void MenuItem_Font_Click(object sender, RoutedEventArgs e)
        {
            FontUtils.Font font = FontUtils.Font.ExtractFrom(Editor);

            if (FontUtils.ChooseFontDlg(ref font))
            {
                font.ApplyTo(Editor);
            }
        }

        private void MenuItem_Print_Click(object sender, RoutedEventArgs e)
        {
            PrintUtils.PrintText(Editor.Text, FontUtils.Font.ExtractFrom(Editor), Path.GetFileName(TextFilePath));
        }

        private void Editor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ToolBarUpdate(true, false, false);
        }

        private void Window_SourceInitialized(object sender, System.EventArgs e)
        {
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);

            Win32.AddClipboardFormatListener(source.Handle);

            var value = Win32.GetWindowLong(source.Handle, Win32.GWL_STYLE);
            Win32.SetWindowLong(source.Handle, Win32.GWL_STYLE, (int)(value & ~Win32.WS_MAXIMIZEBOX));
        }

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Handle messages...

            if (msg == WM_CLIPBOARDUPDATE)
            {
                ClipboardUpdate();
            }

            return IntPtr.Zero;
        }

        private void ClipboardUpdate()
        {
            ToolBarUpdate(false, false, true);
        }

        private void ToolBarUpdate(bool selChange, bool textChange, bool clipboardChange)
        {
            if (textChange)
            {
                if (ToolBarButton_Undo.IsEnabled != Editor.CanUndo)
                {
                    ToolBarButton_Undo.IsEnabled = Editor.CanUndo;
                    RibbonButton_Undo.IsEnabled = ToolBarButton_Undo.IsEnabled;
                }
                if (ToolBarButton_Redo.IsEnabled != Editor.CanRedo)
                {
                    ToolBarButton_Redo.IsEnabled = Editor.CanRedo;
                }
            }

            if (selChange)
            {
                bool s = Editor.SelectionLength > 0;

                ToolBarButton_Copy.IsEnabled = s;
                ToolBarButton_Cut.IsEnabled = s;
                ToolBarButton_Del.IsEnabled = s;
            }

            if (clipboardChange)
            {
                ToolBarButton_Paste.IsEnabled = Clipboard.ContainsText();
            }
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.N:
                        MenuItem_New_Click(sender, null);
                        break;

                    case Key.O:
                        MenuItem_Open_Click(sender, null);
                        break;

                    case Key.S:
                        MenuItem_Save_Click(sender, null);
                        break;

                    case Key.P:
                        MenuItem_Print_Click(sender, null);
                        break;

                    default:
                        e.Handled = false;
                        return;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.F5:
                        MenuItem_DateTime_Click(sender, null);
                        break;

                    default:
                        e.Handled = false;
                        return;
                }
            }

            e.Handled = true;
        }
    }
}
