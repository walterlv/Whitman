﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Walterlv.Whitman.Configs;
using Whitman.Configs;

namespace Walterlv.Whitman
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _keyboardHook = new KeyboardHook();
            ContentRendered += OnContentRendered;
            Closing += OnClosing;
            UpdateCircles(0);
        }

        private async void OnContentRendered(object sender, EventArgs _)
        {
            _keyboardHook.Start();
            _keyboardHook.CtrlShift += (s, e) => Generate(true, true);
            _keyboardHook.Ctrl += (s, e) => Generate(false, true);
            GeneratingPage.MouseLeftButtonDown += (s, e) => Generate(true, false);
            GeneratingPage.MouseRightButtonDown += (s, e) => Generate(false, false);
            GeneratingPage.MouseLeftButtonDown += (s, e) => EffectPanel.Focus();
            DataContext = await FileConfiguration.ReadConfigAsync().ConfigureAwait(false);
        }

        private void OnClosing(object sender, CancelEventArgs e) => _keyboardHook.Stop();

        public bool IsInSettingPage
        {
            get => SettingPage.IsHitTestVisible;
            set
            {
                if (Equals(IsInSettingPage, value))
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

        private async void GeneratingPage_Checked(object sender, RoutedEventArgs e)
        {
            IsInSettingPage = false;
            EffectPanel.Focus();

            var config = (GeneratingConfig)DataContext;
            if (config?.CheckValid() is true)
            {
                await FileConfiguration.SaveConfigAsync(config).ConfigureAwait(false);
            }
        }

        private void SettingPage_Checked(object sender, RoutedEventArgs e)
        {
            IsInSettingPage = true;
            FirstFocusableSettingItem.Focus();
            SettingScrollViewer.ScrollToTop();
        }

        private readonly KeyboardHook _keyboardHook;
        private readonly RandomIdentifier _randomIdentifier = new RandomIdentifier();
        private bool _isInAboutLink;
        private bool _isInConfigErrorLink;

        private void UpdateCircles(int count)
        {
            var oldValue = EffectPanel.Children.OfType<MovingCircle>().Count();
            var newValue = count;
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

        private void AboutLink_MouseEnter(object sender, MouseEventArgs e)
            => _isInAboutLink = Mouse.LeftButton != MouseButtonState.Pressed;

        private void AboutLink_MouseLeave(object sender, MouseEventArgs e)
            => _isInAboutLink = false;

        private void AboutLink_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isInAboutLink)
            {
                try
                {
                    Process.Start("https://walterlv.com/");
                }
                catch
                {
                    // 因为启动的是 Url，一般不会出现问题。
                }
            }
        }

        private void ConfigErrorLink_MouseEnter(object sender, MouseEventArgs e)
            => _isInConfigErrorLink = Mouse.LeftButton != MouseButtonState.Pressed;

        private void ConfigErrorLink_MouseLeave(object sender, MouseEventArgs e)
            => _isInConfigErrorLink = false;

        private void ConfigErrorLink_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isInConfigErrorLink)
            {
                DataContext = new GeneratingConfig();
            }
        }

        private void HandledElement_MouseDown(object sender, MouseButtonEventArgs e) => e.Handled = true;

        private void SettingTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.Source is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        private void SettingTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var config = (GeneratingConfig)DataContext;
            var reason = config.GetInvalidReason();
            if (string.IsNullOrWhiteSpace(reason))
            {
                ErrorMessageRun.Text = "";
                ConfigErrorPanel.Visibility = Visibility.Collapsed;
                _randomIdentifier.Configs = config.Clone();
                UpdateCircles(config.MinimalWordCount == config.MaximumWordCount
                    ? config.MinimalWordCount
                    : 0);
            }
            else
            {
                ErrorMessageRun.Text = reason;
                ConfigErrorPanel.Visibility = Visibility.Visible;
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
                catch (COMException)
                {
                }
            }
        }
    }
}
