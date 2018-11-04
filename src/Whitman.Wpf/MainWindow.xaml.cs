﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Toolkit.Wpf.UI.XamlHost;
using RoutedEventArgs = Windows.UI.Xaml.RoutedEventArgs;

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
            ContentPanel.MouseLeftButtonDown += (s, e) => Generate(true, false);
            ContentPanel.MouseRightButtonDown += (s, e) => Generate(false, false);

            UpdateCircles(0);
            //Dispatcher.InvokeAsync(() => Generate(true, false));
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
            foreach (var circle in ContentPanel.Children.OfType<MovingCircle>())
            {
                circle.IsAnimationEnabled = true;
            }
        }

        private void OnDeactivated(object sender, EventArgs e)
        {
            foreach (var circle in ContentPanel.Children.OfType<MovingCircle>())
            {
                circle.IsAnimationEnabled = false;
            }
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
            var oldValue = ContentPanel.Children.OfType<MovingCircle>().Count();
            var newValue = count;
            _randomIdentifier.WordCount = newValue;
            newValue = newValue <= 0 ? 3 : newValue;

            if (newValue < oldValue)
            {
                for (var i = newValue; i < oldValue; i++)
                {
                    ContentPanel.Children.RemoveAt(newValue);
                }
            }
            else if (newValue > oldValue)
            {
                for (var i = oldValue; i < newValue; i++)
                {
                    ContentPanel.Children.Insert(oldValue, new MovingCircle());
                }
            }

            var circles = ContentPanel.Children.OfType<MovingCircle>().ToList();
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
