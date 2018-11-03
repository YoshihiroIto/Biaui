using Biaui.Controls.Mock.Foundation.Interface;

namespace Biaui.Controls.Mock
{
    /// <summary>
    /// アプリケーション用コンフィグ
    /// </summary>
    public class AppConfig
    {
        public void Load(IRepository<AppConfig> repos)
            => CopyFrom(repos.LoadAsync().Result);

        public void Save(IRepository<AppConfig> repos)
            => repos.SaveAsync(this).Wait();

        // ReSharper disable once UnusedParameter.Local
        private void CopyFrom(AppConfig src)
        {
        }
    }
}