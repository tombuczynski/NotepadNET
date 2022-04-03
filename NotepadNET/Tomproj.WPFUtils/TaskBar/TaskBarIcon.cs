using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Tomproj.WPFUtils.TaskBar
{
    public enum IconAction
    {
        Click, DoubleClick, MsgClick
    }

    public class IconActionEventArgs : EventArgs
    {
        public IconActionEventArgs(IconAction action)
        {
            Action = action;
        }

        public IconAction Action { get; }
    }

    public class TaskBarIcon : IDisposable
    {
        private readonly System.Windows.Forms.NotifyIcon NIcon;
        private ContextMenu CxMenu = null;

        public event EventHandler<IconActionEventArgs> ActionPerformed;

        public TaskBarIcon()
        {
            var attr = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>();

            Uri uri = new Uri("/AppIcon.ico", UriKind.Relative);
            var res = Application.GetResourceStream(uri);
            System.Drawing.Icon icon = new System.Drawing.Icon(res.Stream);

            NIcon = new System.Windows.Forms.NotifyIcon
            {
                Text = attr.Title,
                Icon = icon,
            };

            NIcon.MouseClick += NIcon_MouseClick;
            NIcon.MouseDoubleClick += NIcon_MouseDoubleClick;
            NIcon.BalloonTipClicked += NIcon_BalloonTipClicked;
        }

        protected virtual void OnIconAction(IconAction action)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<IconActionEventArgs> handler = ActionPerformed;

            IconActionEventArgs ea = new IconActionEventArgs(action);

            handler?.Invoke(this, ea);
        }

        public void Show() => NIcon.Visible = true;

        public void Hide() => NIcon.Visible = false;

        public void Close()
        {
            NIcon.Visible = false;
            Dispose();
        }

        public void Dispose() => NIcon.Dispose();

        public void SetMenu(ContextMenu menu) => CxMenu = menu;


        public void ShowInfoMsg(string title, string msg)
        {
            NIcon.ShowBalloonTip(2000, title, msg, System.Windows.Forms.ToolTipIcon.Info);
        }

        public void ShowWarnMsg(string title, string msg)
        {
            NIcon.ShowBalloonTip(2000, title + " - ostrzeżenie", msg, System.Windows.Forms.ToolTipIcon.Warning);
        }

        public void ShowErrorMsg(string title, string msg)
        {
            NIcon.ShowBalloonTip(2000, title + " - błąd", msg, System.Windows.Forms.ToolTipIcon.Error);
        }

        private void NIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                OnIconAction(IconAction.DoubleClick);
            }
        }

        private void NIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                CxMenu.IsOpen = false;
                OnIconAction(IconAction.Click);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right && CxMenu != null && !CxMenu.IsOpen)
            {
                CxMenu.IsOpen = true;
            }
        }

        private void NIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            OnIconAction(IconAction.MsgClick);
        }

    }
}
