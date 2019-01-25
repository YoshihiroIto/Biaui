using Biaui.Controls.Mock.Foundation.Interface;

namespace Biaui.Controls.Mock.Foundation
{
    public class NullLogger : ILogger
    {
        public void Trace(string m)
        {
        }

        public void Debug(string m)
        {
        }

        public void Info(string m)
        {
        }

        public void Warn(string m)
        {
        }

        public void Error(string m)
        {
        }

        public void Fatal(string m)
        {
        }
    }
}