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

        public bool CheckValid()
        {
            if (MinimalWordCount <= 0 || MaximumWordCount > 5
                || MinimalSyllableCount <= 0 || MaximumSyllableCount > 5
                || MinimalWordCount > MaximumWordCount
                || MinimalSyllableCount > MaximumSyllableCount)
            {
                return false;
            }

            var minimalTotalSyllableCount = MinimalWordCount * MinimalSyllableCount;
            var maximumTotalSyllableCount = MaximumWordCount * MaximumSyllableCount;

            if (minimalTotalSyllableCount > MinimalTotalSyllableCount
                || maximumTotalSyllableCount < MaximumTotalSyllableCount)
            {
                return false;
            }

            return true;
        }

        public string GetInvalidReason()
        {
            if (MinimalWordCount <= 0) return $"生成的单词数量必须至少 1 个。";
            if (MaximumWordCount > 5) return $"暂时不支持生成大于 5 个单词。";
            if (MinimalSyllableCount <= 0) return $"每个单词必须至少包含 1 个音节。";
            if (MaximumSyllableCount > 5) return $"暂时不支持每个单词生成大于 5 个音节。";
            if (MinimalWordCount > MaximumWordCount) return $"生成的单词数量最小值必须小于或等于最大值。";
            if (MinimalSyllableCount > MaximumSyllableCount) return $"每个单词的音节数量最小值必须小于或等于最大值。";

            var minimalTotalSyllableCount = MinimalWordCount * MinimalSyllableCount;
            var maximumTotalSyllableCount = MaximumWordCount * MaximumSyllableCount;

            if (minimalTotalSyllableCount > MinimalTotalSyllableCount) return $"最小单词数与音节数的乘积依然大于最小总音节数。";
            if (maximumTotalSyllableCount < MaximumTotalSyllableCount) return $"最大单词数与音节数的乘积依然小于最大总音节数。";

            return null;
        }

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

        public GeneratingConfig Clone() => (GeneratingConfig)MemberwiseClone();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
