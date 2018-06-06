using System;
using RabbitMQ.Client;

namespace Vecc.RabbitListener.SqlSink
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionFactory = GetConnectionFactory("localhost", "logs", "guest", "guest");
            var connection = connectionFactory.CreateConnection();
            var model = connection.CreateModel();
            var logStore = new DapperLogStore("Data Source=.;Initial Catalog=LogEntries;User Id=sa;Password=Abcd1234!");

            model.BasicConsume("allevents", true, new EventConsumer("all", logStore));
            model.BasicConsume("debugevents", true, new EventConsumer("debug", logStore));
            model.BasicConsume("verboseevents", true, new EventConsumer("verbose", logStore));
            model.BasicConsume("informationevents", true, new EventConsumer("information", logStore));
            model.BasicConsume("warningevents", true, new EventConsumer("warning", logStore));
            model.BasicConsume("errorevents", true, new EventConsumer("error", logStore));
            model.BasicConsume("fatalevents", true, new EventConsumer("fatal", logStore));

            Console.ReadLine();
            model.Dispose();
            connection.Dispose();
        }

        private static IConnectionFactory GetConnectionFactory(string hostname,
            string virtualHost,
            string username,
            string password,
            int heartbeat = 2,
            int port = 5672)
        {
            var connectionFactory = new ConnectionFactory
            {
                AutomaticRecoveryEnabled = true,
                HostName = hostname,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(1),
                Password = password,
                UserName = username
            };

            if (heartbeat > 0)
            {
                connectionFactory.RequestedHeartbeat = (ushort)heartbeat;
            }

            if (port > 0)
            {
                connectionFactory.Port = port;
            }

            if (!string.IsNullOrEmpty(virtualHost))
            {
                connectionFactory.VirtualHost = virtualHost;
            }

            return connectionFactory;
        }

    }
}
