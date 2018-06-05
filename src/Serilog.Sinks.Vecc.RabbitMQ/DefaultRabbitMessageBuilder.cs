using System;
using System.IO;
using System.Text;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.Vecc.RabbitMQ
{
    /// <summary>
    /// Default implentation of <seealso cref="IRabbitMessageBuilder" /> for the RabbitMQ sink.
    /// </summary>
    public class DefaultRabbitMessageBuilder : IRabbitMessageBuilder
    {
        private readonly string _application;
        private readonly string _exchange;
        private readonly string _routingKey;
        private readonly string _routingKeyPrefix;
        private readonly string _routingKeySuffix;

        /// <summary>
        /// The constructor that sets the properties for the messages being sent to RabbitMQ.
        /// </summary>
        /// <param name="exchange">The exchange on the RabbitMQ server to send the messages to.</param>
        /// <param name="application">This is the application name that is set in the property header. If <paramref name="routingKey"/> is null, it will be included in the routing key sent to RabbitMQ.</param>
        /// <param name="routingKeyPrefix">If <paramref name="routingKey"/> is null, this is added to the beginning of the key.</param>
        /// <param name="routingKeySuffix">If <paramref name="routingKey"/> is null, this is added to the end of the key.</param>
        /// <param name="routingKey">If null the routing key will be in the format of <code>{routingKeyPrefix}{application}.{logEvent.Level}{routingKeySuffix}</code>. Otherwise it will be this value.</param>
        public DefaultRabbitMessageBuilder(string exchange,
                                           string application,
                                           string routingKey,
                                           string routingKeyPrefix,
                                           string routingKeySuffix)
        {
            _application = application;
            _exchange = exchange;
            _routingKey = routingKey;
            _routingKeyPrefix = routingKeyPrefix;
            _routingKeySuffix = routingKeySuffix;
        }

        public virtual string GetExchange(LogEvent logEvent) => _exchange;

        public virtual byte[] GetMessage(LogEvent logEvent, IFormatProvider formatProvider, ITextFormatter textFormatter)
        {
            using (var sw = new StringWriter(formatProvider))
            {
                textFormatter.Format(logEvent, sw);

                var data = sw.ToString();
                var bytes = Encoding.UTF8.GetBytes(data);

                return bytes;
            }
        }

        public virtual string GetRoutingKey(LogEvent logEvent)
        {
            var result = _routingKey;
            if (result == null)
            {
                result = $"{_routingKeyPrefix}{_application}.{logEvent.Level}{_routingKeySuffix}";
            }
            return result;
        }
    }
}
