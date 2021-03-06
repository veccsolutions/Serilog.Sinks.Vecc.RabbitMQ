﻿using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.Vecc.RabbitMQ
{
    /// <summary>
    /// A sink for Serilog that will batch event messages and push them up to a RabbitMQ exchange.
    /// </summary>
    public class RabbitMQSink : PeriodicBatchingSink
    {

        private readonly IBasicProperties _basicProperties;
        private readonly IRabbitMessageBuilder _rabbitMessageBuilder;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private readonly IFormatProvider _formatProvider;
        private readonly IModel _model;
        private readonly ITextFormatter _textFormatter;

        /// <summary>
        /// The Serilog sink that will send log messages to a RabbitMQ server
        /// </summary>
        /// <param name="batchSizeLimit">Maximum locally queued message count before sending to RabbitMQ.</param>
        /// <param name="batchSubmitTimeSeconds">Maximum time to allow locally queued messages to be queued before sending to RabbitMQ.</param>
        /// <param name="textFormatter">The ITextFormatter to use when formatting the message. If null, this will default to the built-in Serilog JsonFormatter.</param>
        /// <param name="formatProvider">The IFormatProvider to use when formatting the message. If null, this will default to the standard .NET format provider.</param>
        /// <param name="rabbitConnectionFactory">The <see cref="IRabbitConnectionFactory" /> to use when creating the connection to RabbitMQ.</param>
        /// <param name="rabbitMessageBuilder">The <see cref="IRabbitMessageBuilder" /> to use when sending messages to RabbitMQ.</param>
        public RabbitMQSink(int batchSizeLimit,
                            TimeSpan batchSubmitTimeSeconds,
                            ITextFormatter textFormatter,
                            IFormatProvider formatProvider,
                            IRabbitConnectionFactory rabbitConnectionFactory,
                            IRabbitMessageBuilder rabbitMessageBuilder)
            : base(batchSizeLimit, batchSubmitTimeSeconds)
        {
            _formatProvider = formatProvider;
            _textFormatter = textFormatter ?? new JsonFormatter(renderMessage: true);
            
            _connectionFactory = rabbitConnectionFactory.GetConnectionFactory();
            _rabbitMessageBuilder = rabbitMessageBuilder;

            _connection = _connectionFactory.CreateConnection();
            _model = _connection.CreateModel();
            _basicProperties = _model.CreateBasicProperties();

            rabbitConnectionFactory.ConfigureBasicProperties(_basicProperties);
        }

        /// <summary>
        /// Send a batch to RabbitMQ.
        /// </summary>
        /// <param name="events">The LogEvent batch to send</param>
        protected override void EmitBatch(IEnumerable<LogEvent> events)
        {
            foreach (var logEvent in events)
            {
                try
                {
                    var exchange = _rabbitMessageBuilder.GetExchange(logEvent);
                    var routingKey = _rabbitMessageBuilder.GetRoutingKey(logEvent);
                    var bytes = _rabbitMessageBuilder.GetMessage(logEvent, _formatProvider, _textFormatter);

                    _model.BasicPublish(exchange, routingKey, _basicProperties, bytes);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Disposes and closes the connections to RabbitMQ.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _model.Dispose();
                _connection.Dispose();
            }
        }
    }
}
