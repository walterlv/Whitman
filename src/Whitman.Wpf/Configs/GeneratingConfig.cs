using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace Whitman.Wpf.Configs
{
    internal class GeneratingConfig : INotifyPropertyChanged
    {
        private readonly Dispatcher _dispatcher;
        private int _minimalWordCount;
        private int _maximumWordCount;
        private int _minimalSyllableCount;
        private int _maximumSyllableCount;
        private int _minimalTotalSyllableCount;
        private int _maximumTotalSyllableCount;
        private int _word1Weight;
        private int _word2Weight;
        private int _word3Weight;
        private int _word4Weight;
        private int _word5Weight;
        private int _syllable1Weight;
        private int _syllable2Weight;
        private int _syllable3Weight;
        private int _syllable4Weight;
        private int _syllable5Weight;

        public GeneratingConfig() => _dispatcher = Dispatcher.CurrentDispatcher;

        public int MinimalWordCount
        {
            get => _minimalWordCount;
            set => SetValue(ref _minimalWordCount, value);
        }

        public int MaximumWordCount
        {
            get => _maximumWordCount;
            set => SetValue(ref _maximumWordCount, value);
        }

        public int MinimalSyllableCount
        {
            get => _minimalSyllableCount;
            set => SetValue(ref _minimalSyllableCount, value);
        }

        public int MaximumSyllableCount
        {
            get => _maximumSyllableCount;
            set => SetValue(ref _maximumSyllableCount, value);
        }

        public int MinimalTotalSyllableCount
        {
            get => _minimalTotalSyllableCount;
            set => SetValue(ref _minimalTotalSyllableCount, value);
        }

        public int MaximumTotalSyllableCount
        {
            get => _maximumTotalSyllableCount;
            set => SetValue(ref _maximumTotalSyllableCount, value);
        }

        public int Word1Weight
        {
            get => _word1Weight;
            set => SetValue(ref _word1Weight, value);
        }

        public int Word2Weight
        {
            get => _word2Weight;
            set => SetValue(ref _word2Weight, value);
        }

        public int Word3Weight
        {
            get => _word3Weight;
            set => SetValue(ref _word3Weight, value);
        }

        public int Word4Weight
        {
            get => _word4Weight;
            set => SetValue(ref _word4Weight, value);
        }

        public int Word5Weight
        {
            get => _word5Weight;
            set => SetValue(ref _word5Weight, value);
        }

        public int Syllable1Weight
        {
            get => _syllable1Weight;
            set => SetValue(ref _syllable1Weight, value);
        }

        public int Syllable2Weight
        {
            get => _syllable2Weight;
            set => SetValue(ref _syllable2Weight, value);
        }

        public int Syllable3Weight
        {
            get => _syllable3Weight;
            set => SetValue(ref _syllable3Weight, value);
        }

        public int Syllable4Weight
        {
            get => _syllable4Weight;
            set => SetValue(ref _syllable4Weight, value);
        }

        public int Syllable5Weight
        {
            get => _syllable5Weight;
            set => SetValue(ref _syllable5Weight, value);
        }

        private bool SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            _dispatcher.InvokeAsync(
                () => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)),
                DispatcherPriority.Send);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
