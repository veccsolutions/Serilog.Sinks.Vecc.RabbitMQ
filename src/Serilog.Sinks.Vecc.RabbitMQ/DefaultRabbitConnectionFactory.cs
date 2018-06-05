using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace Serilog.Sinks.Vecc.RabbitMQ
{
    /// <summary>
    /// Default implementation of <seealso cref="IRabbitConnectionFactory" /> for the RabbitMQ sink.
    /// </summary>
    public class DefaultRabbitConnectionFactory : IRabbitConnectionFactory
    {
        private readonly string _application;
        private readonly string _hostname;
        private readonly string _localMachineName;
        private readonly string _password;
        private readonly string _username;
        private readonly string _virtualHost;
        private readonly int _heartbeat;
        private readonly int _port;

        /// <summary>
        /// The constructor for the default rabbit connection factory
        /// </summary>
        /// <param name="application">This is the application name that is set in the property header. If <paramref name="routingKey"/> is null, it will be included in the routing key sent to RabbitMQ.</param>
        /// <param name="hostname">The hostname of the RabbitMQ server</param>
        /// <param name="username">Username to use to connect to the RabbitMQ server</param>
        /// <param name="password">Password to use to connect to the RabbitMQ server</param>
        /// <param name="virtualHost">Virtual Host on the RabbitMQ server containing the exchange</param>
        /// <param name="heartbeat">Interval between RabbitMQ server heartbeats</param>
        /// <param name="port">Port to connect to the RabbitMQ server</param>
        public DefaultRabbitConnectionFactory(string application,
                                              string hostname,
                                              string username,
                                              string password,
                                              string virtualHost,
                                              short heartbeat,
                                              int port)
        {
            _application = application;
            _heartbeat = heartbeat;
            _hostname = hostname;
            _localMachineName = Environment.MachineName;
            _password = password;
            _port = port;
            _username = username;
            _virtualHost = virtualHost;
        }

        public virtual void ConfigureBasicProperties(IBasicProperties basicProperties)
        {
            basicProperties.ContentType = "application/json";
            basicProperties.Headers = new Dictionary<string, object>()
            {
                { "Application", _application },
                { "Hostname", _localMachineName }
            };
            basicProperties.AppId = _application;
        }

        public IConnectionFactory GetConnectionFactory()
        {
            var connectionFactory = new ConnectionFactory
            {
                AutomaticRecoveryEnabled = true,
                HostName = _hostname,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(1),
                Password = _password,
                UserName = _username
            };

            if (_heartbeat > 0)
            {
                connectionFactory.RequestedHeartbeat = (ushort)_heartbeat;
            }

            if (_port > 0)
            {
                connectionFactory.Port = _port;
            }

            if (!string.IsNullOrEmpty(_virtualHost))
            {
                connectionFactory.VirtualHost = _virtualHost;
            }

            return connectionFactory;
        }
        }
    }
