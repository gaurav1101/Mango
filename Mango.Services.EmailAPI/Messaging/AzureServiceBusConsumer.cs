using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string registerUserConnectionString;
        private readonly string emailShoppingCartProcessor;
        private readonly string registertopicOrQueueName;
		private readonly string topics;
		private readonly string updaterewards_topic;
		private readonly string updaterewards_subscription;
		private readonly ServiceBusProcessor _updateRewardsProcessor;
		private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private ServiceBusProcessor _emailShoppingCartProcessor;
        //private ServiceBusProcessor _registerUserProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration,EmailService emailService)
        {
            _emailService= emailService;
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionStrings");
            //registerUserConnectionString = _configuration.GetValue<string>("RegisterUserConnectionStrings");
            emailShoppingCartProcessor = _configuration.GetValue<string>("TopicsAndQueueNames:EmailShoppingCartQueue");
            //registertopicOrQueueName = _configuration.GetValue<string>("TopicsAndQueueNames:RegisterUserQueue");
            var client = new ServiceBusClient(serviceBusConnectionString);
            //var client2 =new ServiceBusClient(registerUserConnectionString);
            _emailShoppingCartProcessor = client.CreateProcessor(emailShoppingCartProcessor);
			// _registerUserProcessor = client2.CreateProcessor(registertopicOrQueueName);

           
			//for rewards
			topics = _configuration.GetValue<string>("Topic_ConnectionStrings");
			updaterewards_topic = _configuration.GetValue<string>("TopicsAndQueueNames:ordercreatedemail");
			updaterewards_subscription = _configuration.GetValue<string>("TopicsAndQueueNames:ordercreatedrewardsupdate");
			var client3 = new ServiceBusClient(topics);
			_updateRewardsProcessor = client3.CreateProcessor(updaterewards_topic, updaterewards_subscription);

		}

        public async Task Start()
        {
            _emailShoppingCartProcessor.ProcessMessageAsync += onEmailCartRecieved;
            _emailShoppingCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailShoppingCartProcessor.StartProcessingAsync();

			//_registerUserProcessor.ProcessMessageAsync += onNewRegisteredUserRecieved;
			//_registerUserProcessor.ProcessErrorAsync += ErrorHandler;
			//await _registerUserProcessor.StartProcessingAsync();


			_updateRewardsProcessor.ProcessMessageAsync += onRewardsEarned;
			_updateRewardsProcessor.ProcessErrorAsync += ErrorHandler;
			await _updateRewardsProcessor.StartProcessingAsync();

		}

        private Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }

		private async Task onRewardsEarned(ProcessMessageEventArgs arg)
		{
			var message = arg.Message;
			var body = Encoding.UTF8.GetString(message.Body);
			RewardDto rewardDto = JsonConvert.DeserializeObject<RewardDto>(body);
			try
			{
				await _emailService.emailRewardAndLog(rewardDto);
				await arg.CompleteMessageAsync(arg.Message);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		private async Task onEmailCartRecieved(ProcessMessageEventArgs arg)
        {
            var message=arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(body);
            try
            {
               await _emailService.emailCartAndLog(cartDto);
                await arg.CompleteMessageAsync(arg.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task onNewRegisteredUserRecieved(ProcessMessageEventArgs arg)
        {
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            var email = JsonConvert.DeserializeObject<string>(body);
            try
            {
                await _emailService.RegisteredUserEmailAndLog(email);
                await arg.CompleteMessageAsync(arg.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public async Task Stop()
        {
            await _emailShoppingCartProcessor.StopProcessingAsync();
            await _emailShoppingCartProcessor.DisposeAsync();

			// await _registerUserProcessor.StopProcessingAsync();
			//await _registerUserProcessor.DisposeAsync();

			await _updateRewardsProcessor.StopProcessingAsync();
			await _updateRewardsProcessor.DisposeAsync();
		}
    }
}
