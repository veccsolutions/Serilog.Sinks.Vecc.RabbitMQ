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
        /// <param name="logEvent">The <seealso cref="LogEvent"/> that is being sent.</param>
        /// <returns></returns>
        string GetExchange(LogEvent logEvent);

        /// <summary>
        /// The RabbitMQ exchange to publish events to.
        /// </summary>
        /// <param name="logEvent">The <seealso cref="LogEvent"/> that is being sent.</param>
        /// <returns></returns>
        byte[] GetMessage(LogEvent logEvent, IFormatProvider formatProvider, ITextFormatter textFormatter);

        /// <summary>
        /// The RabbitMQ exchange to publish events to.
        /// </summary>
        /// <param name="logEvent">The <seealso cref="LogEvent"/> that is being sent.</param>
        /// <returns></returns>
        string GetRoutingKey(LogEvent logEvent);
    }
}
