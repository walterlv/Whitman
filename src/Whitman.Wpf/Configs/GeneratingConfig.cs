using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace Whitman.Configs
{
    public class GeneratingConfig : INotifyPropertyChanged
    {
        private readonly Dispatcher _dispatcher;
        private int _minimalWordCount = 2;
        private int _maximumWordCount = 5;
        private int _minimalSyllableCount = 2;
        private int _maximumSyllableCount = 4;
        private int _minimalTotalSyllableCount = 4;
        private int _maximumTotalSyllableCount = 12;
        private int _word1Weight = 2;
        private int _word2Weight = 4;
        private int _word3Weight = 4;
        private int _word4Weight = 1;
        private int _word5Weight = 1;
        private int _syllable1Weight = 1;
        private int _syllable2Weight = 4;
        private int _syllable3Weight = 4;
        private int _syllable4Weight = 2;
        private int _syllable5Weight = 1;

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

        public IEnumerable<int> GetWordWeights()
        {
            yield return Word1Weight;
            yield return Word2Weight;
            yield return Word3Weight;
            yield return Word4Weight;
            yield return Word5Weight;
        }

        public IEnumerable<int> GetSyllableWeights()
        {
            yield return Syllable1Weight;
            yield return Syllable2Weight;
            yield return Syllable3Weight;
            yield return Syllable4Weight;
            yield return Syllable5Weight;
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
