using Mango.Services.RewardsAPI.Models.Dto;

namespace Mango.Services.RewardsAPI.Services
{
    public interface IUpdateRewardsService
    {
        Task UpdateRewards(RewardDto rewardDto);
        //Task RegisteredUserEmailAndLog(string email);
    }
}
