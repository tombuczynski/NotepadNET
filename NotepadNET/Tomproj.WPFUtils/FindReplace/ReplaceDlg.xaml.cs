using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tomproj.WPFUtils.FindReplace
{
    /// <summary>
    /// Interaction logic for ReplaceDlg.xaml
    /// </summary>
    public partial class ReplaceDlg : Window
    {
        public string TextToFind { get => TextBox_FindText.Text; set => TextBox_FindText.Text = value; }
        public string TextToReplace { get => TextBox_ReplaceTo.Text; set => TextBox_ReplaceTo.Text = value; }
        public bool CaseSensitive { get => CheckBox_CaseSens.IsChecked == true; set => CheckBox_CaseSens.IsChecked = value; }


        public event EventHandler<FindReplaceEventArgs> FindReplace;

        public ReplaceDlg()
        {
            InitializeComponent();
        }

        protected virtual void OnFindReplace(FindReplaceMode mode)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<FindReplaceEventArgs> handler = FindReplace;

            FindReplaceEventArgs ea = new FindReplaceEventArgs
            {
                Mode = mode,
                TextToFind = TextToFind,
                TextToReplace = TextToReplace,
                CaseSensitive = CaseSensitive,
            };

            handler?.Invoke(this, ea);
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_FindNext_Click(object sender, RoutedEventArgs e)
        {
            OnFindReplace(FindReplaceMode.FindNext);
        }

        private void Button_Replace_Click(object sender, RoutedEventArgs e)
        {
            OnFindReplace(FindReplaceMode.ReplaceNext);
        }

        private void Button_ReplaceAll_Click(object sender, RoutedEventArgs e)
        {
            OnFindReplace(FindReplaceMode.ReplaceAll);
        }

        private void TextBox_FindText_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool cond = TextBox_FindText.Text.Length > 0;
            Button_FindNext.IsEnabled = cond;
            Button_Replace.IsEnabled = cond;
            Button_ReplaceAll.IsEnabled = cond;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (TextBox_FindText.Text.Length > 0)
            {
                TextBox_FindText.SelectAll();
            }
        }

    }
}
