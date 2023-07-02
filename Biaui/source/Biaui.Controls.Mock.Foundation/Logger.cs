using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Biaui.Controls.Mock.Foundation;

public class Logger : Controls.Mock.Foundation.Interface.ILogger
{
    public Logger(string outputDir)
    {
        var file = new FileTarget("file")
        {
            Encoding = Encoding.UTF8,
            FileName = outputDir + "/Logs/${date:format=yyyyMMdd}.log",
            ArchiveEvery = FileArchivePeriod.Day,
            ArchiveNumbering = ArchiveNumberingMode.Rolling,
            Layout =
                "${longdate} [${uppercase:${level}:padding=-5}] ${callsite}() ${message} ${exception:format=toString}",
            MaxArchiveFiles = 14
        };

        var conf = new LoggingConfiguration();
        conf.AddTarget(file);

        conf.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, file));

        LogManager.Configuration = conf;
    }

    public void Trace(string m)
    {
        LogManager.GetCurrentClassLogger()?.Trace(m);
    }

    public void Debug(string m)
    {
        LogManager.GetCurrentClassLogger()?.Debug(m);
    }

    public void Info(string m)
    {
        LogManager.GetCurrentClassLogger()?.Info(m);
    }

    public void Warn(string m)
    {
        LogManager.GetCurrentClassLogger()?.Warn(m);
    }

    public void Error(string m)
    {
        LogManager.GetCurrentClassLogger()?.Error(m);
    }

    public void Fatal(string m)
    {
        LogManager.GetCurrentClassLogger()?.Fatal(m);
    }
}