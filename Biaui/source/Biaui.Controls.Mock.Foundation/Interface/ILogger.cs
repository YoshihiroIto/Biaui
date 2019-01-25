namespace Biaui.Controls.Mock.Foundation.Interface
{
    public interface ILogger
    {
        void Trace(string m);
        void Debug(string m);
        void Info(string m);
        void Warn(string m);
        void Error(string m);
        void Fatal(string m);
    }
}