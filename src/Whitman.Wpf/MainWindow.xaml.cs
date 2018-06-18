using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Walterlv.Whitman
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

            WordCountSlider.Opacity = 0.0;
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

            WordCountSlider.Opacity = 0.0;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(WordCountSlider);
            var x = position.X > 0 && position.X < WordCountSlider.ActualWidth
                ? 0
                : Math.Min(Math.Abs(position.X), Math.Abs(position.X - WordCountSlider.ActualWidth));
            var y = position.Y > 0 && position.Y < WordCountSlider.ActualHeight
                ? 0
                : Math.Min(Math.Abs(position.Y), Math.Abs(position.Y - WordCountSlider.ActualHeight));
            var distance = Math.Sqrt(x * x + y * y);
            var opacity = (100 - distance) / 100;
            opacity = opacity < 0 ? 0 : opacity;
            opacity = opacity > 1 ? 1 : opacity;
            WordCountSlider.Opacity = opacity;
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
            _randomIdentifier.WordCount = newValue;
            newValue = newValue <= 0 ? 3 : newValue;

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
