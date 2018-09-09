using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Walterlv.Whitman
{
    public partial class WordOptionControl : UserControl
    {
        public WordOptionControl()
        {
            InitializeComponent();
            MouseLeftButtonDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseLeave += OnMouseLeave;
        }

        private readonly bool[] _wordCountOption = new bool[5] { true, true, true, true, false };
        private readonly bool[] _syllableCountOption = new bool[5] { true, true, true, true, true };
        private int _movingCellIndex = -1;
        private int _movingLineIndex = -1;

        private static readonly Brush ActiveBrush = new SolidColorBrush(Colors.White) { Opacity = 0.7 };
        private static readonly Brush InactiveBrush = new SolidColorBrush(Colors.White) { Opacity = 0.2 };
        private static readonly Brush HoverBrush = new SolidColorBrush(Colors.White) { Opacity = 0.5 };

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var (lineIndex, cellIndex) = GetIndexes(e.GetPosition(this));
            var oldActivationState = _wordCountOption[lineIndex];

            if (!oldActivationState)
            {
                _wordCountOption[lineIndex] = true;
                if (cellIndex != 0)
                {
                    _syllableCountOption[cellIndex] = true;
                }
            }
            else if (cellIndex == 0)
            {
                _wordCountOption[lineIndex] = false;
            }
            else
            {
                _syllableCountOption[cellIndex] = !_syllableCountOption[cellIndex];
            }

            UpdateOptions();
            InvalidateVisual();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            (_movingLineIndex, _movingCellIndex) = GetIndexes(e.GetPosition(this));
            InvalidateVisual();
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _movingLineIndex = -1;
            _movingCellIndex = -1;
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            var cellWidth = RenderSize.Width / 5;
            var lineHeight = RenderSize.Height / 5;

            for (var line = 0; line < 5; line++)
            {
                for (var cell = 0; cell < 5; cell++)
                {
                    var rect = new Rect(cellWidth * cell, lineHeight * line, cellWidth, lineHeight);
                    var isHover = line == _movingLineIndex && cell == _movingCellIndex;

                    if (_wordCountOption[line])
                    {
                        if (cell == 0 || _syllableCountOption[cell])
                        {
                            dc.DrawRectangle(ActiveBrush, null, rect);
                        }
                        else if (isHover)
                        {
                            dc.DrawRectangle(HoverBrush, null, rect);
                        }
                        else
                        {
                            dc.DrawRectangle(InactiveBrush, null, rect);
                        }
                    }
                    else if (isHover)
                    {
                        dc.DrawRectangle(HoverBrush, null, rect);
                    }
                    else
                    {
                        dc.DrawRectangle(InactiveBrush, null, rect);
                    }
                }
            }
        }

        private (int lineIndex, int cellIndex) GetIndexes(Point position)
        {
            var cellIndex = (int) (position.X / (RenderSize.Width / 5));
            if (cellIndex < 0) cellIndex = 0;
            if (cellIndex >= 5) cellIndex = 4;
            var lineIndex = (int) (position.Y / (RenderSize.Height / 5));
            if (lineIndex < 0) lineIndex = 0;
            if (lineIndex >= 5) lineIndex = 4;
            return (lineIndex, cellIndex);
        }

        private void UpdateOptions()
        {
            var (minWordCount, maxWordCount) = FindRange(_wordCountOption);
            var (minSyllableCount, maxSyllableCount) = FindRange(_syllableCountOption);
            (MinWordCount, MaxWordCount) = (minWordCount + 1, maxWordCount + 1);
            (MinSyllableCount, MaxSyllableCount) = (minSyllableCount + 1, maxSyllableCount + 1);

            WordOptionChanged?.Invoke(this, new WordOptionChangedEventArgs(
                MinWordCount, MaxWordCount, MinSyllableCount, MaxSyllableCount));

            (int min, int max) FindRange(IReadOnlyList<bool> array)
            {
                int min = 0, max = 0;
                for (var i = 0; i < array.Count; i++)
                {
                    if (array[i])
                    {
                        min = i;
                        break;
                    }
                }

                for (var i = array.Count - 1; i >= 0; i--)
                {
                    if (array[i])
                    {
                        max = i;
                        break;
                    }
                }

                return (min, max);
            }
        }

        public static readonly DependencyProperty MinWordCountProperty = DependencyProperty.Register(
            "MinWordCount", typeof(int), typeof(WordOptionControl), new PropertyMetadata(default(int)));

        public int MinWordCount
        {
            get => (int) GetValue(MinWordCountProperty);
            set => SetValue(MinWordCountProperty, value);
        }

        public static readonly DependencyProperty MaxWordCountProperty = DependencyProperty.Register(
            "MaxWordCount", typeof(int), typeof(WordOptionControl), new PropertyMetadata(default(int)));

        public int MaxWordCount
        {
            get => (int) GetValue(MaxWordCountProperty);
            set => SetValue(MaxWordCountProperty, value);
        }

        public static readonly DependencyProperty MinSyllableCountProperty = DependencyProperty.Register(
            "MinSyllableCount", typeof(int), typeof(WordOptionControl), new PropertyMetadata(default(int)));

        public int MinSyllableCount
        {
            get => (int) GetValue(MinSyllableCountProperty);
            set => SetValue(MinSyllableCountProperty, value);
        }

        public static readonly DependencyProperty MaxSyllableCountProperty = DependencyProperty.Register(
            "MaxSyllableCount", typeof(int), typeof(WordOptionControl), new PropertyMetadata(default(int)));

        public int MaxSyllableCount
        {
            get => (int) GetValue(MaxSyllableCountProperty);
            set => SetValue(MaxSyllableCountProperty, value);
        }

        public static readonly DependencyProperty MaxTotalSyllableCountProperty = DependencyProperty.Register(
            "MaxTotalSyllableCount", typeof(int), typeof(WordOptionControl), new PropertyMetadata(default(int)));

        public int MaxTotalSyllableCount
        {
            get => (int) GetValue(MaxTotalSyllableCountProperty);
            set => SetValue(MaxTotalSyllableCountProperty, value);
        }

        internal event EventHandler<WordOptionChangedEventArgs> WordOptionChanged;
    }

    class WordOptionChangedEventArgs : EventArgs
    {
        public WordOptionChangedEventArgs(int minWordCount, int maxWordCount, int minSyllableCount,
            int maxSyllableCount)
        {
            Option = new WordOption(minWordCount, maxWordCount, minSyllableCount, maxSyllableCount);
        }

        public WordOption Option { get; }
    }

    class WordOption
    {
        public WordOption(int minWordCount, int maxWordCount, int minSyllableCount, int maxSyllableCount)
        {
            MinWordCount = minWordCount;
            MaxWordCount = maxWordCount;
            MinSyllableCount = minSyllableCount;
            MaxSyllableCount = maxSyllableCount;
        }

        public int MinWordCount { get; }
        public int MaxWordCount { get; }
        public int MinSyllableCount { get; }
        public int MaxSyllableCount { get; }
    }
}
