using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Whitman.Configs;

namespace Walterlv.Whitman
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new GeneratingConfig();
            InitializeComponent();

            _keyboardHook = new KeyboardHook();
            _keyboardHook.Start();
            Application.Current.Exit += (s, e) => _keyboardHook.Stop();
            _keyboardHook.CtrlShift += (s, e) => Generate(true, true);
            _keyboardHook.Ctrl += (s, e) => Generate(false, true);
            GeneratingPage.MouseLeftButtonDown += (s, e) => Generate(true, false);
            GeneratingPage.MouseRightButtonDown += (s, e) => Generate(false, false);
            GeneratingPage.MouseLeftButtonDown += (s, e) => EffectPanel.Focus();

            UpdateCircles(0);
        }

        public bool IsInSetting
        {
            get => SettingPage.IsHitTestVisible;
            set
            {
                if (Equals(IsInSetting, value))
                {
                    return;
                }

                if (value)
                {
                    GeneratingPage.IsHitTestVisible = false;
                    SettingPage.IsHitTestVisible = true;
                    SettingPage.IsEnabled = true;
                    ((Storyboard)FindResource("Storyboard.GotoSettingPage")).Begin();
                    EffectPanel.HorizontalAlignment = HorizontalAlignment.Left;
                    EffectPanel.VerticalAlignment = VerticalAlignment.Top;
                    EffectPanel.Width = 160;
                    EffectPanel.Height = 240;
                }
                else
                {
                    GeneratingPage.IsHitTestVisible = true;
                    SettingPage.IsHitTestVisible = false;
                    SettingPage.IsEnabled = false;
                    ((Storyboard)FindResource("Storyboard.GotoGeneratingPage")).Begin();
                    EffectPanel.ClearValue(HorizontalAlignmentProperty);
                    EffectPanel.ClearValue(VerticalAlignmentProperty);
                    EffectPanel.ClearValue(WidthProperty);
                    EffectPanel.ClearValue(HeightProperty);
                }
            }
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
                try
                {
                    Clipboard.SetData(DataFormats.Text, text);
                }
                catch (COMException ex)
                {
                }
            }
        }

        private void OnActivated(object sender, EventArgs e)
        {
            foreach (var circle in EffectPanel.Children.OfType<MovingCircle>())
            {
                circle.IsAnimationEnabled = true;
            }
        }

        private void OnDeactivated(object sender, EventArgs e)
        {
            foreach (var circle in EffectPanel.Children.OfType<MovingCircle>())
            {
                circle.IsAnimationEnabled = false;
            }
        }

        private void GeneratingPage_Checked(object sender, RoutedEventArgs e)
        {
            IsInSetting = false;
            EffectPanel.Focus();
        }

        private void SettingPage_Checked(object sender, RoutedEventArgs e)
        {
            IsInSetting = true;
            FirstFocusableSettingItem.Focus();
            SettingPage.ScrollToTop();
        }

        private readonly KeyboardHook _keyboardHook;

        private readonly RandomIdentifier _randomIdentifier = new RandomIdentifier();

        private void OnWordOptionChanged(object sender, WordOptionChangedEventArgs e)
        {
            UpdateCircles(e.Option.MaxWordCount);
            Generate(true, false);
        }

        private void UpdateCircles(int count)
        {
            var oldValue = EffectPanel.Children.OfType<MovingCircle>().Count();
            var newValue = count;
            _randomIdentifier.WordCount = newValue;
            newValue = newValue <= 0 ? 3 : newValue;

            if (newValue < oldValue)
            {
                for (var i = newValue; i < oldValue; i++)
                {
                    EffectPanel.Children.RemoveAt(newValue);
                }
            }
            else if (newValue > oldValue)
            {
                for (var i = oldValue; i < newValue; i++)
                {
                    EffectPanel.Children.Insert(oldValue, new MovingCircle());
                }
            }

            var circles = EffectPanel.Children.OfType<MovingCircle>().ToList();
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

        private bool _isInAboutLink;

        private void AboutLink_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                _isInAboutLink = true;
            }
        }

        private void AboutLink_MouseLeave(object sender, MouseEventArgs e)
        {
            _isInAboutLink = false;
        }

        private void AboutLink_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isInAboutLink)
            {
                Process.Start("https://walterlv.com/");
            }
        }

        private void HandledElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
