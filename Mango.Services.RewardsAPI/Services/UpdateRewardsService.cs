using Mango.Services.RewardsAPI.Data;
using Mango.Services.RewardsAPI.Models;
using Mango.Services.RewardsAPI.Models;
using Mango.Services.RewardsAPI.Models.Dto;
//using Microsoft.Azure.Amqp.Framing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Text;

namespace Mango.Services.RewardsAPI.Services
{
    public class UpdateRewardsService : IUpdateRewardsService
    {
        private readonly DbContextOptions<ApplicationDBContext> _applicationDBContext;
        public UpdateRewardsService(DbContextOptions<ApplicationDBContext> applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }
        //public async Task UpdateRewards(RewardDto rewardDto)
        //{
        //    StringBuilder message=new StringBuilder();
        //    message.AppendLine("<br/> Email Cart Requested");
        //    message.AppendLine("<br/> Total" + rewardDto.RewardsActivity);
        //    message.AppendLine("<br/>");
        //    message.AppendLine("<ul>");
        //    //foreach(var item in cartDto.CartDetailsDtos)
        //    //{
        //    //    message.AppendLine("<li>");
        //    //    message.AppendLine(item.Product.Name + " - " + item.Count);
        //    //    message.AppendLine("</li>");
        //    //}
        //   // var email=DbContext.
        //    message.AppendLine("</ul>");
        //    await LogAndEmail(message.ToString(), "test");
        //}

        public async Task UpdateRewards(RewardDto rewardDto)
        {
            try
            {
                Rewards rewards = new Rewards
                {
                    RewardsActivity=rewardDto.RewardsActivity,
                    UserId=rewardDto.UserId,
                     OrderId=rewardDto.OrderId,
                };

                using (var dbContext = new ApplicationDBContext(_applicationDBContext))
                {
                    // Assuming DataModels is a DbSet in your ApplicationDBContext
                    await dbContext.Rewards.AddAsync(rewards);
                    await dbContext.SaveChangesAsync();
                }
               
            }
            catch(Exception e) 
            {
                Console.WriteLine(e.Message);
            }
        } 
    }
}
