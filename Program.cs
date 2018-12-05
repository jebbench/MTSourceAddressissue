using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.RabbitMqTransport;

namespace SourceAddress
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = RabbitMqBusFactory.Create(configurator =>
            {
                var host = configurator.Host(new Uri("rabbitmq://rabbitmq"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                
                configurator.ReceiveEndpoint(host, "example.com.mytopic", endpointConfigurator =>
                {
                    endpointConfigurator.Consumer(() => new ErrorConsumer());
                });
                
            });
            
            bus.Start();

            bus.Publish<IMessage>(new
            {
                Text = "this is a message"
            });

            

        }
    }

    public interface IMessage
    {
        string Text { get; set; }
    }


    class ErrorConsumer : IConsumer<IMessage>
    {
        public async Task Consume(ConsumeContext<IMessage> context)
        {
            throw new Exception("Oh no!");
        }
    }
    
}