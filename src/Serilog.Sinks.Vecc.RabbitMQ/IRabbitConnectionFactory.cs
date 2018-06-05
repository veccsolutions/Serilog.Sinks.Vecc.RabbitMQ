using RabbitMQ.Client;

namespace Serilog.Sinks.Vecc.RabbitMQ
{
    /// <summary>
    /// Contains methods that are used for creating a connection to RabbitMQ.
    /// </summary>
    public interface IRabbitConnectionFactory
    {
        /// <summary>
        /// Configure the IBasicProperties that are sent with every message. This is called only once on startup.
        /// </summary>
        /// <param name="basicProperties">The properties object to configure</param>
        void ConfigureBasicProperties(IBasicProperties basicProperties);

        /// <summary>
        /// Creates the RabbitMQ IConnectionFactory/>.
        /// </summary>
        /// <returns></returns>
        IConnectionFactory GetConnectionFactory();
    }
}
