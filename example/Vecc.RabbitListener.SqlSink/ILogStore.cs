using System;

namespace Vecc.RabbitListener.SqlSink
{
    public interface ILogStore
    {
        void StoreLogEntry(string applicationName, DateTime timestamp, string logDetail);
    }
}
