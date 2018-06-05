using System;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.Vecc.RabbitMQ
{
    /// <summary>
    /// Builds the messages that are sent to RabbitMQ
    /// </summary>
    public interface IRabbitMessageBuilder
    {
        /// <summary>
        /// The RabbitMQ exchange to publish events to.
        /// </summary>
        /// <param name="logEvent">The LogEvent that is being sent.</param>
        /// <returns></returns>
        string GetExchange(LogEvent logEvent);

        /// <summary>
        /// The RabbitMQ exchange to publish events to.
        /// </summary>
        /// <param name="logEvent">The LogEvent that is being sent.</param>
        /// <param name="textFormatter">The ITextFormatter to use when formatting the message. If null, this will default to the built-in Serilog JsonFormatter.</param>
        /// <param name="formatProvider">The IFormatProvider to use when formatting the message. If null, this will default to the standard .NET format provider.</param>
        /// <returns></returns>
        byte[] GetMessage(LogEvent logEvent, IFormatProvider formatProvider, ITextFormatter textFormatter);

        /// <summary>
        /// The RabbitMQ exchange to publish events to.
        /// </summary>
        /// <param name="logEvent">The LogEvent that is being sent.</param>
        /// <returns></returns>
        string GetRoutingKey(LogEvent logEvent);
    }
}
