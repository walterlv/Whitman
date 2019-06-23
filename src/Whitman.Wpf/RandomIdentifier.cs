using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Whitman.Configs;

namespace Walterlv.Whitman
{
    public class RandomIdentifier
    {
        public GeneratingConfig Configs
        {
            get => _configs;
            set => _configs = value.CheckValid() ? value : throw new ArgumentException("用于生成随机标识符的生成配置必须是可以被计算出来的合理配置");
        }

        public string Generate(bool pascal)
        {
            var builder = new StringBuilder();
            while (true)
            {
                builder.Clear();
                var totalCount = RandomGenerate(builder, pascal);
                if (totalCount >= Configs.MinimalTotalSyllableCount && totalCount <= Configs.MaximumTotalSyllableCount)
                {
                    return builder.ToString();
                }
                Debug.WriteLine($"[Random] 总音节数 {totalCount} 不在范围 ({Configs.MinimalTotalSyllableCount}, {Configs.MaximumTotalSyllableCount}) 内，重新生成");
            }
        }

        private int RandomGenerate(StringBuilder builder, bool pascal)
        {
            var wordCount = GetWeightRandom(Configs.MinimalWordCount, Configs.MaximumWordCount, Configs.GetWordWeights());
            var totalSyllableCount = 0;
            Debug.WriteLine($"[Random] 生成单词数 {wordCount}");

            for (var i = 0; i < wordCount; i++)
            {
                var syllableCount = GetWeightRandom(Configs.MinimalSyllableCount, Configs.MaximumSyllableCount, Configs.GetSyllableWeights());
                totalSyllableCount += syllableCount;
                Debug.WriteLine($"[Random] 生成音节数 {syllableCount}");

                for (var j = 0; j < syllableCount; j++)
                {
                    var consonant = Consonants[_random.Next(Consonants.Count)];
                    var vowel = Vowels[_random.Next(Vowels.Count)];
                    if ((pascal || i != 0) && j == 0)
                    {
                        consonant = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(consonant);
                    }

                    builder.Append(consonant);
                    builder.Append(vowel);
                }
            }

            Debug.WriteLine($"[Random] 总音节数 {totalSyllableCount}");
            return totalSyllableCount;
        }

        /// <summary>
        /// 获取带权值的随机值。
        /// </summary>
        /// <param name="min">随机时可能取得的最小值（可被取到）。</param>
        /// <param name="max">随机时可能取得的最大值（可被取到）。</param>
        /// <param name="weights">随时时所取的可能值中，从 1 开始的权值。如果数组长度不够，那么后续所有数的权值都是 0。</param>
        /// <returns>一个介于最小值和最大值之间的随机值，以权值来随机。</returns>
        private int GetWeightRandom(int min, int max, IEnumerable<int> weightEnumerable)
        {
            var sourceArray = weightEnumerable.ToArray();
            var weights = new int[max];
            Array.Copy(sourceArray, 0, weights, 0, Math.Min(sourceArray.Length, weights.Length));
            for (var i = 0; i < min - 1; i++)
            {
                weights[i] = 0;
            }

            var sum = weights.Sum();
            var value = _random.Next(0, sum);
            for (int i = min; i <= max; i++)
            {
                value -= weights[i - 1];
                if (value < 0)
                {
                    return i;
                }
            }

            return max;
        }

        private readonly Random _random = new Random();
        private GeneratingConfig _configs = new GeneratingConfig();

        private static readonly List<string> Consonants = new List<string>
        {
            "q","w","r","t","y","p","s","d","f","g","h","j","k","l","z","x","c","v","b","n","m",
            "w","r","t","p","s","d","f","g","h","j","k","l","c","b","n","m",
            "r","t","p","s","d","h","j","k","l","c","b","n","m",
            "r","t","s","j","c","n","m",
            "tr","dr","ch","wh","st",
            "s","s"
        };

        private static readonly List<string> Vowels = new List<string>
        {
            "a","e","i","o","u",
            "a","e","i","o","u",
            "a","e","i",
            "a","e",
            "e",
            "ar","as","ai","air","ay","al","all","aw",
            "ee","ea","ear","em","er","el","ere",
            "is","ir",
            "ou","or","oo","ou","ow",
            "ur"
        };
    }
}