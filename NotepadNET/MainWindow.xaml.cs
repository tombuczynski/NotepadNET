using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Tomproj.WPFUtils;
using Tomproj.WPFUtils.FindReplace;

namespace Notepad.NET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly OpenFileDialog OpenFileDlg;
        private readonly SaveFileDialog SaveFileDlg;
        private string TextFilePath = null;
        private bool fTextModified = false;
        private readonly Encoding TextEncoding = Encoding.GetEncoding(1250);
        private FindDlg TextFindDlg = null;

        private const int WM_CLIPBOARDUPDATE = 0x031D;

        private bool TextModified
        {
            get => fTextModified;
            set {
                fTextModified = value;
                UpdateStatusBar();
            }
        }

        public MainWindow()
        {
           //System.Threading.Thread.CurrentThread.CurrentUICulture = 
           //     System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

            InitializeComponent();

            OpenFileDlg = new OpenFileDialog
            {
                DefaultExt = "txt",
                Filter = Properties.Resources.FileFilter,
                FilterIndex = 1,
                Title = Properties.Resources.OpenFileTitle
            };

            SaveFileDlg = new SaveFileDialog
            {
                DefaultExt = OpenFileDlg.DefaultExt,
                Filter = Properties.Resources.FileFilter,
                FilterIndex = 1,
                Title = Properties.Resources.SaveFileTitle
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
            string textField1 = Properties.Resources.NoFile;

            if (!string.IsNullOrEmpty(TextFilePath))
            {
                textField1 = Path.GetFileName(TextFilePath);
            }

            if (StatusBarFileName.Text != textField1)
            {
                StatusBarFileName.Text = textField1;
            }

            string textField2 = fTextModified ? Properties.Resources.TextModified : "  ";

            if (StatusBarModified.Text != textField2)
            {
                StatusBarModified.Text = textField2;
            }
        }

        private bool? FileSaveAskDialog()
        {
            MessageBoxResult result =
            MessageBox.Show(Properties.Resources.MessageFileChanged, this.Title + " - " + Properties.Resources.MessageTextChanged,
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
            if (ChangesAccepted())
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

            MenuItem_Find.IsEnabled = Editor.Text.Length > 0;
            MenuItem_FindNext.IsEnabled = MenuItem_Find.IsEnabled;
            MenuItem_FindPrev.IsEnabled = MenuItem_Find.IsEnabled;
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

        private void MenuItem_Find_Click(object sender, RoutedEventArgs e)
        {
            ShowFindDialog("", false, false);
        }

        private void ShowFindDialog(string text, bool caseSens, bool dirUp)
        {
            if (Editor.Text.Length > 0)
            {
                if (TextFindDlg != null)
                {
                    text = TextFindDlg.TextToFind;
                    caseSens = TextFindDlg.CaseSensitive;
                    dirUp = TextFindDlg.DirectionUp;
                    TextFindDlg.Close();
                }

                if (Editor.SelectionLength > 0)
                {
                    text = Editor.SelectedText;
                }

                TextFindDlg = new FindDlg
                {
                    Owner = this,
                    TextToFind = text,
                    CaseSensitive = caseSens,
                    DirectionUp = dirUp,
                };
                TextFindDlg.FindReplace += TextFindDlg_FindReplace;

                TextFindDlg.Show();
            }
        }

        private void TextFindDlg_FindReplace(object sender, FindReplaceEventArgs e)
        {

            int startIndex = -1; 

            switch (e.Mode)
            {
                case FindReplaceMode.FindNext:
                    startIndex = Editor.CaretIndex + Editor.SelectionLength;
                    startIndex = Editor.Text.IndexOf(e.TextToFind, startIndex, 
                        e.CaseSensitive ? StringComparison.CurrentCulture :StringComparison.CurrentCultureIgnoreCase);
                    break;

                case FindReplaceMode.FindPrev:
                    startIndex = Editor.CaretIndex;
                    startIndex = Editor.Text.LastIndexOf(e.TextToFind, startIndex,
                        e.CaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
                    break;

                case FindReplaceMode.ReplaceNext:
                    break;

                case FindReplaceMode.ReplaceAll:
                    break;

                default:
                    break;
            }


            if (startIndex >= 0)
            {
                Editor.Select(startIndex, e.TextToFind.Length);
                Editor.Focus();
            }
        }

        private void MenuItem_FindNext_Click(object sender, RoutedEventArgs e)
        {
            FindLastText(sender, false);
        }

        private void MenuItem_FindPrev_Click(object sender, RoutedEventArgs e)
        {
            FindLastText(sender, true);
        }

        private void FindLastText(object sender, bool dirUp)
        {
            if (Editor.Text.Length > 0)
            {
                if (TextFindDlg != null)
                {
                    FindReplaceEventArgs eventArgs = new FindReplaceEventArgs
                    {
                        CaseSensitive = TextFindDlg.CaseSensitive,
                        TextToFind = TextFindDlg.TextToFind,
                        TextToReplace = null,
                        Mode = dirUp ? FindReplaceMode.FindPrev : FindReplaceMode.FindNext,
                    };

                    TextFindDlg_FindReplace(sender, eventArgs);
                }
                else
                {
                    ShowFindDialog("", false, dirUp);
                }
            }
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
                }

                if (ToolBarButton_Redo.IsEnabled != Editor.CanRedo)
                {
                    ToolBarButton_Redo.IsEnabled = Editor.CanRedo;
                }

                if (ToolBarButton_Find.IsEnabled != (Editor.Text.Length > 0))
                {
                    ToolBarButton_Find.IsEnabled = Editor.Text.Length > 0;
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

                    case Key.F:
                        MenuItem_Find_Click(sender, null);
                        break;

                    default:
                        e.Handled = false;
                        return;
                }
            }
            else if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                switch (e.Key)
                {
                    case Key.F3:
                        MenuItem_FindPrev_Click(sender, null);
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
                    case Key.F3:
                        MenuItem_FindNext_Click(sender, null);
                        break;

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
