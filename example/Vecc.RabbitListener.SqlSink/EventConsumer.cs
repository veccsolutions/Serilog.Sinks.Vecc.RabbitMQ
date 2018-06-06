using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;

namespace Vecc.RabbitListener.SqlSink
{
    public class EventConsumer : DefaultBasicConsumer
    {
        private readonly string _identifier;
        private readonly ILogStore _logStore;

        public EventConsumer(string identifier, ILogStore logStore)
            : base()
        {
            _identifier = identifier;
            _logStore = logStore;
        }

        public override void HandleBasicDeliver(string consumerTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties properties,
            byte[] body)
        {
            base.HandleBasicDeliver(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body);

            var data = Encoding.Default.GetString(body);
            DateTime timestamp;

            Console.WriteLine(_identifier + " -- ");
            var application = Encoding.Default.GetString(properties.Headers?["Application"] as byte[]);

            if (properties.IsHeadersPresent())
            {
                foreach (var kvp in properties.Headers)
                {
                    object kvpValue = null;
                    if (kvp.Value is byte[])
                    {
                        kvpValue = Encoding.Default.GetString((byte[])kvp.Value);
                    }
                    else
                    {
                        kvpValue = kvp.Value;
                    }

                    Console.WriteLine("{0:-10} - {1}", kvp.Key, kvpValue);
                }
            }

            if (properties.ContentType?.Equals("application/json", StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                var o = JsonConvert.DeserializeObject(data) as JObject;
                timestamp = GetDateTime(o, "Timestamp") ?? DateTime.UtcNow;

                Console.WriteLine(o["RenderedMessage"]?.Value<string>());
                Console.WriteLine(timestamp.ToUniversalTime());
                Console.WriteLine(timestamp);
                Console.WriteLine(o["Level"]?.Value<string>());
            }
            else
            {
                timestamp = DateTime.UtcNow;
            }

            _logStore.StoreLogEntry(application, timestamp, data);

            Console.WriteLine(data);
        }

        private DateTime? GetDateTime(JObject o, string property)
        {
            if (o.ContainsKey(property))
            {
                try
                {
                    var result = o[property].Value<DateTime?>();

                    return result;
                }
                catch
                {
                }
            }
            return null;
        }
    }
}
