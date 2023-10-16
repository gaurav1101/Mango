using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.ServiceBus
{
    public class MessageBus : IMessageBus
    {
        private string connectionString ="Endpoint=sb://mangoweblocal.servicebus.windows.net/;SharedAccessKeyName=RootAccessPolicy;SharedAccessKey=8vGngTmKNAOqVbOT8WLM39f3xAD/LaWgA+ASbNIJ0yA=;EntityPath=mangoshoppingcartemailqueue";
        public async Task publishMessage(object message, string topic_queue_name)
        {
            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(topic_queue_name);
            var jsonMessage= JsonConvert.SerializeObject(message);
                ServiceBusMessage serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
                {
                    CorrelationId=Guid.NewGuid().ToString(),
                };
            await sender.SendMessageAsync(serviceBusMessage);
            await client.DisposeAsync();
        }
    }
}
