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
    public static class ListBoxUtils
    {
        private static HitTestFilterBehavior HTFilterCallback(DependencyObject potentialHitTestTarget)
        {
            if (potentialHitTestTarget.GetType() == typeof(ListBoxItem))
            {
                LBIResult = (ListBoxItem)potentialHitTestTarget;
                return HitTestFilterBehavior.Stop;
            }
            else
                return HitTestFilterBehavior.Continue;
        }

        private static HitTestResultBehavior HTResultCallback(HitTestResult result)
        {
            return HitTestResultBehavior.Stop;
        }

        private static ListBoxItem LBIResult;

        public static ListBoxItem GetListItemFromPoint(this ListBox lb, Point p)
        {
            LBIResult = null;
            VisualTreeHelper.HitTest(lb,
                new HitTestFilterCallback(HTFilterCallback),
                new HitTestResultCallback(HTResultCallback), 
                new PointHitTestParameters(p));

            return LBIResult;
        }

        public static int GetListItemIndexFromPoint(this ListBox lb, Point p)
        {
            return lb.Items.IndexOf(GetListItemFromPoint(lb, p));
        } 
    }
}
