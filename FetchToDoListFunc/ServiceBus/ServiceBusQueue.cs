using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FetchToDoListFunc.ServiceBus
{
    public class ServieBusQueue : IServiceBusQueue
    {
        private readonly IConfiguration _configuration;
        public ServieBusQueue(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendMessageAsync(string serviceBusMessage)
        {
            try
            {
                string queueName = "todolist-queue";
                ServiceBusClient serviceBusClient = new ServiceBusClient(_configuration.GetConnectionString("AzurServiceBusConnectionString"), new ServiceBusClientOptions()
                {
                    TransportType = ServiceBusTransportType.AmqpWebSockets
                });
                ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(queueName);
                await serviceBusSender.SendMessageAsync(new ServiceBusMessage(serviceBusMessage));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
