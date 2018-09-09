using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Walterlv.Whitman
{
    /// <summary>
    /// Interaction logic for WordOptionControl.xaml
    /// </summary>
    public partial class WordOptionControl : UserControl
    {
        public WordOptionControl()
        {
            InitializeComponent();
        }

        private void CellPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CellPanel_MouseMove(object sender, MouseEventArgs e)
        {
            var source = (UIElement) e.OriginalSource;
            var index = InteractivePanel.Children.IndexOf(source);
            var child = SourceRow.Children[index];

            for (var i = 0; i < SourceRow.Children.Count; i++)
            {
                if (i == index)
                {
                    child.Opacity = 1.0;
                }
                else
                {
                    child.Opacity = 0.5;
                }
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
        public WordOptionChangedEventArgs(int minWordCount, int maxWordCount, int minSyllableCount, int maxSyllableCount)
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
