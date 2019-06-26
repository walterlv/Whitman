using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Whitman.Configs;

namespace Walterlv.Whitman.Configs
{
    class FileConfiguration
    {
        private static readonly string _configFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            Assembly.GetExecutingAssembly().GetName().Name,
            "configs.json");

        public static async Task<GeneratingConfig> ReadConfigAsync()
        {
            try
            {
                if (File.Exists(_configFile))
                {
#if NETCOREAPP3_0
                    var text = await File.ReadAllTextAsync(_configFile);
#else
                    var text = File.ReadAllText(_configFile);
#endif
                    var config = JsonConvert.DeserializeObject<GeneratingConfig>(text);
                    return config;
                }
            }
            catch
            {
#if DEBUG
                throw;
#endif
            }
            return new GeneratingConfig();
        }

        public static async Task SaveConfigAsync(GeneratingConfig config)
        {
            try
            {
                var folder = Path.GetDirectoryName(_configFile);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                var text = JsonConvert.SerializeObject(config);
#if NETCOREAPP3_0
                await File.WriteAllTextAsync(_configFile, text);
#else
                File.WriteAllText(_configFile, text);
#endif
            }
            catch
            {
#if DEBUG
                throw;
#endif
            }
        }
    }
}
