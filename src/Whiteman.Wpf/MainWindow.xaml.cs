using System;
using System.Windows;
using System.Windows.Input;

namespace Walterlv.Whiteman
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var keyboardHook = new KeyboardHook();
            keyboardHook.Start();
            Application.Current.Exit += (s, e) => keyboardHook.Stop();
            keyboardHook.Ctrl += (s, e) =>
            {
                var text = _randomIdentifier.Generate(false);
                OutputTextBlock.Text = text;
                keyboardHook.Send(text);
            };
            keyboardHook.CtrlShift += (s, e) =>
            {
                var text = _randomIdentifier.Generate(true);
                OutputTextBlock.Text = text;
                keyboardHook.Send(text);
            };
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var text = _randomIdentifier.Generate(true);
            OutputTextBlock.Text = text;
            Clipboard.SetData(DataFormats.Text, text);
        }

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var text = _randomIdentifier.Generate(false);
            OutputTextBlock.Text = text;
            Clipboard.SetData(DataFormats.Text, text);
        }

        private void OnActivated(object sender, EventArgs e)
        {
            MovingCircle.IsAnimationEnabled = true;
        }

        private void OnDeactivated(object sender, EventArgs e)
        {
            MovingCircle.IsAnimationEnabled = false;
        }

        private readonly RandomIdentifier _randomIdentifier = new RandomIdentifier();

        private void WordCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var value = (int) e.NewValue;
            _randomIdentifier.WordCount = value;
        }
    }
}
