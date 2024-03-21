using Azure.Messaging.ServiceBus;
using Mango.Services.RewardsAPI.Models.Dto;
using Mango.Services.RewardsAPI.Services;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace Mango.Services.RewardsAPI.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string registerUserConnectionString;
        private readonly string updaterewards_topic;
		private readonly string updaterewards_subscription;
		private readonly string registertopicOrQueueName;
        private readonly IConfiguration _configuration;
        private readonly UpdateRewardsService _emailService;
        private ServiceBusProcessor _updateRewardsProcessor;
        //private ServiceBusProcessor _registerUserProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration,UpdateRewardsService emailService)
        {
            _emailService= emailService;
            _configuration= configuration;
            registerUserConnectionString = _configuration.GetValue<string>("RegisterUserConnectionStrings");
			updaterewards_topic = _configuration.GetValue<string>("TopicsAndQueueNames:ordercreatedemail");
			updaterewards_subscription = _configuration.GetValue<string>("TopicsAndQueueNames:ordercreatedrewardsupdate");
			var client = new ServiceBusClient(registerUserConnectionString);
			_updateRewardsProcessor = client.CreateProcessor(updaterewards_topic, updaterewards_subscription);
        }

        public async Task Start()
        {
			_updateRewardsProcessor.ProcessMessageAsync += updateRewards;
			_updateRewardsProcessor.ProcessErrorAsync += ErrorHandler;
            await _updateRewardsProcessor.StartProcessingAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task updateRewards(ProcessMessageEventArgs arg)
        {
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            var rewardsdto = JsonConvert.DeserializeObject<RewardDto>(body);
            try
            {
                if(rewardsdto != null)
                {
					await _emailService.UpdateRewards(rewardsdto);
					await arg.CompleteMessageAsync(arg.Message);
				}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public async Task Stop()
        {
            await _updateRewardsProcessor.StopProcessingAsync();
            await _updateRewardsProcessor.DisposeAsync();
        }
    }
}
