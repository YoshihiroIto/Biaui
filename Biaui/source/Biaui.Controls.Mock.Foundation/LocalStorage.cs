using System.IO;
using System.Threading.Tasks;
using Biaui.Controls.Mock.Foundation.Interface;
using Utf8Json;

namespace Biaui.Controls.Mock.Foundation
{
    public class LocalStorage
    {
        private readonly ILogger _logger;

        public LocalStorage(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<T> ReadAsync<T>(string path)
        {
            try
            {
                if (File.Exists(path) == false)
                {
                    _logger.Info($"Read:Not Found: {path}");
                    return default;
                }

                _logger.Info($"Read: {path}");

                // ReSharper disable once UseAwaitUsing
                using var s = new FileStream(path, FileMode.Open, FileAccess.Read);
                return await JsonSerializer.DeserializeAsync<T>(s).ConfigureAwait(false);
            }
            catch (System.Exception e)
            {
                _logger.Info($"Read:exception: {path}: {e.Message}");
                return default;
            }
        }

        public async Task WriteAsync<T>(T target, string path)
        {
            try
            {
                _logger.Info($"Write: {path}");

                SetupDir(path);

                // ReSharper disable once UseAwaitUsing
                using var s = new FileStream(path, FileMode.Create, FileAccess.Write);
                await JsonSerializer.SerializeAsync(s, target).ConfigureAwait(false);
            }
            catch (System.Exception e)
            {
                _logger.Info($"Write:exception: {path}: {e.Message}");
            }
        }

        private static void SetupDir(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (dir is null)
                return;

            Directory.CreateDirectory(dir);
        }
    }
}