using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Walterlv.Whiteman
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            _keyboardHook = new KeyboardHook();
            _keyboardHook.Start();
            Application.Current.Exit += (s, e) => _keyboardHook.Stop();
            _keyboardHook.CtrlShift += (s, e) => Generate(true, true);
            _keyboardHook.Ctrl += (s, e) => Generate(false, true);
            MouseLeftButtonDown += (s, e) => Generate(true, false);
            MouseRightButtonDown += (s, e) => Generate(false, false);

            UpdateCircles(0);
            Generate(true, false);
        }

        private void Generate(bool pascal = true, bool write = false)
        {
            var text = _randomIdentifier.Generate(pascal);
            OutputTextBlock.Text = text;
            if (write)
            {
                _keyboardHook.Send(text);
            }
            else
            {
                Clipboard.SetData(DataFormats.Text, text);
            }
        }

        private void OnActivated(object sender, EventArgs e)
        {
            foreach (var circle in RootPanel.Children.OfType<MovingCircle>())
            {
                circle.IsAnimationEnabled = true;
            }
        }

        private void OnDeactivated(object sender, EventArgs e)
        {
            foreach (var circle in RootPanel.Children.OfType<MovingCircle>())
            {
                circle.IsAnimationEnabled = false;
            }
        }

        private readonly KeyboardHook _keyboardHook;

        private readonly RandomIdentifier _randomIdentifier = new RandomIdentifier();

        private void WordCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateCircles((int) e.NewValue);
            Generate(true, false);
        }

        private void UpdateCircles(int count)
        {
            var oldValue = RootPanel.Children.OfType<MovingCircle>().Count();
            var newValue = count;
            newValue = newValue <= 0 ? 3 : newValue;
            _randomIdentifier.WordCount = newValue;

            if (newValue < oldValue)
            {
                for (var i = newValue; i < oldValue; i++)
                {
                    RootPanel.Children.RemoveAt(newValue);
                }
            }
            else if (newValue > oldValue)
            {
                for (var i = oldValue; i < newValue; i++)
                {
                    RootPanel.Children.Insert(oldValue, new MovingCircle());
                }
            }

            var circles = RootPanel.Children.OfType<MovingCircle>().ToList();
            for (var i = 0; i < circles.Count; i++)
            {
                var circle = circles[i];
                circle.BrushOpacity = (i + 1.0) / circles.Count / 2;
                circle.CenterRange = 1.0 / (15 + i * 2.5);
                circle.RadiusBaseRange = 1.0 / (2.5 + i);
                circle.RadiusRange = 1.0 / 50.0;
            }

            circles[circles.Count - 1].BrushOpacity = 1.0;
        }
    }
}
