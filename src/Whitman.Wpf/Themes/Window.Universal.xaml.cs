using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace Walterlv.Whitman.Themes
{
    public partial class UniversalWindowStyle
    {
        public static readonly DependencyProperty TitleBarProperty = DependencyProperty.RegisterAttached(
            "TitleBar", typeof(UniversalTitleBar), typeof(UniversalWindowStyle),
            new PropertyMetadata(null));

        public static UniversalTitleBar GetTitleBar(DependencyObject element)
            => (UniversalTitleBar)element.GetValue(TitleBarProperty);

        public static void SetTitleBar(DependencyObject element, UniversalTitleBar value)
            => element.SetValue(TitleBarProperty, value);

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

    public class UniversalTitleBar
    {
        public Color ForegroundColor { get; set; } = Colors.Black;
        public Color InactiveForegroundColor { get; set; } = Color.FromRgb(0xEE, 0xEE, 0xEE);
    }
}
