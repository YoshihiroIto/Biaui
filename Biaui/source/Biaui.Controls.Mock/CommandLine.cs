using System.IO;
using Mono.Options;

namespace Biaui.Controls.Mock
{
    public class CommandLine
    {
        public string AppConfigFilePath { get; set; } =
            Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                "Biaui",
                "App",
                "AppConfig.json"
            );

        public static CommandLine Parse(string[] args)
        {
            var cl = new CommandLine();

            try
            {
                var options = new OptionSet
                {
                    {"config=", "config file path", v => cl.AppConfigFilePath = v}
                };

                options.Parse(args);
            }
            catch
            {
                // ignored
            }

            return cl;
        }
    }
}