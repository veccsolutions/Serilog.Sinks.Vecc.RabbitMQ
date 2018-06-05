using Serilog.Configuration;
using Serilog.Formatting;
using Serilog.Sinks.Vecc.RabbitMQ;
using System;

namespace Serilog
{
    /// <summary>
    /// Extension methods for configuring the RabbitMQ sink
    /// </summary>
    public static class RabbitMQLoggerConfigurationExtensions
    {
        /// <summary>
        /// Adds the RabbitMQ sink to serilog
        /// </summary>
        /// <param name="configuration">Serilog Sink Configuration.</param>
        /// <param name="hostname">The hostname of the RabbitMQ server.</param>
        /// <param name="username">Username to use to connect to the RabbitMQ server.</param>
        /// <param name="password">Password to use to connect to the RabbitMQ server.</param>
        /// <param name="virtualHost">Virtual Host on the RabbitMQ server containing the exchange.</param>
        /// <param name="exchange">The exchange to publish the events to.</param>
        /// <param name="application">This is the application name that is set in the property header. If <paramref name="routingKey"/> is null, it will be included in the routing key sent to RabbitMQ.</param>
        /// <param name="routingKeyPrefix">If <paramref name="routingKey"/> is null, this is added to the beginning of the key.</param>
        /// <param name="routingKeySuffix">If <paramref name="routingKey"/> is null, this is added to the end of the key.</param>
        /// <param name="routingKey">If null the routing key will be in the format of <code>{routingKeyPrefix}{application}.{logEvent.Level}{routingKeySuffix}</code>. Otherwise it will be this value.</param>
        /// <param name="heartbeat">Interval between RabbitMQ server heartbeats. Defaults to 2 seconds.</param>
        /// <param name="port">Port to connect to the RabbitMQ server. Defaults to 5672.</param>
        /// <param name="batchSizeLimit">Maximum locally queued message count before sending to RabbitMQ. Defaults to 50.</param>
        /// <param name="batchSubmitTimeSeconds">Maximum time to allow locally queued messages to be queued before sending to RabbitMQ. Defaults to 2 seconds.</param>
        /// <param name="textFormatter">The ITextFormatter to use when formatting the message. If null, this will default to the built-in Serilog JsonFormatter.</param>
        /// <param name="formatProvider">The IFormatProvider to use when formatting the message. If null, this will default to the standard .NET format provider.</param>
        /// <returns></returns>
        public static LoggerConfiguration RabbitMQ(this LoggerSinkConfiguration configuration,
                                                   string hostname,
                                                   string username,
                                                   string password,
                                                   string virtualHost,
                                                   string exchange,
                                                   string application = "unknown",
                                                   string routingKeyPrefix = null,
                                                   string routingKeySuffix = null,
                                                   string routingKey = null,
                                                   short heartbeat = 2,
                                                   int port = 5672,
                                                   int batchSizeLimit = 50,
                                                   int batchSubmitTimeSeconds = 2,
                                                   ITextFormatter textFormatter = null,
                                                   IFormatProvider formatProvider = null)
        {
            var connectionFactory = new DefaultRabbitConnectionFactory(application, hostname, username, password, virtualHost, heartbeat, port);
            var messageBuilder = new DefaultRabbitMessageBuilder(exchange, application, routingKey, routingKeyPrefix, routingKeySuffix);

            var sink = new RabbitMQSink(batchSizeLimit,
                                        TimeSpan.FromSeconds(batchSubmitTimeSeconds),
                                        textFormatter,
                                        formatProvider,
                                        connectionFactory,
                                        messageBuilder);

            return configuration.Sink(sink);
        }
        /// <summary>
        /// Adds the RabbitMQ sink to serilog
        /// </summary>
        /// <param name="configuration">Serilog Sink Configuration.</param>
        /// <param name="rabbitConnectionFactory">The <see cref="IRabbitConnectionFactory" /> to use when creating the connection to RabbitMQ.</param>
        /// <param name="rabbitMessageBuilder">The <see cref="IRabbitMessageBuilder" /> to use when sending messages to RabbitMQ.</param>
        /// <param name="batchSizeLimit">Maximum locally queued message count before sending to RabbitMQ.</param>
        /// <param name="batchSubmitTimeSeconds">Maximum time to allow locally queued messages to be queued before sending to RabbitMQ.</param>
        /// <param name="textFormatter">The ITextFormatter to use when formatting the message. If null, this will default to the built-in Serilog JsonFormatter.</param>
        /// <param name="formatProvider">The IFormatProvider to use when formatting the message. If null, this will default to the standard .NET format provider.</param>
        /// <returns></returns>
        public static LoggerConfiguration RabbitMQ(this LoggerSinkConfiguration configuration,
                                                   IRabbitConnectionFactory rabbitConnectionFactory,
                                                   IRabbitMessageBuilder rabbitMessageBuilder,
                                                   int batchSizeLimit = 50,
                                                   int batchSubmitTimeSeconds = 2,
                                                   ITextFormatter textFormatter = null,
                                                   IFormatProvider formatProvider = null)
        {
            var sink = new RabbitMQSink(batchSizeLimit,
                                        TimeSpan.FromSeconds(batchSubmitTimeSeconds),
                                        textFormatter,
                                        formatProvider,
                                        rabbitConnectionFactory,
                                        rabbitMessageBuilder);

            return configuration.Sink(sink);
        }
    }
}
