using System;
using System.Linq;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Services.Logging;

namespace StockManagementSystem.Services.Events
{
    public class EventPublisher : IEventPublisher
    {
        /// <summary>
        /// Publish event to consumers
        /// </summary>
        /// <typeparam name="TEvent">Type of event</typeparam>
        /// <param name="event">Event object</param>
        public virtual void Publish<TEvent>(TEvent @event)
        {
            var consumers = EngineContext.Current.ResolveAll<IConsumer<TEvent>>().ToList();
            foreach (var consumer in consumers)
            {
                try
                {
                    consumer.HandleEvent(@event);
                }
                catch (Exception exception)
                {
                    try
                    {
                        EngineContext.Current.Resolve<ILogger>()?.Error(exception.Message, exception);
                    }
                    catch
                    {
                        //do nothing
                    }
                }
            }
        }
    }
}