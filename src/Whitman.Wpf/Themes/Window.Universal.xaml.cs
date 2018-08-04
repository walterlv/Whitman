using System.Runtime.CompilerServices;
using System.Windows;

namespace Walterlv.Whitman.Themes
{
    public partial class UniversalWindowStyle
    {
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
            => SetWindowState(sender, WindowState.Minimized);

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
            => SetWindowState(sender, WindowState.Normal);

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
            => SetWindowState(sender, WindowState.Maximized);

        private void CloseButton_Click(object sender, RoutedEventArgs e)
            => Window.GetWindow((DependencyObject) sender)?.Close();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetWindowState(object sender, WindowState state)
        {
            var window = Window.GetWindow((DependencyObject) sender);
            if (window != null)
            {
                window.WindowState = state;
            }
        }
    }
}
