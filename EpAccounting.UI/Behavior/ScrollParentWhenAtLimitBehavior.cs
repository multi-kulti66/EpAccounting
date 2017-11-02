// ///////////////////////////////////
// File: ScrollParentWhenAtLimitBehavior.cs
// Last Change: 28.10.2017  11:54
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.Behavior
{
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;



public class ScrollParentWhenAtLimitBehavior : Behavior<FrameworkElement>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        this.AssociatedObject.PreviewMouseWheel += this.PreviewMouseWheel;
    }

    protected override void OnDetaching()
    {
        this.AssociatedObject.PreviewMouseWheel -= this.PreviewMouseWheel;
        base.OnDetaching();
    }

    private static T GetVisualChild<T>(DependencyObject parent) where T : Visual
    {
        T child = default(T);

        int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < numVisuals; i++)
        {
            Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
            child = v as T;
            if (child == null)
            {
                child = GetVisualChild<T>(v);
            }
            if (child != null)
            {
                break;
            }
        }

        return child;
    }

    private void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        ScrollViewer scrollViewer = GetVisualChild<ScrollViewer>(this.AssociatedObject);
        double scrollPos = scrollViewer.ContentVerticalOffset;

        if ((scrollPos == scrollViewer.ScrollableHeight && e.Delta < 0) || (scrollPos == 0 && e.Delta > 0))
        {
            e.Handled = true;
            MouseWheelEventArgs e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            e2.RoutedEvent = UIElement.MouseWheelEvent;
            this.AssociatedObject.RaiseEvent(e2);
        }
    }
}
}