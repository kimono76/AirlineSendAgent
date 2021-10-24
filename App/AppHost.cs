using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using AirlineSendAgent.Client;
using AirlineSendAgent.Data;
using AirlineSendAgent.Dtos;
using System.Linq;

namespace AirlineSendAgent.App
{
    public class AppHost:IntAppHost
    {
        private readonly SendAgentDbContext _dbContext;
        private readonly IntWebhookClient _webhookClient;

        public AppHost(SendAgentDbContext dbContext,IntWebhookClient webhookClient)
        {
            _dbContext = dbContext;
            _webhookClient = webhookClient;
        
        }
        public void Run(){
            var factory = new ConnectionFactory(){ HostName="localhost", Port=5672 };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel()){
                channel.ExchangeDeclare(exchange:"trigger", type: ExchangeType.Fanout);
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(
                    queue: queueName,
                    exchange:"trigger",
                    routingKey:""
                    );
                var consumer = new EventingBasicConsumer(channel);
                Console.WriteLine("Listening on the message bus ...");

                consumer.Received += async (ModuleHandle, ea)=>{
                    System.Console.WriteLine("Event is triggered!");
                    var body = ea.Body;
                    var notificationMessage =Encoding.UTF8.GetString(body.ToArray());
                    var message = JsonSerializer.Deserialize<NotificationMessageDto>(notificationMessage);
                    var webhookToSend = new FlightDetailChangePayloadDto{
                        WebhookType = message.WebhookType,
                        WebhookURI = string.Empty,
                        Secret = string.Empty,
                        Publisher = string.Empty,
                        OldPrice = message.OldPrice,
                        NewPrice = message.NewPrice,
                        FlightCode = message.FlightCode,
                    };
                    var subscriptionsWithSameType = _dbContext.WebhookSubscriptions.Where(x=>x.WebhookType.Equals(message.WebhookType));
                    foreach (var whs in subscriptionsWithSameType)
                    {
                        webhookToSend.WebhookURI = whs.WebhookURI;
                        webhookToSend.Secret = whs.Secret;
                        webhookToSend.Publisher = whs.WebhookPublisher;

                        await _webhookClient.SendWebhookNotification(webhookToSend);
                    }
                };
                channel.BasicConsume(
                    queue:queueName,
                    autoAck: true,
                    consumer:consumer
                );
                Console.ReadLine();
            }
        }
    }
}