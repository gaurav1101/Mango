using Mango.Services.EmailAPI.Messaging;

namespace Mango.Services.EmailAPI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        private static IAzureServiceBusConsumer _azureServiceBusConsumer { get; set; }
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            _azureServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            hostApplicationLife.ApplicationStopping.Register(onStop);
            hostApplicationLife.ApplicationStarted.Register(onStart);
            return app;
        }

        private static void onStart()
        {
            _azureServiceBusConsumer.Start();
        }

        private static void onStop()
        {
            _azureServiceBusConsumer.Stop();
        }
    }
}
